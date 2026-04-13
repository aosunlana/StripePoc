using System;
using System.Linq;
using System.Threading.Tasks;
using StripePoc.Api.Brokers.DateTimes;
using StripePoc.Api.Brokers.Loggings;
using StripePoc.Api.Brokers.Storages;
using StripePoc.Api.Models.PaymentIntents;

namespace StripePoc.Api.Services.Foundations.PaymentIntents
{
    public interface IPaymentIntentService
    {
        ValueTask<PaymentIntent> AddPaymentIntentAsync(PaymentIntent paymentIntent);
        ValueTask<IQueryable<PaymentIntent>> RetrieveAllPaymentIntentsByAccountIdAsync(Guid accountId);
        ValueTask<PaymentIntent> RetrievePaymentIntentByStripePaymentIntentIdAsync(string stripePaymentIntentId);
        ValueTask<PaymentIntent> RetrievePaymentIntentByStripeInvoiceIdAsync(string stripeInvoiceId);
        ValueTask<PaymentIntent> ModifyPaymentIntentAsync(PaymentIntent paymentIntent);
    }

    public partial class PaymentIntentService : IPaymentIntentService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public PaymentIntentService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public async ValueTask<PaymentIntent> AddPaymentIntentAsync(PaymentIntent paymentIntent)
        {
            if (paymentIntent is null) throw new ArgumentNullException(nameof(paymentIntent));

            paymentIntent.Id = Guid.NewGuid();
            paymentIntent.CreatedDate = this.dateTimeBroker.GetCurrentDateTimeOffset();
            paymentIntent.UpdatedDate = paymentIntent.CreatedDate;

            return await this.storageBroker.InsertPaymentIntentAsync(paymentIntent);
        }

        public async ValueTask<IQueryable<PaymentIntent>> RetrieveAllPaymentIntentsByAccountIdAsync(Guid accountId)
        {
            if (accountId == Guid.Empty) throw new ArgumentException("AccountId is required.");

            return await this.storageBroker.SelectAllPaymentIntentsByAccountIdAsync(accountId);
        }

        public async ValueTask<PaymentIntent> RetrievePaymentIntentByStripePaymentIntentIdAsync(string stripePaymentIntentId)
        {
            if (string.IsNullOrWhiteSpace(stripePaymentIntentId)) throw new ArgumentException("StripePaymentIntentId is required.");

            return await this.storageBroker.SelectPaymentIntentByStripePaymentIntentIdAsync(stripePaymentIntentId);
        }

        public async ValueTask<PaymentIntent> RetrievePaymentIntentByStripeInvoiceIdAsync(string stripeInvoiceId)
        {
            if (string.IsNullOrWhiteSpace(stripeInvoiceId)) throw new ArgumentException("StripeInvoiceId is required.");

            return await this.storageBroker.SelectPaymentIntentByStripeInvoiceIdAsync(stripeInvoiceId);
        }

        public async ValueTask<PaymentIntent> ModifyPaymentIntentAsync(PaymentIntent paymentIntent)
        {
            if (paymentIntent is null) throw new ArgumentNullException(nameof(paymentIntent));
            
            paymentIntent.UpdatedDate = this.dateTimeBroker.GetCurrentDateTimeOffset();

            return await this.storageBroker.UpdatePaymentIntentAsync(paymentIntent);
        }
    }
}
