using Stripe;
using StripePoc.Api.Brokers.Loggings;
using LocalPaymentAccount = StripePoc.Api.Models.PaymentAccounts.PaymentAccount;
using LocalPaymentIntent = StripePoc.Api.Models.PaymentIntents.PaymentIntent;
using LocalPaymentIntentStatus = StripePoc.Api.Models.PaymentIntents.PaymentIntentStatus;
using LocalPaymentMethod = StripePoc.Api.Models.PaymentMethods.PaymentMethod;
using LocalSubscription = StripePoc.Api.Models.Subscriptions.Subscription;
using LocalSubscriptionStatus = StripePoc.Api.Models.Subscriptions.SubscriptionStatus;
using StripePoc.Api.Models.Events;

namespace StripePoc.Api.Brokers.Payments
{
    public class PaymentBroker : IPaymentBroker
    {
        private readonly StripeClient client;
        private readonly ILoggingBroker loggingBroker;
        private readonly string webhookSecret;

        public PaymentBroker(IConfiguration configuration, ILoggingBroker loggingBroker)
        {
            this.client = new StripeClient(configuration["Stripe:SecretKey"]);
            this.loggingBroker = loggingBroker;
            this.webhookSecret = configuration["Stripe:WebhookSecret"];
        }

        public async ValueTask<LocalPaymentAccount> CreateCustomerAsync(
            LocalPaymentAccount paymentAccount)
        {
            var options = new CustomerCreateOptions
            {
                Name = paymentAccount.AccountId.ToString(),
                Email = "osunlanaabdulsamad@gmail.com",
                Metadata = new Dictionary<string, string>
                {
                    { "AccountId", paymentAccount.AccountId.ToString() }
                }
            };

            Customer customer = await new CustomerService(this.client).CreateAsync(options);
            paymentAccount.StripeCustomerId = customer.Id;

            return paymentAccount;
        }

        public async ValueTask<string> CreateSetupIntentAsync(string stripeCustomerId)
        {
            var options = new SetupIntentCreateOptions
            {
                Customer = stripeCustomerId,
                Usage = "off_session"
            };

            SetupIntent setupIntent =
                await new SetupIntentService(this.client).CreateAsync(options);

            return setupIntent.ClientSecret;
        }

        public async ValueTask<LocalPaymentMethod> RetrievePaymentMethodAsync(
            LocalPaymentMethod paymentMethod)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentMethod.StripePaymentMethodId))
                    throw new ArgumentException("StripePaymentMethodId is null or empty.");

                this.loggingBroker.LogInformation($"Retrieving Stripe PaymentMethod with ID: '{paymentMethod.StripePaymentMethodId}' (Length: {paymentMethod.StripePaymentMethodId.Length})");

                Stripe.PaymentMethod stripePaymentMethod =
                    await new PaymentMethodService(this.client).GetAsync(paymentMethod.StripePaymentMethodId);

                paymentMethod.Brand = stripePaymentMethod.Card.Brand;
                paymentMethod.Last4 = stripePaymentMethod.Card.Last4;
                paymentMethod.ExpMonth = stripePaymentMethod.Card.ExpMonth;
                paymentMethod.ExpYear = stripePaymentMethod.Card.ExpYear;

                return paymentMethod;
            }
            catch (StripeException stripeEx)
            {
                this.loggingBroker.LogError(stripeEx);
                throw;
            }
            catch (Exception ex)
            {
                this.loggingBroker.LogError(ex);
                throw;
            }
        }

        public async ValueTask<LocalSubscription> CreateSubscriptionAsync(
            LocalSubscription subscription,
            string stripeCustomerId,
            string stripePaymentMethodId)
        {
            string priceId = await GetOrCreatePriceAsync(
                subscription.ServiceName,
                subscription.Amount,
                subscription.Currency,
                subscription.IntervalCount);

            var options = new SubscriptionCreateOptions
            {
                Customer = stripeCustomerId,
                DefaultPaymentMethod = stripePaymentMethodId,
                CollectionMethod = "send_invoice",
                PaymentBehavior = "allow_incomplete",
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions { Price = priceId }
                },
                Expand = new List<string> { "latest_invoice.payment_intent" }
            };

            Stripe.Subscription stripeSubscription =
                await new SubscriptionService(this.client).CreateAsync(options);

            subscription.StripeSubscriptionId = stripeSubscription.Id;
            subscription.Status = MapToSubscriptionStatus(stripeSubscription.Status);

            try
            {
                // In v51, LatestInvoice property is deleted. We extract from RawJObject.
                var latestInvoice = stripeSubscription.RawJObject?["latest_invoice"];
                if (latestInvoice != null && latestInvoice.Type != Newtonsoft.Json.Linq.JTokenType.Null)
                {
                    subscription.CurrentPeriodStart =
                        DateTimeOffset.FromUnixTimeSeconds((long)(latestInvoice["period_start"] ?? 0));
                    subscription.CurrentPeriodEnd =
                        DateTimeOffset.FromUnixTimeSeconds((long)(latestInvoice["period_end"] ?? 0));

                    var piTokens = latestInvoice["payment_intent"];
                    if (piTokens != null && piTokens.Type != Newtonsoft.Json.Linq.JTokenType.Null)
                    {
                        subscription.StripeClientSecret = piTokens["client_secret"]?.ToString();
                    }
                }
                else
                {
                    subscription.CurrentPeriodStart = (DateTimeOffset)stripeSubscription.StartDate;
                    subscription.CurrentPeriodEnd = DateTimeOffset.UtcNow.AddSeconds(subscription.Amount);
                }
            }
            catch (Exception ex)
            {
                this.loggingBroker.LogWarning($"Subscription property access failed: {ex.Message}. Falling back to defaults.");
                subscription.CurrentPeriodStart = DateTimeOffset.UtcNow;
                subscription.CurrentPeriodEnd = DateTimeOffset.UtcNow.AddSeconds(subscription.Amount);
            }

            return subscription;
        }

        private async Task<string> GetOrCreatePriceAsync(
            string productName, long amount, string currency, int intervalCount)
        {
            var productOptions =
                new ProductListOptions
                {
                    Active = true
                };

            StripeList<Product> products =
                await new ProductService(this.client).ListAsync(productOptions);

            Product product =
                products.FirstOrDefault(p => p.Name == productName) ??
                await new ProductService(this.client).CreateAsync(new ProductCreateOptions { Name = productName });

            var priceOptions = new PriceListOptions
            {
                Product = product.Id,
                Active = true
            };
            StripeList<Price> prices = await new PriceService(this.client).ListAsync(priceOptions);

            Price price = prices.FirstOrDefault(p =>
                p.UnitAmount == amount &&
                p.Currency == currency.ToLower() &&
                ((intervalCount > 0 && p.Recurring != null && p.Recurring.Interval == "week" && p.Recurring.IntervalCount == intervalCount) || 
                 (intervalCount == 0 && p.Recurring == null)));

            if (price == null)
            {
                var priceCreateOptions = new PriceCreateOptions
                {
                    Product = product.Id,
                    UnitAmount = amount,
                    Currency = currency.ToLower()
                };

                if (intervalCount > 0)
                {
                    priceCreateOptions.Recurring = new PriceRecurringOptions
                    {
                        Interval = "week",
                        IntervalCount = intervalCount
                    };
                }

                price = await new PriceService(this.client).CreateAsync(priceCreateOptions);
            }

            return price.Id;
        }

        public async ValueTask<LocalSubscription> CancelSubscriptionAsync(LocalSubscription subscription)
        {
            var options = new SubscriptionUpdateOptions { CancelAtPeriodEnd = true };

            Stripe.Subscription stripeSubscription = await new SubscriptionService(this.client).UpdateAsync(
                subscription.StripeSubscriptionId, options);

            subscription.CancelAtPeriodEnd = stripeSubscription.CancelAtPeriodEnd;
            subscription.Status = MapToSubscriptionStatus(stripeSubscription.Status);
            return subscription;
        }

        public async ValueTask UpdateSubscriptionCollectionMethodAsync(string stripeSubscriptionId, string collectionMethod)
        {
            var options = new SubscriptionUpdateOptions
            {
                CollectionMethod = collectionMethod
            };

            await new SubscriptionService(this.client).UpdateAsync(stripeSubscriptionId, options);
        }

        public async ValueTask<PaymentLifecycleEvent> CreateOneTimeInvoiceAsync(string stripeCustomerId, long amount, string currency)
        {
            var itemOptions = new InvoiceItemCreateOptions
            {
                Customer = stripeCustomerId,
                Amount = amount,
                Currency = currency.ToLower(),
                Description = "One-time Payment (Manual Invoicing)"
            };

            await new InvoiceItemService(this.client).CreateAsync(itemOptions);

            var invoiceOptions = new InvoiceCreateOptions
            {
                Customer = stripeCustomerId,
                CollectionMethod = "send_invoice",
                AutoAdvance = true,
                DaysUntilDue = 7
            };

            Stripe.Invoice invoice = await new InvoiceService(this.client).CreateAsync(invoiceOptions);

            // Finalize the invoice to ensure it is sent/viewable
            invoice = await new InvoiceService(this.client).FinalizeInvoiceAsync(invoice.Id);

            return new PaymentLifecycleEvent
            {
                IsValid = true,
                EventType = "invoice.created",
                ExternalReferenceId = invoice.Id,
                Amount = invoice.AmountDue,
                Currency = invoice.Currency,
                OccurredAt = DateTimeOffset.UtcNow
            };
        }

        public async ValueTask<PaymentLifecycleEvent> CreateQuoteAsync(string customerId, long amount, string currency, string serviceName, bool isSubscription)
        {
            var options = new QuoteCreateOptions
            {
                Customer = customerId,
                LineItems = new List<QuoteLineItemOptions>(),
                CollectionMethod = "charge_automatically",
                Header = $"Approval Request for {serviceName}",
                Footer = "Thank you for choosing Emerald Kilonova."
            };

            // Use our helper to get or create a price. 0 interval means one-time.
            string priceId = await GetOrCreatePriceAsync(
                productName: isSubscription ? serviceName : $"{serviceName} (One-time)",
                amount: amount,
                currency: currency,
                intervalCount: isSubscription ? 1 : 0);

            options.LineItems.Add(new QuoteLineItemOptions { Price = priceId });

            Stripe.Quote stripeQuote = await new QuoteService(this.client).CreateAsync(options);
            
            // Finalize the quote to generate the HostedQuoteUrl
            stripeQuote = await new QuoteService(this.client).FinalizeQuoteAsync(stripeQuote.Id);

            // Extract HostedQuoteUrl from RawJObject because it's missing from the type definition in v51
            string hostedQuoteUrl = stripeQuote.RawJObject?["hosted_quote_url"]?.ToString();

            return new PaymentLifecycleEvent
            {
                IsValid = true,
                EventType = "quote.created",
                ExternalReferenceId = stripeQuote.Id,
                Amount = stripeQuote.AmountTotal,
                Currency = stripeQuote.Currency,
                CheckoutUrl = hostedQuoteUrl,
                OccurredAt = DateTimeOffset.UtcNow
            };
        }

        public async ValueTask<LocalPaymentIntent> CreatePaymentIntentAsync(LocalPaymentIntent paymentIntent)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = paymentIntent.Amount,
                Currency = paymentIntent.Currency,
                Customer = paymentIntent.StripeCustomerId,
                PaymentMethod = paymentIntent.StripePaymentMethodId,
                OffSession = true,
                Confirm = true
            };

            Stripe.PaymentIntent stripePaymentIntent =
                await new PaymentIntentService(this.client).CreateAsync(options);

            paymentIntent.StripePaymentIntentId = stripePaymentIntent.Id;
            paymentIntent.Status = MapToPaymentIntentStatus(stripePaymentIntent.Status);

            return paymentIntent;
        }

        public async ValueTask<string> RefundPaymentAsync(string stripeReferenceId)
        {
            string targetPaymentIntentId = stripeReferenceId.StartsWith("pi_") ? stripeReferenceId : null;
            string targetChargeId = stripeReferenceId.StartsWith("ch_") || stripeReferenceId.StartsWith("py_") ? stripeReferenceId : null;

            if (stripeReferenceId.StartsWith("in_"))
            {
                var getOptions = new Stripe.InvoiceGetOptions
                {
                    Expand = new System.Collections.Generic.List<string> { "payments" }
                };
                var invoice = await new Stripe.InvoiceService(this.client).GetAsync(stripeReferenceId, getOptions);
                var firstPayment = invoice.Payments?.Data?.FirstOrDefault();
                targetPaymentIntentId = firstPayment?.Payment?.PaymentIntentId;
                targetChargeId = firstPayment?.Payment?.ChargeId;
            }

            var options = new RefundCreateOptions
            {
                PaymentIntent = targetPaymentIntentId,
                Charge = targetChargeId
            };

            Refund refund = await new RefundService(this.client).CreateAsync(options);
            return refund.Status;
        }

        public PaymentLifecycleEvent ParseAndVerifyWebhookEvent(string jsonPayload, string stripeSignatureHeader)
        {
            try
            {
                Event stripeEvent = EventUtility.ConstructEvent(
                    jsonPayload,
                    stripeSignatureHeader,
                    this.webhookSecret);

                var rawEvent = Newtonsoft.Json.Linq.JObject.Parse(jsonPayload);
                string rawChargeInvoiceId = rawEvent["data"]?["object"]?["invoice"]?.ToString();

                return stripeEvent.Data.Object switch
                {
                    Invoice invoice => ExtractFromInvoice(stripeEvent.Type, invoice),
                    Stripe.PaymentIntent intent => ExtractFromPaymentIntent(stripeEvent.Type, intent),
                    Stripe.Subscription sub => ExtractFromSubscription(stripeEvent.Type, sub),
                    Charge charge => ExtractFromCharge(stripeEvent.Type, charge, rawChargeInvoiceId),
                    Refund refund => ExtractFromRefund(stripeEvent.Type, refund),
                    _ => new PaymentLifecycleEvent { IsValid = false, EventType = stripeEvent.Type }
                };
            }
            catch (StripeException stripeEx)
            {
                this.loggingBroker.LogWarning($"Webhook signature verification failed: {stripeEx.Message}");
                return new PaymentLifecycleEvent { IsValid = false };
            }
        }

        private static PaymentLifecycleEvent ExtractFromInvoice(string eventType, Stripe.Invoice invoice)
        {
            string subscriptionId = invoice.Parent?.SubscriptionDetails?.SubscriptionId
                ?? invoice.Parent?.SubscriptionDetails?.Subscription?.Id;

            return new PaymentLifecycleEvent
            {
                IsValid = true,
                EventType = eventType,
                ExternalReferenceId = invoice.Id,
                SubscriptionId = subscriptionId,
                Amount = invoice.AmountPaid,
                Currency = invoice.Currency,
                OccurredAt = DateTimeOffset.UtcNow
            };
        }

        private static PaymentLifecycleEvent ExtractFromPaymentIntent(string eventType, Stripe.PaymentIntent intent) =>
            new PaymentLifecycleEvent
            {
                IsValid = true,
                EventType = eventType,
                ExternalReferenceId = intent.Id,
                Amount = intent.Amount,
                Currency = intent.Currency,
                OccurredAt = DateTimeOffset.UtcNow
            };

        private static PaymentLifecycleEvent ExtractFromSubscription(string eventType, Stripe.Subscription sub) =>
            new PaymentLifecycleEvent
            {
                IsValid = true,
                EventType = eventType,
                ExternalReferenceId = sub.Id,
                SubscriptionId = sub.Id,
                OccurredAt = DateTimeOffset.UtcNow
            };

        private static PaymentLifecycleEvent ExtractFromCharge(string eventType, Charge charge, string fallbackInvoiceId) =>
            new PaymentLifecycleEvent
            {
                IsValid = true,
                EventType = eventType,
                ExternalReferenceId = fallbackInvoiceId ?? charge.PaymentIntentId,
                Amount = charge.AmountRefunded,
                Currency = charge.Currency,
                OccurredAt = DateTimeOffset.UtcNow
            };

        private static PaymentLifecycleEvent ExtractFromRefund(string eventType, Refund refund) =>
            new PaymentLifecycleEvent
            {
                IsValid = true,
                EventType = eventType,
                ExternalReferenceId = refund.PaymentIntentId ?? refund.ChargeId,
                Amount = refund.Amount,
                Currency = refund.Currency,
                OccurredAt = DateTimeOffset.UtcNow
            };

        private static LocalSubscriptionStatus MapToSubscriptionStatus(string status) =>
            status switch
            {
                "active" => LocalSubscriptionStatus.Active,
                "incomplete" => LocalSubscriptionStatus.Incomplete,
                "past_due" => LocalSubscriptionStatus.PastDue,
                "canceled" => LocalSubscriptionStatus.Canceled,
                "trialing" => LocalSubscriptionStatus.Trialing,
                _ => LocalSubscriptionStatus.Incomplete
            };

        private static LocalPaymentIntentStatus MapToPaymentIntentStatus(string status) =>
            status switch
            {
                "succeeded" => LocalPaymentIntentStatus.Succeeded,
                "requires_action" => LocalPaymentIntentStatus.RequiresAction,
                _ => LocalPaymentIntentStatus.Failed
            };
    }
}
