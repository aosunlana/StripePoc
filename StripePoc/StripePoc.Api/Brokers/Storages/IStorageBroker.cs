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
    public interface IStorageBroker
    {
        // PaymentAccounts
        ValueTask<PaymentAccount> InsertPaymentAccountAsync(PaymentAccount paymentAccount);
        ValueTask<PaymentAccount> SelectPaymentAccountByAccountIdAsync(Guid accountId);
        ValueTask<PaymentAccount> UpdatePaymentAccountAsync(PaymentAccount paymentAccount);
        ValueTask<PaymentAccount> DeletePaymentAccountAsync(PaymentAccount paymentAccount);

        // PaymentMethods
        ValueTask<PaymentMethod> InsertPaymentMethodAsync(PaymentMethod paymentMethod);
        ValueTask<IQueryable<PaymentMethod>> SelectAllPaymentMethodsByPaymentAccountIdAsync(Guid paymentAccountId);
        ValueTask<PaymentMethod> SelectDefaultPaymentMethodByPaymentAccountIdAsync(Guid paymentAccountId);
        ValueTask<PaymentMethod> SelectPaymentMethodByIdAsync(Guid paymentMethodId);
        ValueTask<PaymentMethod> UpdatePaymentMethodAsync(PaymentMethod paymentMethod);
        ValueTask<PaymentMethod> DeletePaymentMethodAsync(PaymentMethod paymentMethod);

        // Subscriptions
        ValueTask<Subscription> InsertSubscriptionAsync(Subscription subscription);
        ValueTask<IQueryable<Subscription>> SelectAllSubscriptionsByAccountIdAsync(Guid accountId);
        ValueTask<Subscription> SelectSubscriptionByIdAsync(Guid subscriptionId);
        ValueTask<Subscription> SelectSubscriptionByStripeSubscriptionIdAsync(string stripeSubscriptionId);
        ValueTask<Subscription> UpdateSubscriptionAsync(Subscription subscription);
        ValueTask<Subscription> DeleteSubscriptionAsync(Subscription subscription);

        // PaymentIntents
        ValueTask<PaymentIntent> InsertPaymentIntentAsync(PaymentIntent paymentIntent);
        ValueTask<IQueryable<PaymentIntent>> SelectAllPaymentIntentsByAccountIdAsync(Guid accountId);
        ValueTask<PaymentIntent> SelectPaymentIntentByStripePaymentIntentIdAsync(string stripePaymentIntentId);
        ValueTask<PaymentIntent> SelectPaymentIntentByStripeInvoiceIdAsync(string stripeInvoiceId);
        ValueTask<PaymentIntent> UpdatePaymentIntentAsync(PaymentIntent paymentIntent);

        // Payments
        ValueTask<Payment> InsertPaymentAsync(Payment payment);
        ValueTask<IQueryable<Payment>> SelectAllPaymentsByAccountIdAsync(Guid accountId);
        ValueTask<Payment> SelectPaymentByStripeReferenceIdAsync(string stripeReferenceId);
        ValueTask<Payment> UpdatePaymentAsync(Payment payment);

        // Business
        ValueTask<Business> InsertBusinessAsync(Business business);
        ValueTask<Business> SelectBusinessByIdAsync(Guid businessId);
        ValueTask<IQueryable<Business>> SelectAllBusinessesAsync();

        // Accounts
        ValueTask<Account> InsertAccountAsync(Account account);
        ValueTask<IQueryable<Account>> SelectAllAccountsAsync();
        ValueTask<IQueryable<Account>> SelectAllAccountsByBusinessIdAsync(Guid businessId);
        ValueTask<Account> SelectAccountByIdAsync(Guid accountId);
    }
}
