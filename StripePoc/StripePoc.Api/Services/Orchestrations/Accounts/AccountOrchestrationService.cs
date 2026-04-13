using System;
using System.Linq;
using System.Threading.Tasks;
using StripePoc.Api.Brokers.Loggings;
using StripePoc.Api.Services.Foundations.Accounts;
using StripePoc.Api.Services.Foundations.Businesses;
using StripePoc.Api.Models.Accounts;
using StripePoc.Api.Models.Businesses;

namespace StripePoc.Api.Services.Orchestrations.Accounts
{
    public class AccountOrchestrationService : IAccountOrchestrationService
    {
        private readonly IBusinessService businessService;
        private readonly IAccountService accountService;
        private readonly ILoggingBroker loggingBroker;

        public AccountOrchestrationService(
            IBusinessService businessService,
            IAccountService accountService,
            ILoggingBroker loggingBroker)
        {
            this.businessService = businessService;
            this.accountService = accountService;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<IQueryable<Business>> RetrieveAllBusinessesAsync() =>
            await this.businessService.RetrieveAllBusinessesAsync();

        public async ValueTask<IQueryable<Account>> RetrieveAllAccountsAsync() =>
            await this.accountService.RetrieveAllAccountsAsync();

        public async ValueTask<IQueryable<Account>> RetrieveAccountsByBusinessIdAsync(Guid businessId) =>
            await this.accountService.RetrieveAllAccountsByBusinessIdAsync(businessId);
    }
}
