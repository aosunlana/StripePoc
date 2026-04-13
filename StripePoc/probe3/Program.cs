using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe;

class Program
{
    static async Task Main(string[] args)
    {
        StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY"); // From appsettings

        try
        {
            var options = new CustomerCreateOptions { Email = "test@test.com" };
            var customer = await new CustomerService().CreateAsync(options);

            var price = await new PriceService().CreateAsync(new PriceCreateOptions
            {
                Currency = "usd",
                UnitAmount = 1000,
                ProductData = new PriceProductDataOptions { Name = "Test Product" }
            });

            var quoteOptions = new QuoteCreateOptions
            {
                Customer = customer.Id,
                LineItems = new List<QuoteLineItemOptions> { new QuoteLineItemOptions { Price = price.Id } },
                CollectionMethod = "send_invoice",
                InvoiceSettings = new QuoteInvoiceSettingsOptions { DaysUntilDue = 7 }
            };

            var quote = await new QuoteService().CreateAsync(quoteOptions);
            Console.WriteLine("Quote created: {0}", quote.Id);

            quote = await new QuoteService().FinalizeQuoteAsync(quote.Id);
            Console.WriteLine("Quote finalized.");

            var raw = quote.RawJObject;
            Console.WriteLine("RawJObject present? {0}", raw != null);
            if (raw != null)
            {
                Console.WriteLine("Entire JSON:\n" + raw.ToString());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
