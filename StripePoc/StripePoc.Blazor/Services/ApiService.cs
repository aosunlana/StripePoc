using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using StripePoc.Blazor.Models.Payments;
using StripePoc.Blazor.Models.Businesses;
using StripePoc.Blazor.Models.Accounts;
using StripePoc.Blazor.Models.PaymentAccounts;
using StripePoc.Blazor.Models.PaymentMethods;
using StripePoc.Blazor.Models.Subscriptions;
using StripePoc.Blazor.Models.PaymentIntents;
using StripePoc.Blazor.Models.Events;

namespace StripePoc.Blazor.Services
{
    public class ApiService
    {
        private readonly HttpClient httpClient;

        public ApiService(HttpClient httpClient) =>
            this.httpClient = httpClient;

        // Identity
        public async Task<List<Business>> GetBusinessesAsync() =>
            await this.httpClient.GetFromJsonAsync<List<Business>>("api/account/businesses");

        public async Task<List<Account>> GetAccountsByBusinessAsync(Guid businessId) =>
            await this.httpClient.GetFromJsonAsync<List<Account>>($"api/account/business/{businessId}");

        public async Task<List<Account>> GetAllAccountsAsync() =>
            await this.httpClient.GetFromJsonAsync<List<Account>>("api/account/all");

        // Payment Accounts
        public async Task<PaymentAccount> ProvidePaymentAccountAsync(Guid accountId)
        {
            var response = await this.httpClient.PostAsync($"api/paymentaccounts/{accountId}", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PaymentAccount>();
        }

        // Payment Methods
        public async Task<string> InitiateCardSetupAsync(Guid accountId)
        {
            var response = await this.httpClient.PostAsync($"api/paymentmethods/accounts/{accountId}/setup/initiate", null);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<PaymentMethod> FinaliseCardSetupAsync(Guid accountId, string stripePaymentMethodId)
        {
            var paymentMethod = new PaymentMethod { StripePaymentMethodId = stripePaymentMethodId };
            var response = await this.httpClient.PostAsJsonAsync($"api/paymentmethods/accounts/{accountId}", paymentMethod);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PaymentMethod>();
        }

        public async Task<List<PaymentMethod>> GetPaymentMethodsAsync(Guid accountId) =>
            await this.httpClient.GetFromJsonAsync<List<PaymentMethod>>($"api/paymentmethods/accounts/{accountId}");

        // Subscriptions
        public async Task<Subscription> CreateSubscriptionAsync(Subscription subscription)
        {
            var response = await this.httpClient.PostAsJsonAsync("api/subscriptions", subscription);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Subscription>();
        }

        public async Task<PaymentLifecycleEvent> CreateSubscriptionQuoteAsync(Subscription subscription)
        {
            var response = await this.httpClient.PostAsJsonAsync("api/subscriptions/quote", subscription);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PaymentLifecycleEvent>();
        }

        public async Task<List<Subscription>> GetSubscriptionsAsync(Guid accountId) =>
            await this.httpClient.GetFromJsonAsync<List<Subscription>>($"api/subscriptions/accounts/{accountId}");

        public async Task CancelSubscriptionAsync(Guid subscriptionId) =>
            await this.httpClient.DeleteAsync($"api/subscriptions/{subscriptionId}");

        // One-time Payments
        public async Task<PaymentIntent> ProcessOneTimePaymentAsync(PaymentIntent paymentIntent)
        {
            var response = await this.httpClient.PostAsJsonAsync("api/one-time-payments", paymentIntent);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PaymentIntent>();
        }

        public async Task<PaymentLifecycleEvent> ProcessOneTimeQuotePaymentAsync(PaymentIntent paymentIntent)
        {
            var response = await this.httpClient.PostAsJsonAsync("api/one-time-payments/quote", paymentIntent);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PaymentLifecycleEvent>();
        }

        public async Task<string> RefundLastPaymentAsync(Guid accountId)
        {
            var response = await this.httpClient.PostAsync($"api/one-time-payments/accounts/{accountId}/refund-last", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // Payment History
        public async Task<List<Payment>> GetPaymentHistoryAsync(Guid accountId) =>
            await this.httpClient.GetFromJsonAsync<List<Payment>>($"api/payments/accounts/{accountId}");
    }
}
