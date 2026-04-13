using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StripePoc.Api.Models.Accounts;
using StripePoc.Api.Models.Businesses;
using StripePoc.Api.Services.Foundations.Accounts;
using StripePoc.Api.Services.Foundations.Businesses;

namespace StripePoc.Api.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var businessService = scope.ServiceProvider.GetRequiredService<IBusinessService>();
            var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();

            var existingBusinesses = await businessService.RetrieveAllBusinessesAsync();
            if (existingBusinesses.Any()) return;

            var business = new Business
            {
                Id = Guid.NewGuid(),
                Name = "Subscription POC",
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow
            };
            await businessService.AddBusinessAsync(business);

            string[] accountNames = { "Corporate HQ", "Regional Support", "Marketing Group" };

            foreach (var name in accountNames)
            {
                var account = new Account
                {
                    Id = Guid.NewGuid(),
                    BusinessId = business.Id,
                    Name = name,
                    CreatedDate = DateTimeOffset.UtcNow,
                    UpdatedDate = DateTimeOffset.UtcNow
                };
                await accountService.AddAccountAsync(account);
            }
        }
    }
}
