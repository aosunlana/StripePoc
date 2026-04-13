using System;
using System.Threading.Tasks;
using StripePoc.Api.Brokers.DateTimes;
using StripePoc.Api.Brokers.Loggings;
using StripePoc.Api.Brokers.Storages;
using StripePoc.Api.Models.PaymentAccounts;

namespace StripePoc.Api.Services.Foundations.PaymentAccounts
{
    public interface IPaymentAccountService
    {
        ValueTask<PaymentAccount> AddPaymentAccountAsync(PaymentAccount paymentAccount);
        ValueTask<PaymentAccount> RetrievePaymentAccountByAccountIdAsync(Guid accountId);
        ValueTask<PaymentAccount> ModifyPaymentAccountAsync(PaymentAccount paymentAccount);
    }

    public partial class PaymentAccountService : IPaymentAccountService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public PaymentAccountService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public async ValueTask<PaymentAccount> AddPaymentAccountAsync(PaymentAccount paymentAccount)
        {
            if (paymentAccount is null) throw new ArgumentNullException(nameof(paymentAccount));
            if (paymentAccount.AccountId == Guid.Empty) throw new ArgumentException("AccountId is required.");

            paymentAccount.Id = Guid.NewGuid();
            paymentAccount.CreatedDate = this.dateTimeBroker.GetCurrentDateTimeOffset();
            paymentAccount.UpdatedDate = paymentAccount.CreatedDate;

            return await this.storageBroker.InsertPaymentAccountAsync(paymentAccount);
        }

        public async ValueTask<PaymentAccount> RetrievePaymentAccountByAccountIdAsync(Guid accountId)
        {
            if (accountId == Guid.Empty) throw new ArgumentException("AccountId is required.");

            return await this.storageBroker.SelectPaymentAccountByAccountIdAsync(accountId);
        }

        public async ValueTask<PaymentAccount> ModifyPaymentAccountAsync(PaymentAccount paymentAccount)
        {
            if (paymentAccount is null) throw new ArgumentNullException(nameof(paymentAccount));

            paymentAccount.UpdatedDate = this.dateTimeBroker.GetCurrentDateTimeOffset();

            return await this.storageBroker.UpdatePaymentAccountAsync(paymentAccount);
        }
    }
}
