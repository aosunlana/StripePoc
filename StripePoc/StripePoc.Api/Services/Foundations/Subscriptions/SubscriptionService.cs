using System;
using System.Linq;
using System.Threading.Tasks;
using StripePoc.Api.Brokers.DateTimes;
using StripePoc.Api.Brokers.Loggings;
using StripePoc.Api.Brokers.Storages;
using StripePoc.Api.Models.Subscriptions;

namespace StripePoc.Api.Services.Foundations.Subscriptions
{
    public interface ISubscriptionService
    {
        ValueTask<Subscription> AddSubscriptionAsync(Subscription subscription);
        ValueTask<IQueryable<Subscription>> RetrieveAllSubscriptionsByAccountIdAsync(Guid accountId);
        ValueTask<Subscription> RetrieveSubscriptionByIdAsync(Guid subscriptionId);
        ValueTask<Subscription> RetrieveSubscriptionByStripeSubscriptionIdAsync(string stripeSubscriptionId);
        ValueTask<Subscription> ModifySubscriptionAsync(Subscription subscription);
        ValueTask<Subscription> RemoveSubscriptionAsync(Guid subscriptionId);
    }

    public partial class SubscriptionService : ISubscriptionService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public SubscriptionService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public async ValueTask<Subscription> AddSubscriptionAsync(Subscription subscription)
        {
            if (subscription is null) throw new ArgumentNullException(nameof(subscription));

            subscription.Id = Guid.NewGuid();
            subscription.CreatedDate = this.dateTimeBroker.GetCurrentDateTimeOffset();
            subscription.UpdatedDate = subscription.CreatedDate;

            return await this.storageBroker.InsertSubscriptionAsync(subscription);
        }

        public async ValueTask<IQueryable<Subscription>> RetrieveAllSubscriptionsByAccountIdAsync(Guid accountId)
        {
            if (accountId == Guid.Empty) throw new ArgumentException("AccountId is required.");

            return await this.storageBroker.SelectAllSubscriptionsByAccountIdAsync(accountId);
        }

        public async ValueTask<Subscription> RetrieveSubscriptionByIdAsync(Guid subscriptionId)
        {
            if (subscriptionId == Guid.Empty) throw new ArgumentException("SubscriptionId is required.");

            return await this.storageBroker.SelectSubscriptionByIdAsync(subscriptionId);
        }

        public async ValueTask<Subscription> RetrieveSubscriptionByStripeSubscriptionIdAsync(string stripeSubscriptionId)
        {
            if (string.IsNullOrWhiteSpace(stripeSubscriptionId)) throw new ArgumentException("StripeSubscriptionId is required.");

            return await this.storageBroker.SelectSubscriptionByStripeSubscriptionIdAsync(stripeSubscriptionId);
        }

        public async ValueTask<Subscription> ModifySubscriptionAsync(Subscription subscription)
        {
            if (subscription is null) throw new ArgumentNullException(nameof(subscription));
            
            subscription.UpdatedDate = this.dateTimeBroker.GetCurrentDateTimeOffset();

            return await this.storageBroker.UpdateSubscriptionAsync(subscription);
        }

        public async ValueTask<Subscription> RemoveSubscriptionAsync(Guid subscriptionId)
        {
            if (subscriptionId == Guid.Empty) throw new ArgumentException("SubscriptionId is required.");

            Subscription subscription = await this.storageBroker.SelectSubscriptionByIdAsync(subscriptionId);
            if (subscription is null) throw new Exception("Subscription not found.");

            return await this.storageBroker.DeleteSubscriptionAsync(subscription);
        }
    }
}
