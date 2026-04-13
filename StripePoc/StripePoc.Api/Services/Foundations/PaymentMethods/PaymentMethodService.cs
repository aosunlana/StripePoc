using System;
using System.Linq;
using System.Threading.Tasks;
using StripePoc.Api.Brokers.DateTimes;
using StripePoc.Api.Brokers.Loggings;
using StripePoc.Api.Brokers.Storages;
using StripePoc.Api.Models.PaymentMethods;

namespace StripePoc.Api.Services.Foundations.PaymentMethods
{
    public interface IPaymentMethodService
    {
        ValueTask<PaymentMethod> AddPaymentMethodAsync(PaymentMethod paymentMethod);
        ValueTask<IQueryable<PaymentMethod>> RetrieveAllPaymentMethodsByPaymentAccountIdAsync(Guid paymentAccountId);
        ValueTask<PaymentMethod> RetrieveDefaultPaymentMethodByPaymentAccountIdAsync(Guid paymentAccountId);
        ValueTask<PaymentMethod> RetrievePaymentMethodByIdAsync(Guid paymentMethodId);
        ValueTask<PaymentMethod> ModifyPaymentMethodAsync(PaymentMethod paymentMethod);
        ValueTask<PaymentMethod> RemovePaymentMethodAsync(Guid paymentMethodId);
    }

    public partial class PaymentMethodService : IPaymentMethodService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public PaymentMethodService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public async ValueTask<PaymentMethod> AddPaymentMethodAsync(PaymentMethod paymentMethod)
        {
            if (paymentMethod is null) throw new ArgumentNullException(nameof(paymentMethod));

            paymentMethod.Id = Guid.NewGuid();
            paymentMethod.CreatedDate = this.dateTimeBroker.GetCurrentDateTimeOffset();
            paymentMethod.UpdatedDate = paymentMethod.CreatedDate;

            return await this.storageBroker.InsertPaymentMethodAsync(paymentMethod);
        }

        public async ValueTask<IQueryable<PaymentMethod>> RetrieveAllPaymentMethodsByPaymentAccountIdAsync(Guid paymentAccountId)
        {
            if (paymentAccountId == Guid.Empty) throw new ArgumentException("PaymentAccountId is required.");

            return await this.storageBroker.SelectAllPaymentMethodsByPaymentAccountIdAsync(paymentAccountId);
        }

        public async ValueTask<PaymentMethod> RetrieveDefaultPaymentMethodByPaymentAccountIdAsync(Guid paymentAccountId)
        {
            if (paymentAccountId == Guid.Empty) throw new ArgumentException("PaymentAccountId is required.");

            return await this.storageBroker.SelectDefaultPaymentMethodByPaymentAccountIdAsync(paymentAccountId);
        }

        public async ValueTask<PaymentMethod> RetrievePaymentMethodByIdAsync(Guid paymentMethodId)
        {
            if (paymentMethodId == Guid.Empty) throw new ArgumentException("PaymentMethodId is required.");

            return await this.storageBroker.SelectPaymentMethodByIdAsync(paymentMethodId);
        }

        public async ValueTask<PaymentMethod> ModifyPaymentMethodAsync(PaymentMethod paymentMethod)
        {
            if (paymentMethod is null) throw new ArgumentNullException(nameof(paymentMethod));

            paymentMethod.UpdatedDate = this.dateTimeBroker.GetCurrentDateTimeOffset();

            return await this.storageBroker.UpdatePaymentMethodAsync(paymentMethod);
        }

        public async ValueTask<PaymentMethod> RemovePaymentMethodAsync(Guid paymentMethodId)
        {
            if (paymentMethodId == Guid.Empty) throw new ArgumentException("PaymentMethodId is required.");

            PaymentMethod paymentMethod = await this.storageBroker.SelectPaymentMethodByIdAsync(paymentMethodId);
            if (paymentMethod is null) throw new Exception("PaymentMethod not found.");

            return await this.storageBroker.DeletePaymentMethodAsync(paymentMethod);
        }
    }
}
