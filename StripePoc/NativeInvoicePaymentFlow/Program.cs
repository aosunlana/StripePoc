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
            Console.WriteLine("=== FLOW: NATIVE INVOICE PAYMENT (Subscription + Tax) ===");
            Console.WriteLine("Goal: Subscription invoice with EXCLUSIVE tax calculated on top by Stripe automatically.");
            Console.WriteLine("NOTE: Requires Stripe Tax enabled in Dashboard > Settings > Tax.\n");

            // STEP 1: Create customer WITH billing address for tax jurisdiction
            Console.WriteLine("1. Creating Customer with billing address (needed for tax jurisdiction lookup)...");
            var customer = await new CustomerService().CreateAsync(new CustomerCreateOptions
            {
                Email = "aged.customer@examplestripe.com",
                Name = "Test Patient",
                // Address is required for Stripe to know which tax rate to apply
                Address = new AddressOptions
                {
                    Line1 = "123 Main Street",
                    City = "New York",
                    State = "NY",
                    PostalCode = "10001",
                    Country = "US"
                },
                PaymentMethod = "pm_card_visa"
            });
            Console.WriteLine($"   Customer: {customer.Id} | Address: New York, NY, USA\n");

            // STEP 2: Create price with TaxBehavior = "exclusive"
            // "exclusive" means: customer pays $15 + tax ON TOP
            // "inclusive" would mean: tax is baked into the $15
            Console.WriteLine("2. Creating $15.00/week Nursing Care service with TaxBehavior = 'exclusive'...");
            var price = await new PriceService().CreateAsync(new PriceCreateOptions
            {
                Currency = "usd",
                UnitAmount = 1500, // $15.00 base — tax will be added on top
                TaxBehavior = "exclusive", // <-- TAX IS ADDED ON TOP OF THIS AMOUNT
                Recurring = new PriceRecurringOptions { Interval = "week" },
                ProductData = new PriceProductDataOptions
                {
                    Name = "Weekly Nursing Care",
                    TaxCode = "txcd_20030000" // Professional Services tax code
                }
            });
            Console.WriteLine($"   Price: {price.Id} | Base: $15.00 (tax will be added on top)\n");

            // STEP 3: Create subscription with AutomaticTax enabled
            Console.WriteLine("3. Creating Subscription with AutomaticTax = { Enabled = true }...");
            var subscription = await new SubscriptionService().CreateAsync(new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                Items = new List<SubscriptionItemOptions> { new SubscriptionItemOptions { Price = price.Id } },
                CollectionMethod = "send_invoice",
                DaysUntilDue = 3,
                AutomaticTax = new SubscriptionAutomaticTaxOptions { Enabled = true }, // <-- ENABLE STRIPE TAX
                Expand = new List<string> { "latest_invoice" }
            });
            Console.WriteLine($"   Subscription: {subscription.Id} | Status: {subscription.Status}\n");

            // STEP 4: Read invoice and show tax breakdown
            var latestInvoice = subscription.LatestInvoice;
            if (latestInvoice != null)
            {
                if (latestInvoice.Status == "draft")
                    latestInvoice = await new InvoiceService().FinalizeInvoiceAsync(latestInvoice.Id);

                Console.WriteLine("===========================================================");
                Console.WriteLine("SUCCESS! Invoice with Tax Breakdown:");
                Console.WriteLine($"  Invoice ID       : {latestInvoice.Id}");
                Console.WriteLine($"  Status           : {latestInvoice.Status}");
                Console.WriteLine($"  Subtotal (base)  : ${latestInvoice.Subtotal / 100.0:F2}");
                Console.WriteLine($"  Tax (exclusive)  : ${(latestInvoice.Total - latestInvoice.Subtotal) / 100.0:F2}");
                Console.WriteLine($"  Total (incl tax) : ${latestInvoice.Total / 100.0:F2}");
                Console.WriteLine($"  AutoTax Status   : {latestInvoice.AutomaticTax?.Status}");
                Console.WriteLine($"\n  Hosted Invoice URL:");
                Console.WriteLine($"  {latestInvoice.HostedInvoiceUrl}");
                Console.WriteLine("===========================================================\n");

                if (latestInvoice.AutomaticTax?.Status == "requires_location_inputs")
                {
                    Console.WriteLine(">>> Tax status is 'requires_location_inputs'.");
                    Console.WriteLine(">>> Please enable Stripe Tax in Dashboard > Settings > Tax.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
