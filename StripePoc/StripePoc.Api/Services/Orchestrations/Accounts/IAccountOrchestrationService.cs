using System;
using System.Linq;
using System.Threading.Tasks;
using StripePoc.Api.Models.Accounts;
using StripePoc.Api.Models.Businesses;

namespace StripePoc.Api.Services.Orchestrations.Accounts
{
    public interface IAccountOrchestrationService
    {
        ValueTask<IQueryable<Business>> RetrieveAllBusinessesAsync();
        ValueTask<IQueryable<Account>> RetrieveAllAccountsAsync();
        ValueTask<IQueryable<Account>> RetrieveAccountsByBusinessIdAsync(Guid businessId);
    }
}
