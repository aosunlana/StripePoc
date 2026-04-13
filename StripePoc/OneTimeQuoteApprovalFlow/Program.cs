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
            Console.WriteLine("=== FLOW 4: ONE-TIME QUOTE APPROVAL AUTO-CHARGE ===");
            Console.WriteLine("Goal: Send a one-time service quote. When customer accepts, Stripe auto-charges their saved card.");
            
            Console.WriteLine("\n1. Creating Customer with a saved card...");
            var customer = await new CustomerService().CreateAsync(new CustomerCreateOptions
            {
                Email = "customer.onetimequote@examplestripe.com",
                PaymentMethod = "pm_card_visa",
                InvoiceSettings = new CustomerInvoiceSettingsOptions { DefaultPaymentMethod = "pm_card_visa" }
            });

            Console.WriteLine("2. Creating the Service ($120.00 One-time)...");
            var price = await new PriceService().CreateAsync(new PriceCreateOptions
            {
                Currency = "usd", UnitAmount = 12000,
                ProductData = new PriceProductDataOptions { Name = "Specialized Employee Audit" }
            });

            Console.WriteLine("3. Creating the Quote...");
            var quote = await new QuoteService().CreateAsync(new QuoteCreateOptions
            {
                Customer = customer.Id,
                LineItems = new List<QuoteLineItemOptions> { new QuoteLineItemOptions { Price = price.Id } },
                CollectionMethod = "charge_automatically" // Natively pulls funds later
            });

            quote = await new QuoteService().FinalizeQuoteAsync(quote.Id);
            Console.WriteLine($"\nSUCCESS: Quote {quote.Id} finalized!");
            Console.WriteLine("Again, in a real environment the employee sends this Quote via Dashboard.");
            Console.WriteLine("Once the user clicks 'Accept' in the emailed Quote PDF link, Stripe will charge the $120.00 to the visa!\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
