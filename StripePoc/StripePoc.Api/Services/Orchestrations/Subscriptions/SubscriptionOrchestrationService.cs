using StripePoc.Api.Models.Events;
using StripePoc.Api.Brokers.Loggings;
using StripePoc.Api.Models.PaymentAccounts;
using StripePoc.Api.Models.PaymentMethods;
using StripePoc.Api.Models.Subscriptions;
using StripePoc.Api.Services.Foundations.PaymentAccounts;
using StripePoc.Api.Services.Foundations.PaymentMethods;
using StripePoc.Api.Services.Foundations.StripePayments;
using StripePoc.Api.Services.Foundations.Subscriptions;

namespace StripePoc.Api.Services.Orchestrations.Subscriptions
{
    public interface ISubscriptionOrchestrationService
    {
        ValueTask<Subscription> AddSubscriptionAsync(Subscription subscription);
        ValueTask<Subscription> CancelSubscriptionAsync(Guid subscriptionId);
        ValueTask<IQueryable<Subscription>> RetrieveAllSubscriptionsByAccountIdAsync(Guid accountId);
        ValueTask<Subscription> RetrieveSubscriptionByIdAsync(Guid subscriptionId);
        ValueTask<PaymentLifecycleEvent> ProcessSubscriptionQuoteAsync(Subscription subscription);
    }

    public partial class SubscriptionOrchestrationService : ISubscriptionOrchestrationService
    {
        private readonly IPaymentAccountService paymentAccountService;
        private readonly IPaymentMethodService paymentMethodService;
        private readonly IStripePaymentService stripePaymentService;
        private readonly ISubscriptionService subscriptionService;
        private readonly ILoggingBroker loggingBroker;

        public SubscriptionOrchestrationService(
            IPaymentAccountService paymentAccountService,
            IPaymentMethodService paymentMethodService,
            IStripePaymentService stripePaymentService,
            ISubscriptionService subscriptionService,
            ILoggingBroker loggingBroker)
        {
            this.paymentAccountService = paymentAccountService;
            this.paymentMethodService = paymentMethodService;
            this.stripePaymentService = stripePaymentService;
            this.subscriptionService = subscriptionService;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<Subscription> AddSubscriptionAsync(Subscription subscription)
        {
            PaymentAccount paymentAccount =
                await this.paymentAccountService.RetrievePaymentAccountByAccountIdAsync(
                    subscription.AccountId);

            if (paymentAccount is null)
                throw new Exception("Account setup incomplete. Please add a payment method in the Wallet step (Step 2).");

            PaymentMethod defaultPaymentMethod =
                await this.paymentMethodService.RetrieveDefaultPaymentMethodByPaymentAccountIdAsync(
                    paymentAccount.Id);

            if (defaultPaymentMethod is null)
                throw new Exception("No default payment method found. Please save a card in the Wallet step (Step 2).");

            Subscription stripeSubscription =
                await this.stripePaymentService.CreateSubscriptionAsync(
                    subscription,
                    paymentAccount.StripeCustomerId,
                    defaultPaymentMethod.StripePaymentMethodId);

            return await this.subscriptionService.AddSubscriptionAsync(stripeSubscription);
        }

        public async ValueTask<Subscription> CancelSubscriptionAsync(Guid subscriptionId)
        {
            Subscription existingSubscription = await this.subscriptionService.RetrieveSubscriptionByIdAsync(subscriptionId);
            Subscription cancelledSubscription = await this.stripePaymentService.CancelSubscriptionAsync(existingSubscription);

            return await this.subscriptionService.ModifySubscriptionAsync(cancelledSubscription);
        }

        public async ValueTask<IQueryable<Subscription>> RetrieveAllSubscriptionsByAccountIdAsync(Guid accountId) =>
            await this.subscriptionService.RetrieveAllSubscriptionsByAccountIdAsync(accountId);

        public async ValueTask<Subscription> RetrieveSubscriptionByIdAsync(Guid subscriptionId) =>
            await this.subscriptionService.RetrieveSubscriptionByIdAsync(subscriptionId);

        public async ValueTask<PaymentLifecycleEvent> ProcessSubscriptionQuoteAsync(Subscription subscription)
        {
            PaymentAccount paymentAccount =
                await this.paymentAccountService.RetrievePaymentAccountByAccountIdAsync(
                    subscription.AccountId);

            if (paymentAccount is null)
                throw new Exception("Account setup incomplete.");

            PaymentMethod defaultPaymentMethod =
                await this.paymentMethodService.RetrieveDefaultPaymentMethodByPaymentAccountIdAsync(
                    paymentAccount.Id);

            if (defaultPaymentMethod is null)
                throw new Exception("No default payment method found.");

            var quoteEvent = await this.stripePaymentService.CreateQuoteAsync(
                paymentAccount.StripeCustomerId,
                subscription.Amount,
                subscription.Currency,
                subscription.ServiceName,
                isSubscription: true);

            subscription.StripeSubscriptionId = quoteEvent.ExternalReferenceId; // Store Quote ID temporarily
            subscription.Status = SubscriptionStatus.Incomplete;
            await this.subscriptionService.AddSubscriptionAsync(subscription);

            return quoteEvent;
        }
    }
}
