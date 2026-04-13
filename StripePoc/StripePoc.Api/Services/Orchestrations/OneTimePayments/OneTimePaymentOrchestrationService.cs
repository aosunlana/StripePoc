using System.Linq;
using StripePoc.Api.Models.Events;
using StripePoc.Api.Brokers.Loggings;
using StripePoc.Api.Models.PaymentAccounts;
using StripePoc.Api.Models.PaymentIntents;
using StripePoc.Api.Models.PaymentMethods;
using StripePoc.Api.Services.Foundations.PaymentAccounts;
using StripePoc.Api.Services.Foundations.PaymentIntents;
using StripePoc.Api.Services.Foundations.PaymentMethods;
using StripePoc.Api.Services.Foundations.Payments;
using StripePoc.Api.Services.Foundations.StripePayments;

namespace StripePoc.Api.Services.Orchestrations.OneTimePayments
{
    public interface IOneTimePaymentOrchestrationService
    {
        ValueTask<PaymentIntent> ProcessOneTimePaymentAsync(PaymentIntent paymentIntent);
        ValueTask<PaymentLifecycleEvent> ProcessOneTimeQuotePaymentAsync(PaymentIntent paymentIntent);
        ValueTask<string> RefundOneTimePaymentAsync(string stripePaymentIntentId);
        ValueTask<string> RefundLastPaymentAsync(Guid accountId);
    }

    public partial class OneTimePaymentOrchestrationService : IOneTimePaymentOrchestrationService
    {
        private readonly IPaymentAccountService paymentAccountService;
        private readonly IPaymentMethodService paymentMethodService;
        private readonly IStripePaymentService stripePaymentService;
        private readonly IPaymentIntentService paymentIntentService;
        private readonly IPaymentService paymentService;
        private readonly ILoggingBroker loggingBroker;

        public OneTimePaymentOrchestrationService(
            IPaymentAccountService paymentAccountService,
            IPaymentMethodService paymentMethodService,
            IStripePaymentService stripePaymentService,
            IPaymentIntentService paymentIntentService,
            IPaymentService paymentService,
            ILoggingBroker loggingBroker)
        {
            this.paymentAccountService = paymentAccountService;
            this.paymentMethodService = paymentMethodService;
            this.stripePaymentService = stripePaymentService;
            this.paymentIntentService = paymentIntentService;
            this.paymentService = paymentService;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<PaymentIntent> ProcessOneTimePaymentAsync(PaymentIntent paymentIntent)
        {
            PaymentAccount paymentAccount =
                await this.paymentAccountService.RetrievePaymentAccountByAccountIdAsync(
                    paymentIntent.AccountId);

            if (paymentAccount is null)
                throw new Exception("Account setup incomplete. Please add a payment method in the Wallet step (Step 2).");

            PaymentMethod defaultPaymentMethod =
                await this.paymentMethodService.RetrieveDefaultPaymentMethodByPaymentAccountIdAsync(
                    paymentAccount.Id);

            if (defaultPaymentMethod is null)
                throw new Exception("No default payment method found. Please save a card in the Wallet step (Step 2).");

            paymentIntent.StripeCustomerId = paymentAccount.StripeCustomerId;
            paymentIntent.StripePaymentMethodId = defaultPaymentMethod.StripePaymentMethodId;

            var paymentEvent = await this.stripePaymentService.CreateOneTimeInvoiceAsync(
                paymentIntent.StripeCustomerId,
                paymentIntent.Amount,
                paymentIntent.Currency);

            paymentIntent.StripeInvoiceId = paymentEvent.ExternalReferenceId;
            paymentIntent.Status = PaymentIntentStatus.RequiresAction;

            return await this.paymentIntentService.AddPaymentIntentAsync(paymentIntent);
        }

        public async ValueTask<PaymentLifecycleEvent> ProcessOneTimeQuotePaymentAsync(PaymentIntent paymentIntent)
        {
            PaymentAccount paymentAccount =
                await this.paymentAccountService.RetrievePaymentAccountByAccountIdAsync(
                    paymentIntent.AccountId);

            if (paymentAccount is null)
                throw new Exception("Account setup incomplete.");

            PaymentMethod defaultPaymentMethod =
                await this.paymentMethodService.RetrieveDefaultPaymentMethodByPaymentAccountIdAsync(
                    paymentAccount.Id);

            if (defaultPaymentMethod is null)
                throw new Exception("No default payment method found.");

            var quoteEvent = await this.stripePaymentService.CreateQuoteAsync(
                paymentAccount.StripeCustomerId,
                paymentIntent.Amount,
                paymentIntent.Currency,
                "One-time Service",
                isSubscription: false);

            paymentIntent.StripeCustomerId = paymentAccount.StripeCustomerId;
            paymentIntent.StripeInvoiceId = quoteEvent.ExternalReferenceId; // We store Quote ID in the Invoice field for now
            paymentIntent.Status = PaymentIntentStatus.RequiresAction;
            await this.paymentIntentService.AddPaymentIntentAsync(paymentIntent);

            return quoteEvent;
        }

        public async ValueTask<string> RefundOneTimePaymentAsync(string stripePaymentIntentId) =>
            await this.stripePaymentService.RefundPaymentAsync(stripePaymentIntentId);

        public async ValueTask<string> RefundLastPaymentAsync(Guid accountId)
        {
            var payments = await this.paymentService.RetrieveAllPaymentsByAccountIdAsync(accountId);
            var lastPayment = payments
                .OrderByDescending(p => p.PaidAt)
                .FirstOrDefault();

            if (lastPayment == null)
            {
                throw new Exception("No payments found for this account to refund.");
            }

            return await this.stripePaymentService.RefundPaymentAsync(lastPayment.StripeReferenceId);
        }
    }
}
