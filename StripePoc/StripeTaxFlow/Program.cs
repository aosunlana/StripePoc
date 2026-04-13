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
            Console.WriteLine("=== FLOW: STRIPE TAX ===");
            Console.WriteLine("Goal: Automatically calculate and apply the correct tax rate to invoices.");
            Console.WriteLine("NOTE: Stripe Tax must be enabled in your Dashboard under Settings > Tax.\n");

            // STEP 1: Create a customer WITH an address
            // This is critical! Stripe Tax uses the customer's address to determine
            // the correct tax jurisdiction (state/county/country VAT rates etc.)
            Console.WriteLine("1. Creating Customer with billing address for tax jurisdiction lookup...");
            var customer = await new CustomerService().CreateAsync(new CustomerCreateOptions
            {
                Email = "taxable.customer@examplestripe.com",
                Name = "John Smith",
                Address = new AddressOptions
                {
                    Line1 = "123 Main Street",
                    City = "New York",
                    State = "NY",
                    PostalCode = "10001",
                    Country = "US"
                },
                PaymentMethod = "pm_card_visa",
                InvoiceSettings = new CustomerInvoiceSettingsOptions
                {
                    DefaultPaymentMethod = "pm_card_visa"
                }
            });
            Console.WriteLine($"   Customer created: {customer.Id}");
            Console.WriteLine($"   Address: New York, NY, USA (Stripe will look up NY tax rate)\n");

            // STEP 2: Create a Product/Price
            // Mark the product as taxable using tax_code
            // txcd_10000000 = General - Tangible Goods
            // txcd_90000001 = Software as a Service (SaaS)
            // txcd_20030000 = Professional Services (most likely for nursing/care)
            Console.WriteLine("2. Creating Care Service with Tax Code (txcd_20030000 = Professional Services)...");
            var product = await new ProductService().CreateAsync(new ProductCreateOptions
            {
                Name = "Weekly Nursing Care",
                TaxCode = "txcd_20030000" // Professional Services tax code
            });

            var price = await new PriceService().CreateAsync(new PriceCreateOptions
            {
                Currency = "usd",
                UnitAmount = 10000, // $100.00
                Recurring = new PriceRecurringOptions { Interval = "week" },
                Product = product.Id
            });
            Console.WriteLine($"   Product: {product.Id}");
            Console.WriteLine($"   Price: {price.Id}\n");

            // STEP 3: Create a subscription with automatic_tax ENABLED
            // This single flag tells Stripe to:
            // - Look up the customer's address
            // - Find the applicable tax rate for that jurisdiction
            // - Automatically add it to the invoice line items
            Console.WriteLine("3. Creating Subscription with automatic_tax = { enabled: true }...");
            var subscription = await new SubscriptionService().CreateAsync(new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                Items = new List<SubscriptionItemOptions> { new SubscriptionItemOptions { Price = price.Id } },
                CollectionMethod = "send_invoice",
                DaysUntilDue = 3,
                AutomaticTax = new SubscriptionAutomaticTaxOptions { Enabled = true }, // <-- THE KEY FLAG
                Expand = new List<string> { "latest_invoice" }
            });

            Console.WriteLine($"   Subscription: {subscription.Id}");
            Console.WriteLine($"   Status: {subscription.Status}\n");

            // STEP 4: Inspect the invoice to see tax applied
            var latestInvoice = subscription.LatestInvoice;
            if (latestInvoice != null)
            {
                var invoiceService = new InvoiceService();
                if (latestInvoice.Status == "draft")
                    latestInvoice = await invoiceService.FinalizeInvoiceAsync(latestInvoice.Id);

                Console.WriteLine("===========================================================");
                Console.WriteLine("SUCCESS! Invoice created with Stripe Tax breakdown:");
                Console.WriteLine($"  Invoice ID    : {latestInvoice.Id}");
                Console.WriteLine($"  Status        : {latestInvoice.Status}");
                Console.WriteLine($"  Subtotal      : ${latestInvoice.Subtotal / 100.0:F2}");
                Console.WriteLine($"  Tax           : ${latestInvoice.TotalExcludingTax.GetValueOrDefault() / 100.0:F2} (excl. tax base)");
                Console.WriteLine($"  Total         : ${latestInvoice.Total / 100.0:F2}");
                Console.WriteLine($"\n  Automatic Tax Status : {latestInvoice.AutomaticTax?.Status}");
                Console.WriteLine($"\n  Hosted Invoice URL (customer sees full tax breakdown):");
                Console.WriteLine($"  {latestInvoice.HostedInvoiceUrl}");
                Console.WriteLine("===========================================================\n");

                Console.WriteLine("NOTE: If Tax is '$0.00' and status is 'requires_location_inputs',");
                Console.WriteLine("you need to enable Stripe Tax in your Dashboard:");
                Console.WriteLine("Dashboard > Settings > Tax > Enable automatic tax collection");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
