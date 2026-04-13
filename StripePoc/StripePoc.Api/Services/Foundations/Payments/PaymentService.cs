using System;
using System.Linq;
using System.Threading.Tasks;
using StripePoc.Api.Brokers.DateTimes;
using StripePoc.Api.Brokers.Loggings;
using StripePoc.Api.Brokers.Storages;
using StripePoc.Api.Models.Payments;

namespace StripePoc.Api.Services.Foundations.Payments
{
    public interface IPaymentService
    {
        ValueTask<Payment> AddPaymentAsync(Payment payment);
        ValueTask<IQueryable<Payment>> RetrieveAllPaymentsByAccountIdAsync(Guid accountId);
        ValueTask<Payment> RetrievePaymentByStripeReferenceIdAsync(string stripeReferenceId);
        ValueTask<Payment> ModifyPaymentAsync(Payment payment);
    }

    public partial class PaymentService : IPaymentService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public PaymentService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public async ValueTask<Payment> AddPaymentAsync(Payment payment)
        {
            if (payment is null) throw new ArgumentNullException(nameof(payment));

            payment.Id = Guid.NewGuid();
            payment.CreatedDate = this.dateTimeBroker.GetCurrentDateTimeOffset();
            payment.UpdatedDate = payment.CreatedDate;

            return await this.storageBroker.InsertPaymentAsync(payment);
        }

        public async ValueTask<IQueryable<Payment>> RetrieveAllPaymentsByAccountIdAsync(Guid accountId)
        {
            if (accountId == Guid.Empty) throw new ArgumentException("AccountId is required.");

            return await this.storageBroker.SelectAllPaymentsByAccountIdAsync(accountId);
        }

        public async ValueTask<Payment> RetrievePaymentByStripeReferenceIdAsync(string stripeReferenceId)
        {
            if (string.IsNullOrWhiteSpace(stripeReferenceId)) throw new ArgumentException("StripeReferenceId is required.");

            return await this.storageBroker.SelectPaymentByStripeReferenceIdAsync(stripeReferenceId);
        }

        public async ValueTask<Payment> ModifyPaymentAsync(Payment payment)
        {
            if (payment is null) throw new ArgumentNullException(nameof(payment));

            payment.UpdatedDate = this.dateTimeBroker.GetCurrentDateTimeOffset();

            return await this.storageBroker.UpdatePaymentAsync(payment);
        }
    }
}
