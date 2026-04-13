using System;
using System.Linq;
using System.Threading.Tasks;
using StripePoc.Api.Brokers.Loggings;
using StripePoc.Api.Brokers.Storages;
using StripePoc.Api.Models.Businesses;

namespace StripePoc.Api.Services.Foundations.Businesses
{
    public class BusinessService : IBusinessService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public BusinessService(IStorageBroker storageBroker, ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<Business> AddBusinessAsync(Business business) =>
            await this.storageBroker.InsertBusinessAsync(business);

        public async ValueTask<Business> RetrieveBusinessByIdAsync(Guid businessId) =>
            await this.storageBroker.SelectBusinessByIdAsync(businessId);

        public async ValueTask<IQueryable<Business>> RetrieveAllBusinessesAsync() =>
            await this.storageBroker.SelectAllBusinessesAsync();
    }
}
