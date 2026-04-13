using StripePoc.Api.Brokers.Loggings;
using StripePoc.Api.Models.PaymentAccounts;
using StripePoc.Api.Models.PaymentMethods;
using StripePoc.Api.Services.Foundations.PaymentAccounts;
using StripePoc.Api.Services.Foundations.PaymentMethods;
using StripePoc.Api.Services.Foundations.StripePayments;

namespace StripePoc.Api.Services.Orchestrations.AccountSetups
{
    public interface IAccountSetupOrchestrationService
    {
        ValueTask<PaymentAccount> ProvidePaymentAccountAsync(Guid accountId);
        ValueTask<string> InitiatePaymentMethodSetupAsync(Guid accountId);
        ValueTask<PaymentMethod> FinalisePaymentMethodSetupAsync(Guid accountId, string stripePaymentMethodId);
        ValueTask<IQueryable<PaymentMethod>> RetrieveAllPaymentMethodsByAccountIdAsync(Guid accountId);
        ValueTask<PaymentMethod> SetDefaultPaymentMethodAsync(Guid accountId, Guid paymentMethodId);
        ValueTask<PaymentMethod> RemovePaymentMethodAsync(Guid accountId, Guid paymentMethodId);
    }

    public partial class AccountSetupOrchestrationService : IAccountSetupOrchestrationService
    {
        private readonly IPaymentAccountService paymentAccountService;
        private readonly IStripePaymentService stripePaymentService;
        private readonly IPaymentMethodService paymentMethodService;
        private readonly ILoggingBroker loggingBroker;

        public AccountSetupOrchestrationService(
            IPaymentAccountService paymentAccountService,
            IStripePaymentService stripePaymentService,
            IPaymentMethodService paymentMethodService,
            ILoggingBroker loggingBroker)
        {
            this.paymentAccountService = paymentAccountService;
            this.stripePaymentService = stripePaymentService;
            this.paymentMethodService = paymentMethodService;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<PaymentAccount> ProvidePaymentAccountAsync(Guid accountId)
        {
            PaymentAccount existingAccount =
                await this.paymentAccountService.RetrievePaymentAccountByAccountIdAsync(
                    accountId);

            if (existingAccount is not null)
                return existingAccount;

            var paymentAccount =
                new PaymentAccount
                {
                    AccountId = accountId
                };

            PaymentAccount paymentAccountWithCustomer =
                await this.stripePaymentService.CreateCustomerAsync(paymentAccount);

            return await this.paymentAccountService.AddPaymentAccountAsync(
                paymentAccountWithCustomer);
        }

        public async ValueTask<string> InitiatePaymentMethodSetupAsync(Guid accountId)
        {
            PaymentAccount paymentAccount =
                await this.paymentAccountService.RetrievePaymentAccountByAccountIdAsync(accountId);

            return await this.stripePaymentService.CreateSetupIntentAsync(paymentAccount.StripeCustomerId);
        }

        public async ValueTask<PaymentMethod> FinalisePaymentMethodSetupAsync(
            Guid accountId,
            string stripePaymentMethodId)
        {
            PaymentAccount paymentAccount =
                await this.paymentAccountService.RetrievePaymentAccountByAccountIdAsync(accountId);

            // Retrieve card details directly from Stripe using the payment method ID
            PaymentMethod retrievedPaymentMethod =
                await this.stripePaymentService.RetrievePaymentMethodAsync(
                new PaymentMethod
                {
                    PaymentAccountId = paymentAccount.Id,
                    StripePaymentMethodId = stripePaymentMethodId
                });

            PaymentMethod existingDefault =
                await this.paymentMethodService.RetrieveDefaultPaymentMethodByPaymentAccountIdAsync(
                    paymentAccount.Id);

            if (existingDefault is not null)
            {
                existingDefault.IsDefault = false;
                await this.paymentMethodService.ModifyPaymentMethodAsync(existingDefault);
            }

            retrievedPaymentMethod.IsDefault = true;
            return await this.paymentMethodService.AddPaymentMethodAsync(retrievedPaymentMethod);
        }

        public async ValueTask<IQueryable<PaymentMethod>> RetrieveAllPaymentMethodsByAccountIdAsync(Guid accountId)
        {
            PaymentAccount paymentAccount = await this.paymentAccountService.RetrievePaymentAccountByAccountIdAsync(accountId);
            return await this.paymentMethodService.RetrieveAllPaymentMethodsByPaymentAccountIdAsync(paymentAccount.Id);
        }

        public async ValueTask<PaymentMethod> SetDefaultPaymentMethodAsync(Guid accountId, Guid paymentMethodId)
        {
            PaymentAccount paymentAccount = await this.paymentAccountService.RetrievePaymentAccountByAccountIdAsync(accountId);

            PaymentMethod existingDefault = await this.paymentMethodService.RetrieveDefaultPaymentMethodByPaymentAccountIdAsync(paymentAccount.Id);
            if (existingDefault is not null && existingDefault.Id != paymentMethodId)
            {
                existingDefault.IsDefault = false;
                await this.paymentMethodService.ModifyPaymentMethodAsync(existingDefault);
            }

            PaymentMethod targetPaymentMethod = await this.paymentMethodService.RetrievePaymentMethodByIdAsync(paymentMethodId);
            if (targetPaymentMethod.PaymentAccountId != paymentAccount.Id) throw new UnauthorizedAccessException();

            targetPaymentMethod.IsDefault = true;
            return await this.paymentMethodService.ModifyPaymentMethodAsync(targetPaymentMethod);
        }

        public async ValueTask<PaymentMethod> RemovePaymentMethodAsync(Guid accountId, Guid paymentMethodId)
        {
            PaymentAccount paymentAccount = await this.paymentAccountService.RetrievePaymentAccountByAccountIdAsync(accountId);
            PaymentMethod paymentMethod = await this.paymentMethodService.RetrievePaymentMethodByIdAsync(paymentMethodId);

            if (paymentMethod.PaymentAccountId != paymentAccount.Id) throw new UnauthorizedAccessException();

            return await this.paymentMethodService.RemovePaymentMethodAsync(paymentMethodId);
        }
    }
}
