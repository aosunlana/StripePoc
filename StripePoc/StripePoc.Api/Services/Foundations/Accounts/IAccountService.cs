using System;
using System.Linq;
using System.Threading.Tasks;
using StripePoc.Api.Models.Accounts;

namespace StripePoc.Api.Services.Foundations.Accounts
{
    public interface IAccountService
    {
        ValueTask<Account> AddAccountAsync(Account account);
        ValueTask<IQueryable<Account>> RetrieveAllAccountsAsync();
        ValueTask<IQueryable<Account>> RetrieveAllAccountsByBusinessIdAsync(Guid businessId);
        ValueTask<Account> RetrieveAccountByIdAsync(Guid accountId);
    }
}
