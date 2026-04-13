using System;
using System.Linq;
using System.Threading.Tasks;
using StripePoc.Api.Brokers.Loggings;
using StripePoc.Api.Brokers.Storages;
using StripePoc.Api.Models.Accounts;

namespace StripePoc.Api.Services.Foundations.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public AccountService(IStorageBroker storageBroker, ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<Account> AddAccountAsync(Account account) =>
            await this.storageBroker.InsertAccountAsync(account);

        public async ValueTask<IQueryable<Account>> RetrieveAllAccountsAsync() =>
            await this.storageBroker.SelectAllAccountsAsync();

        public async ValueTask<IQueryable<Account>> RetrieveAllAccountsByBusinessIdAsync(Guid businessId) =>
            await this.storageBroker.SelectAllAccountsByBusinessIdAsync(businessId);

        public async ValueTask<Account> RetrieveAccountByIdAsync(Guid accountId) =>
            await this.storageBroker.SelectAccountByIdAsync(accountId);
    }
}
