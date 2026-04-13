using System;
using System.Threading.Tasks;
using StripePoc.Api.Brokers.Loggings;
using StripePoc.Api.Brokers.Payments;
using StripePoc.Api.Models.PaymentAccounts;
using StripePoc.Api.Models.PaymentMethods;
using StripePoc.Api.Models.Subscriptions;
using StripePoc.Api.Models.PaymentIntents;
using StripePoc.Api.Models.Events;

namespace StripePoc.Api.Services.Foundations.StripePayments
{
    public interface IStripePaymentService
    {
        ValueTask<PaymentAccount> CreateCustomerAsync(PaymentAccount paymentAccount);
        ValueTask<string> CreateSetupIntentAsync(string stripeCustomerId);
        ValueTask<PaymentMethod> RetrievePaymentMethodAsync(PaymentMethod paymentMethod);

        ValueTask<Subscription> CreateSubscriptionAsync(
            Subscription subscription,
            string stripeCustomerId,
            string stripePaymentMethodId);

        ValueTask<Subscription> CancelSubscriptionAsync(Subscription subscription);
        ValueTask<PaymentIntent> CreatePaymentIntentAsync(PaymentIntent paymentIntent);
        ValueTask<string> RefundPaymentAsync(string stripeReferenceId);
        ValueTask UpdateSubscriptionCollectionMethodAsync(string stripeSubscriptionId, string collectionMethod);
        ValueTask<PaymentLifecycleEvent> CreateQuoteAsync(string customerId, long amount, string currency, string serviceName, bool isSubscription);
        ValueTask<PaymentLifecycleEvent> CreateOneTimeInvoiceAsync(string stripeCustomerId, long amount, string currency);

        PaymentLifecycleEvent ParseAndVerifyWebhookEvent(string jsonPayload, string stripeSignatureHeader);
    }

    public partial class StripePaymentService : IStripePaymentService
    {
        private readonly IPaymentBroker paymentBroker;
        private readonly ILoggingBroker loggingBroker;

        public StripePaymentService(
            IPaymentBroker paymentBroker,
            ILoggingBroker loggingBroker)
        {
            this.paymentBroker = paymentBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<PaymentAccount> CreateCustomerAsync(PaymentAccount paymentAccount) =>
            await this.paymentBroker.CreateCustomerAsync(paymentAccount);

        public async ValueTask<string> CreateSetupIntentAsync(string stripeCustomerId) =>
            await this.paymentBroker.CreateSetupIntentAsync(stripeCustomerId);

        public async ValueTask<PaymentMethod> RetrievePaymentMethodAsync(PaymentMethod paymentMethod) =>
            await this.paymentBroker.RetrievePaymentMethodAsync(paymentMethod);

        public async ValueTask<Subscription> CreateSubscriptionAsync(
            Subscription subscription,
            string stripeCustomerId,
            string stripePaymentMethodId) =>
            await this.paymentBroker.CreateSubscriptionAsync(
                subscription, stripeCustomerId, stripePaymentMethodId);

        public async ValueTask<Subscription> CancelSubscriptionAsync(Subscription subscription) =>
            await this.paymentBroker.CancelSubscriptionAsync(subscription);

        public async ValueTask<PaymentIntent> CreatePaymentIntentAsync(PaymentIntent paymentIntent) =>
            await this.paymentBroker.CreatePaymentIntentAsync(paymentIntent);

        public async ValueTask<string> RefundPaymentAsync(string stripeReferenceId) =>
            await this.paymentBroker.RefundPaymentAsync(stripeReferenceId);

        public async ValueTask UpdateSubscriptionCollectionMethodAsync(string stripeSubscriptionId, string collectionMethod) =>
            await this.paymentBroker.UpdateSubscriptionCollectionMethodAsync(stripeSubscriptionId, collectionMethod);

        public async ValueTask<PaymentLifecycleEvent> CreateQuoteAsync(string customerId, long amount, string currency, string serviceName, bool isSubscription) =>
            await this.paymentBroker.CreateQuoteAsync(customerId, amount, currency, serviceName, isSubscription);

        public async ValueTask<PaymentLifecycleEvent> CreateOneTimeInvoiceAsync(string stripeCustomerId, long amount, string currency) =>
            await this.paymentBroker.CreateOneTimeInvoiceAsync(stripeCustomerId, amount, currency);

        public PaymentLifecycleEvent ParseAndVerifyWebhookEvent(string jsonPayload, string stripeSignatureHeader) =>
            this.paymentBroker.ParseAndVerifyWebhookEvent(jsonPayload, stripeSignatureHeader);
    }
}
