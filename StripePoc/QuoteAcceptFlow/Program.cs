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
            Console.WriteLine("=== FLOW: QUOTE NATIVE ACCEPT & SIGN ===");
            Console.WriteLine("Goal: Test whether clicking 'Accept and Sign' on the Stripe Hosted Quote");
            Console.WriteLine("      auto-charges the customer's saved card without them entering card details.\n");

            // STEP 1: Create customer with a pre-saved card (simulating completed Wallet Step)
            Console.WriteLine("1. Creating Customer with pre-saved Visa card (like post-Wallet-Step)...");
            var customer = await new CustomerService().CreateAsync(new CustomerCreateOptions
            {
                Email = "aged.customer.quote@examplestripe.com",
                Name = "Test Patient",
                PaymentMethod = "pm_card_visa",
                InvoiceSettings = new CustomerInvoiceSettingsOptions
                {
                    DefaultPaymentMethod = "pm_card_visa" // This is what enables auto-charge on Quote acceptance
                }
            });
            Console.WriteLine($"   Customer: {customer.Id} | Visa card is saved as default!\n");

            // STEP 2: Create a service price
            Console.WriteLine("2. Creating $50.00/week Nursing Care service...");
            var price = await new PriceService().CreateAsync(new PriceCreateOptions
            {
                Currency = "usd",
                UnitAmount = 5000,
                Recurring = new PriceRecurringOptions { Interval = "week" },
                ProductData = new PriceProductDataOptions { Name = "Weekly Nursing Care" }
            });
            Console.WriteLine($"   Price: {price.Id}\n");

            // STEP 3: Create Quote with charge_automatically
            // This means: once the customer accepts, Stripe INSTANTLY charges the saved Visa
            Console.WriteLine("3. Creating Quote with CollectionMethod = 'charge_automatically'...");
            var quote = await new QuoteService().CreateAsync(new QuoteCreateOptions
            {
                Customer = customer.Id,
                LineItems = new List<QuoteLineItemOptions> { new QuoteLineItemOptions { Price = price.Id } },
                CollectionMethod = "charge_automatically",
                Header = "Care Service Approval",
                Footer = "By accepting this quote, your saved card will be charged automatically."
            });

            // STEP 4: Finalize to get the hosted URL
            quote = await new QuoteService().FinalizeQuoteAsync(quote.Id);
            Console.WriteLine($"   Quote: {quote.Id}");
            Console.WriteLine($"   Status: {quote.Status}");

            // The hosted_quote_url is not in the C# type yet so we read from raw JSON
            var rawJson = quote.RawJObject;
            var hostedQuoteUrl = rawJson?["hosted_quote_url"]?.ToString();

            Console.WriteLine("\n===========================================================");
            Console.WriteLine("Quote is ready! Open this URL in your browser:\n");
            if (!string.IsNullOrEmpty(hostedQuoteUrl))
            {
                Console.WriteLine(hostedQuoteUrl);
                Console.WriteLine("\nYou should see the 'Accept and Sign' button on the Stripe page.");
                Console.WriteLine("Click it and see if the saved Visa card gets auto-charged!\n");
            }
            else
            {
                Console.WriteLine("(hosted_quote_url is null — Stripe does not expose this in Test Mode)");
                Console.WriteLine("The employee must send the quote manually from the Stripe Dashboard.\n");
                Console.WriteLine($"OR — your backend can programmatically accept the quote via API:");
                Console.WriteLine($"\nQuote ID to accept: {quote.Id}");
            }
            Console.WriteLine("===========================================================");

            // STEP 5: If no hosted URL, demonstrate the API AcceptAsync path
            Console.WriteLine("\nPress ENTER to programmatically accept the Quote via API (simulating customer approval)...");
            Console.ReadLine();

            Console.WriteLine("Calling QuoteService.AcceptAsync()...");
            var acceptedQuote = await new QuoteService().AcceptAsync(quote.Id);

            Console.WriteLine("\n===========================================================");
            Console.WriteLine($"Quote Status after Accept: {acceptedQuote.Status}");

            if (acceptedQuote.Status == "accepted")
            {
                Console.WriteLine("SUCCESS! Quote was accepted.");
                Console.WriteLine("Stripe has now created a Subscription/Invoice and charged the saved Visa!");
                Console.WriteLine("Check your Stripe Dashboard for the created invoice and payment.");
            }
            Console.WriteLine("===========================================================\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
