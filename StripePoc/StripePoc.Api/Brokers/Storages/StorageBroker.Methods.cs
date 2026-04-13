using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StripePoc.Api.Models.Payments;
using StripePoc.Api.Models.Businesses;
using StripePoc.Api.Models.Accounts;
using StripePoc.Api.Models.PaymentAccounts;
using StripePoc.Api.Models.PaymentMethods;
using StripePoc.Api.Models.Subscriptions;
using StripePoc.Api.Models.PaymentIntents;

namespace StripePoc.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        // PaymentAccounts
        public async ValueTask<PaymentAccount> InsertPaymentAccountAsync(PaymentAccount paymentAccount)
        {
            this.PaymentAccounts.Add(paymentAccount);
            await this.SaveChangesAsync();
            return paymentAccount;
        }

        public async ValueTask<PaymentAccount> SelectPaymentAccountByAccountIdAsync(Guid accountId) =>
            await this.PaymentAccounts.FirstOrDefaultAsync(b => b.AccountId == accountId);

        public async ValueTask<PaymentAccount> UpdatePaymentAccountAsync(PaymentAccount paymentAccount)
        {
            this.PaymentAccounts.Update(paymentAccount);
            await this.SaveChangesAsync();
            return paymentAccount;
        }

        public async ValueTask<PaymentAccount> DeletePaymentAccountAsync(PaymentAccount paymentAccount)
        {
            this.PaymentAccounts.Remove(paymentAccount);
            await this.SaveChangesAsync();
            return paymentAccount;
        }

        // PaymentMethods
        public async ValueTask<PaymentMethod> InsertPaymentMethodAsync(PaymentMethod paymentMethod)
        {
            this.PaymentMethods.Add(paymentMethod);
            await this.SaveChangesAsync();
            return paymentMethod;
        }

        public async ValueTask<IQueryable<PaymentMethod>> SelectAllPaymentMethodsByPaymentAccountIdAsync(Guid paymentAccountId) =>
            this.PaymentMethods.Where(p => p.PaymentAccountId == paymentAccountId);

        public async ValueTask<PaymentMethod> SelectDefaultPaymentMethodByPaymentAccountIdAsync(Guid paymentAccountId) =>
            await this.PaymentMethods.FirstOrDefaultAsync(p => p.PaymentAccountId == paymentAccountId && p.IsDefault);

        public async ValueTask<PaymentMethod> SelectPaymentMethodByIdAsync(Guid paymentMethodId) =>
            await this.PaymentMethods.FindAsync(paymentMethodId);

        public async ValueTask<PaymentMethod> UpdatePaymentMethodAsync(PaymentMethod paymentMethod)
        {
            this.PaymentMethods.Update(paymentMethod);
            await this.SaveChangesAsync();
            return paymentMethod;
        }

        public async ValueTask<PaymentMethod> DeletePaymentMethodAsync(PaymentMethod paymentMethod)
        {
            this.PaymentMethods.Remove(paymentMethod);
            await this.SaveChangesAsync();
            return paymentMethod;
        }

        // Subscriptions
        public async ValueTask<Subscription> InsertSubscriptionAsync(Subscription subscription)
        {
            this.Subscriptions.Add(subscription);
            await this.SaveChangesAsync();
            return subscription;
        }

        public async ValueTask<IQueryable<Subscription>> SelectAllSubscriptionsByAccountIdAsync(Guid accountId) =>
            this.Subscriptions.Where(s => s.AccountId == accountId);

        public async ValueTask<Subscription> SelectSubscriptionByIdAsync(Guid subscriptionId) =>
            await this.Subscriptions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == subscriptionId);

        public async ValueTask<Subscription> SelectSubscriptionByStripeSubscriptionIdAsync(string stripeSubscriptionId) =>
            await this.Subscriptions.FirstOrDefaultAsync(s => s.StripeSubscriptionId == stripeSubscriptionId);

        public async ValueTask<Subscription> UpdateSubscriptionAsync(Subscription subscription)
        {
            this.Subscriptions.Update(subscription);
            await this.SaveChangesAsync();
            return subscription;
        }

        public async ValueTask<Subscription> DeleteSubscriptionAsync(Subscription subscription)
        {
            this.Subscriptions.Remove(subscription);
            await this.SaveChangesAsync();
            return subscription;
        }

        // PaymentIntents
        public async ValueTask<PaymentIntent> InsertPaymentIntentAsync(PaymentIntent paymentIntent)
        {
            this.PaymentIntents.Add(paymentIntent);
            await this.SaveChangesAsync();
            return paymentIntent;
        }

        public async ValueTask<IQueryable<PaymentIntent>> SelectAllPaymentIntentsByAccountIdAsync(Guid accountId) =>
            this.PaymentIntents.Where(p => p.AccountId == accountId);

        public async ValueTask<PaymentIntent> SelectPaymentIntentByStripePaymentIntentIdAsync(string stripePaymentIntentId) =>
            await this.PaymentIntents.FirstOrDefaultAsync(p => p.StripePaymentIntentId == stripePaymentIntentId);

        public async ValueTask<PaymentIntent> SelectPaymentIntentByStripeInvoiceIdAsync(string stripeInvoiceId) =>
            await this.PaymentIntents.FirstOrDefaultAsync(p => p.StripeInvoiceId == stripeInvoiceId);

        public async ValueTask<PaymentIntent> UpdatePaymentIntentAsync(PaymentIntent paymentIntent)
        {
            this.PaymentIntents.Update(paymentIntent);
            await this.SaveChangesAsync();
            return paymentIntent;
        }

        // Payments
        public async ValueTask<Payment> InsertPaymentAsync(Payment payment)
        {
            this.Payments.Add(payment);
            await this.SaveChangesAsync();
            return payment;
        }

        public async ValueTask<IQueryable<Payment>> SelectAllPaymentsByAccountIdAsync(Guid accountId) =>
            this.Payments.Where(p => p.AccountId == accountId);

        public async ValueTask<Payment> SelectPaymentByStripeReferenceIdAsync(string stripeReferenceId) =>
            await this.Payments.FirstOrDefaultAsync(p => p.StripeReferenceId == stripeReferenceId);

        public async ValueTask<Payment> UpdatePaymentAsync(Payment payment)
        {
            this.Payments.Update(payment);
            await this.SaveChangesAsync();
            return payment;
        }

        // Business
        public async ValueTask<Business> InsertBusinessAsync(Business business)
        {
            this.Businesses.Add(business);
            await this.SaveChangesAsync();
            return business;
        }

        public async ValueTask<Business> SelectBusinessByIdAsync(Guid businessId) =>
            await this.Businesses.FindAsync(businessId);

        public async ValueTask<IQueryable<Business>> SelectAllBusinessesAsync() =>
            this.Businesses.AsQueryable();

        // Accounts
        public async ValueTask<Account> InsertAccountAsync(Account account)
        {
            this.Accounts.Add(account);
            await this.SaveChangesAsync();
            return account;
        }

        public async ValueTask<IQueryable<Account>> SelectAllAccountsAsync() =>
            this.Accounts.AsQueryable();

        public async ValueTask<IQueryable<Account>> SelectAllAccountsByBusinessIdAsync(Guid businessId) =>
            this.Accounts.Where(a => a.BusinessId == businessId);

        public async ValueTask<Account> SelectAccountByIdAsync(Guid accountId) =>
            await this.Accounts.FindAsync(accountId);
    }
}
