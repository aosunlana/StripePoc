using System;
using System.Threading.Tasks;
using StripePoc.Api.Models.PaymentAccounts;
using StripePoc.Api.Models.PaymentMethods;
using StripePoc.Api.Models.Subscriptions;
using StripePoc.Api.Models.PaymentIntents;
using StripePoc.Api.Models.Events;

namespace StripePoc.Api.Brokers.Payments
{
    public interface IPaymentBroker
    {
        ValueTask<StripePoc.Api.Models.PaymentAccounts.PaymentAccount> CreateCustomerAsync(StripePoc.Api.Models.PaymentAccounts.PaymentAccount paymentAccount);
        ValueTask<string> CreateSetupIntentAsync(string stripeCustomerId);
        ValueTask<StripePoc.Api.Models.PaymentMethods.PaymentMethod> RetrievePaymentMethodAsync(StripePoc.Api.Models.PaymentMethods.PaymentMethod paymentMethod);

        ValueTask<StripePoc.Api.Models.Subscriptions.Subscription> CreateSubscriptionAsync(
            StripePoc.Api.Models.Subscriptions.Subscription subscription,
            string stripeCustomerId,
            string stripePaymentMethodId);

        ValueTask<StripePoc.Api.Models.Subscriptions.Subscription> CancelSubscriptionAsync(StripePoc.Api.Models.Subscriptions.Subscription subscription);
        ValueTask UpdateSubscriptionCollectionMethodAsync(string stripeSubscriptionId, string collectionMethod);
        ValueTask<PaymentLifecycleEvent> CreateQuoteAsync(string customerId, long amount, string currency, string serviceName, bool isSubscription);
        ValueTask<PaymentLifecycleEvent> CreateOneTimeInvoiceAsync(string stripeCustomerId, long amount, string currency);
        ValueTask<StripePoc.Api.Models.PaymentIntents.PaymentIntent> CreatePaymentIntentAsync(StripePoc.Api.Models.PaymentIntents.PaymentIntent paymentIntent);
        ValueTask<string> RefundPaymentAsync(string stripeReferenceId);

        PaymentLifecycleEvent ParseAndVerifyWebhookEvent(string jsonPayload, string stripeSignatureHeader);
    }
}
