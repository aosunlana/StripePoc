using StripePoc.Api.Brokers.DateTimes;
using StripePoc.Api.Brokers.Loggings;
using StripePoc.Api.Models.PaymentIntents;
using StripePoc.Api.Models.Payments;
using StripePoc.Api.Models.Subscriptions;
using StripePoc.Api.Services.Foundations.PaymentIntents;
using StripePoc.Api.Services.Foundations.Payments;
using StripePoc.Api.Services.Foundations.StripePayments;
using StripePoc.Api.Services.Foundations.Subscriptions;
using StripePoc.Api.Models.Events;

namespace StripePoc.Api.Services.Orchestrations.Webhooks
{
    public interface IWebhookOrchestrationService
    {
        ValueTask HandleStripeEventAsync(string jsonPayload, string stripeSignatureHeader);
    }

    public partial class WebhookOrchestrationService : IWebhookOrchestrationService
    {
        private readonly IStripePaymentService stripePaymentService;
        private readonly ISubscriptionService subscriptionService;
        private readonly IPaymentService paymentService;
        private readonly IPaymentIntentService paymentIntentService;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public WebhookOrchestrationService(
            IStripePaymentService stripePaymentService,
            ISubscriptionService subscriptionService,
            IPaymentService paymentService,
            IPaymentIntentService paymentIntentService,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.stripePaymentService = stripePaymentService;
            this.subscriptionService = subscriptionService;
            this.paymentService = paymentService;
            this.paymentIntentService = paymentIntentService;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public async ValueTask HandleStripeEventAsync(string jsonPayload, string stripeSignatureHeader)
        {
            PaymentLifecycleEvent paymentLifecycleEvent =
                this.stripePaymentService.ParseAndVerifyWebhookEvent(jsonPayload, stripeSignatureHeader);

            if (!paymentLifecycleEvent.IsValid)
            {
                this.loggingBroker.LogWarning("Received invalid Stripe webhook — ignoring.");
                return;
            }

            switch (paymentLifecycleEvent.EventType)
            {
                case "invoice.paid":
                    await HandleInvoicePaidAsync(
                        paymentLifecycleEvent.SubscriptionId,
                        paymentLifecycleEvent.ExternalReferenceId,
                        paymentLifecycleEvent.Amount,
                        paymentLifecycleEvent.Currency,
                        paymentLifecycleEvent.OccurredAt);
                    break;

                case "invoice.payment_failed":
                    await HandleInvoicePaymentFailedAsync(
                        paymentLifecycleEvent.SubscriptionId,
                        paymentLifecycleEvent.ExternalReferenceId,
                        paymentLifecycleEvent.Currency);
                    break;

                case "payment_intent.succeeded":
                    await HandlePaymentIntentSucceededAsync(
                        paymentLifecycleEvent.ExternalReferenceId,
                        paymentLifecycleEvent.Amount,
                        paymentLifecycleEvent.Currency,
                        paymentLifecycleEvent.OccurredAt);
                    break;

                case "payment_intent.payment_failed":
                    await HandlePaymentIntentFailedAsync(
                        paymentLifecycleEvent.ExternalReferenceId,
                        paymentLifecycleEvent.Amount,
                        paymentLifecycleEvent.Currency);
                    break;

                case "customer.subscription.deleted":
                    await HandleSubscriptionDeletedAsync(paymentLifecycleEvent.SubscriptionId);
                    break;

                case "charge.refunded":
                    await HandleChargeRefundedAsync(
                        paymentLifecycleEvent.ExternalReferenceId,
                        paymentLifecycleEvent.Amount,
                        paymentLifecycleEvent.Currency,
                        paymentLifecycleEvent.OccurredAt);
                    break;

                default:
                    this.loggingBroker.LogInformation($"Unhandled Stripe event: {paymentLifecycleEvent.EventType}");
                    break;
            }
        }

        private async ValueTask HandleInvoicePaidAsync(
            string stripeSubscriptionId, string stripeInvoiceId, long amount, string currency, DateTimeOffset paidAt)
        {
            Payment existingPayment = await this.paymentService.RetrievePaymentByStripeReferenceIdAsync(stripeInvoiceId);
            if (existingPayment is not null) return;

            if (string.IsNullOrWhiteSpace(stripeSubscriptionId))
            {
                // Handle One-Time Manual Invoice
                PaymentIntent paymentIntent = await this.paymentIntentService.RetrievePaymentIntentByStripeInvoiceIdAsync(stripeInvoiceId);
                if (paymentIntent is null) return;

                var oneTimePayment = new Payment
                {
                    AccountId = paymentIntent.AccountId,
                    StripeReferenceId = stripeInvoiceId,
                    Type = PaymentType.OneTime,
                    Amount = amount,
                    Currency = currency,
                    Status = PaymentStatus.Succeeded,
                    PaidAt = paidAt
                };

                await this.paymentService.AddPaymentAsync(oneTimePayment);
                paymentIntent.Status = PaymentIntentStatus.Succeeded;
                await this.paymentIntentService.ModifyPaymentIntentAsync(paymentIntent);
                return;
            }

            // Handle Subscription Invoice
            Subscription subscription = await this.subscriptionService.RetrieveSubscriptionByStripeSubscriptionIdAsync(stripeSubscriptionId);
            if (subscription is null) return;

            var payment = new Payment
            {
                AccountId = subscription.AccountId,
                StripeReferenceId = stripeInvoiceId,
                Type = PaymentType.Recurring,
                Amount = amount,
                Currency = currency,
                Status = PaymentStatus.Succeeded,
                PaidAt = paidAt
            };

            await this.paymentService.AddPaymentAsync(payment);
            subscription.Status = SubscriptionStatus.Active;
            await this.subscriptionService.ModifySubscriptionAsync(subscription);

            // Transition to Auto-Pay for future invoices
            this.loggingBroker.LogInformation($"Transitioning subscription {stripeSubscriptionId} to automatic billing.");
            await this.stripePaymentService.UpdateSubscriptionCollectionMethodAsync(stripeSubscriptionId, "charge_automatically");
        }

        private async ValueTask HandleInvoicePaymentFailedAsync(string stripeSubscriptionId, string stripeInvoiceId, string currency)
        {
            if (string.IsNullOrWhiteSpace(stripeSubscriptionId)) return;
            Payment existingPayment = await this.paymentService.RetrievePaymentByStripeReferenceIdAsync(stripeInvoiceId);
            if (existingPayment is not null) return;

            Subscription subscription = await this.subscriptionService.RetrieveSubscriptionByStripeSubscriptionIdAsync(stripeSubscriptionId);
            if (subscription is null) return;

            var payment = new Payment
            {
                AccountId = subscription.AccountId,
                StripeReferenceId = stripeInvoiceId,
                Type = PaymentType.Recurring,
                Amount = 0,
                Currency = currency,
                Status = PaymentStatus.Failed,
                PaidAt = this.dateTimeBroker.GetCurrentDateTimeOffset()
            };

            await this.paymentService.AddPaymentAsync(payment);
            subscription.Status = SubscriptionStatus.PastDue;
            await this.subscriptionService.ModifySubscriptionAsync(subscription);
        }

        private async ValueTask HandlePaymentIntentSucceededAsync(string stripePaymentIntentId, long amount, string currency, DateTimeOffset occurredAt)
        {
            Payment existingPayment = await this.paymentService.RetrievePaymentByStripeReferenceIdAsync(stripePaymentIntentId);
            if (existingPayment is not null) return;

            PaymentIntent paymentIntent = await this.paymentIntentService.RetrievePaymentIntentByStripePaymentIntentIdAsync(stripePaymentIntentId);
            if (paymentIntent is null) return;

            var payment = new Payment
            {
                AccountId = paymentIntent.AccountId,
                StripeReferenceId = stripePaymentIntentId,
                Type = PaymentType.OneTime,
                Amount = amount,
                Currency = currency,
                Status = PaymentStatus.Succeeded,
                PaidAt = occurredAt
            };

            await this.paymentService.AddPaymentAsync(payment);
            paymentIntent.Status = PaymentIntentStatus.Succeeded;
            await this.paymentIntentService.ModifyPaymentIntentAsync(paymentIntent);
        }

        private async ValueTask HandlePaymentIntentFailedAsync(string stripePaymentIntentId, long amount, string currency)
        {
            Payment existingPayment = await this.paymentService.RetrievePaymentByStripeReferenceIdAsync(stripePaymentIntentId);
            if (existingPayment is not null) return;

            PaymentIntent paymentIntent = await this.paymentIntentService.RetrievePaymentIntentByStripePaymentIntentIdAsync(stripePaymentIntentId);
            if (paymentIntent is null) return;

            var payment = new Payment
            {
                AccountId = paymentIntent.AccountId,
                StripeReferenceId = stripePaymentIntentId,
                Type = PaymentType.OneTime,
                Amount = amount,
                Currency = currency,
                Status = PaymentStatus.Failed,
                PaidAt = this.dateTimeBroker.GetCurrentDateTimeOffset()
            };

            await this.paymentService.AddPaymentAsync(payment);
            paymentIntent.Status = PaymentIntentStatus.Failed;
            await this.paymentIntentService.ModifyPaymentIntentAsync(paymentIntent);
        }

        private async ValueTask HandleSubscriptionDeletedAsync(string stripeSubscriptionId)
        {
            Subscription subscription = await this.subscriptionService.RetrieveSubscriptionByStripeSubscriptionIdAsync(stripeSubscriptionId);
            if (subscription is null) return;

            subscription.Status = SubscriptionStatus.Canceled;
            await this.subscriptionService.ModifySubscriptionAsync(subscription);
        }

        private async ValueTask HandleChargeRefundedAsync(string stripeReferenceId, long amountRefunded, string currency, DateTimeOffset occurredAt)
        {
            this.loggingBroker.LogInformation($"Refund received for reference {stripeReferenceId}. Amount: {amountRefunded}");

            Payment existingPayment = await this.paymentService.RetrievePaymentByStripeReferenceIdAsync(stripeReferenceId);
            if (existingPayment is null) return;

            existingPayment.Status = PaymentStatus.Refunded;
            existingPayment.Amount = existingPayment.Amount - amountRefunded;
            await this.paymentService.ModifyPaymentAsync(existingPayment);
        }
    }
}
