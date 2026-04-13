using Stripe;

class Program
{
    static async Task Main()
    {
        StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");

        try
        {
            Console.WriteLine("=== FLOW: NATIVE QUOTE APPROVAL ===");
            Console.WriteLine("Goal: Employee initiates a Quote. Upon approval, Stripe instantly auto-deducts from the saved card.");

            // 1. Create a customer with a saved Payment Method for auto-charge
            Console.WriteLine("\n1. Creating Customer and permanently saving a test Visa card (pm_card_visa)...");
            var customer = await new CustomerService().CreateAsync(new CustomerCreateOptions
            {
                Email = "osunlanaabdulsamad@gmail.com", // YOUR VERIFIED TEST EMAIL GOES HERE
                PaymentMethod = "pm_card_visa",
                InvoiceSettings = new CustomerInvoiceSettingsOptions { DefaultPaymentMethod = "pm_card_visa" }
            });

            // 2. Create the Price
            Console.WriteLine("2. Creating Service ($50.00 One-time)...");
            var price = await new PriceService().CreateAsync(new PriceCreateOptions
            {
                Currency = "usd",
                UnitAmount = 5000,
                ProductData = new PriceProductDataOptions { Name = "Weekly Nursing Quote Service" }
            });

            // 3. Create the Quote telling Stripe to Auto-Charge upon acceptance
            Console.WriteLine("3. Generating Quote with CollectionMethod = 'charge_automatically'...");
            var quote = await new QuoteService().CreateAsync(new QuoteCreateOptions
            {
                Customer = customer.Id,
                LineItems = new List<QuoteLineItemOptions> { new QuoteLineItemOptions { Price = price.Id } },
                CollectionMethod = "charge_automatically" // <--- The magic toggle for auto-deduction
            });

            // 4. Finalize the Quote
            quote = await new QuoteService().FinalizeQuoteAsync(quote.Id);
            Console.WriteLine($"\nSUCCESS: Quote {quote.Id} Finalized!");

            Console.WriteLine("\n*** IMPORTANT STRIPE QUOTE LIMITATION ***");
            Console.WriteLine("Unlike Invoices, Stripe does NOT have a 'SendQuoteAsync()' API method to trigger the email programmatically.");
            Console.WriteLine("To send this quote, the Employee must either log into the Stripe Dashboard and click 'Send Quote',");
            Console.WriteLine("or you must manually download the PDF via API and email it.");
            Console.WriteLine("Once the customer manually accepts the Quote, Stripe will AUTOMATICALLY charge the Visa card we attached in Step 1!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
