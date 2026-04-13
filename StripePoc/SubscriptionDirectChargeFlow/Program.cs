using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe;

class Program
{
    static async Task Main()
    {
        StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");

        try
        {
            Console.WriteLine("=== FLOW 2: SUBSCRIPTION DIRECT AUTO-CHARGE ===");
            Console.WriteLine("Goal: Start a subscription for a customer that skips approvals and instantly charges the corporate card on file.");

            Console.WriteLine("\n1. Creating Customer with pre-saved testing Visa card...");
            var customer = await new CustomerService().CreateAsync(new CustomerCreateOptions
            {
                PaymentMethod = "pm_card_visa",
                InvoiceSettings = new CustomerInvoiceSettingsOptions { DefaultPaymentMethod = "pm_card_visa" }
            });

            Console.WriteLine("2. Creating Service ($25.00/week)...");
            var price = await new PriceService().CreateAsync(new PriceCreateOptions
            {
                Currency = "usd", UnitAmount = 2500,
                Recurring = new PriceRecurringOptions { Interval = "week" },
                ProductData = new PriceProductDataOptions { Name = "Weekly Premium Auto-Charge Care" }
            });

            Console.WriteLine("3. Creating Subscription with 'charge_automatically'...");
            var subscription = await new SubscriptionService().CreateAsync(new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                Items = new List<SubscriptionItemOptions> { new SubscriptionItemOptions { Price = price.Id } },
                CollectionMethod = "charge_automatically" // Natively pulls funds immediately
            });

            Console.WriteLine("\n===========================================================");
            Console.WriteLine("SUCCESS!");
            Console.WriteLine($"Subscription {subscription.Id} successfully generated!");
            Console.WriteLine($"Current Status: {subscription.Status}");
            Console.WriteLine("Because the card was saved, Stripe successfully charged it instantly.");
            Console.WriteLine("===========================================================\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
