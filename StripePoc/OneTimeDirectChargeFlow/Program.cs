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
            Console.WriteLine("=== FLOW: ONE-TIME DIRECT CHARGE (With Tax) ===");
            Console.WriteLine("Goal: One-off Invoice, instantly auto-charged, with exclusive tax calculated on top.");
            Console.WriteLine("NOTE: Requires Stripe Tax enabled in Dashboard > Settings > Tax.\n");

            // STEP 1: Create customer WITH billing address for tax jurisdiction
            Console.WriteLine("1. Creating Customer with billing address and saved card...");
            var customer = await new CustomerService().CreateAsync(new CustomerCreateOptions
            {
                Email = "onetime.customer@examplestripe.com",
                Name = "One-Time Patient",
                Address = new AddressOptions
                {
                    Line1 = "456 Park Avenue",
                    City = "New York",
                    State = "NY",
                    PostalCode = "10022",
                    Country = "US"
                },
                PaymentMethod = "pm_card_visa",
                InvoiceSettings = new CustomerInvoiceSettingsOptions
                {
                    DefaultPaymentMethod = "pm_card_visa"
                }
            });
            Console.WriteLine($"   Customer: {customer.Id} | Address: New York, NY, USA\n");

            // STEP 2: Create a one-time Price with TaxBehavior = "exclusive"
            Console.WriteLine("2. Creating $150.00 one-time service price with TaxBehavior = 'exclusive'...");
            var price = await new PriceService().CreateAsync(new PriceCreateOptions
            {
                Currency = "usd",
                UnitAmount = 15000, // $150.00 base
                TaxBehavior = "exclusive", // <-- Tax added ON TOP of $150
                ProductData = new PriceProductDataOptions
                {
                    Name = "Specialist Nursing Consultation",
                    TaxCode = "txcd_20030000" // Professional Services
                }
            });
            Console.WriteLine($"   Price: {price.Id} | Base: $150.00 (exclusive tax will be added on top)\n");

            // STEP 3: Add invoice item using the price
            Console.WriteLine("3. Adding line item to invoice...");
            await new InvoiceItemService().CreateAsync(new InvoiceItemCreateOptions
            {
                Customer = customer.Id,
                PriceData = new InvoiceItemPriceDataOptions
                {
                    Currency = "usd",
                    UnitAmount = 15000, // $150.00 base — tax added on top
                    TaxBehavior = "exclusive",
                    Product = price.ProductId
                }
            });

            // STEP 4: Create one-time invoice with auto-collection + AutomaticTax
            Console.WriteLine("4. Creating Invoice with AutomaticTax = { Enabled = true } and charge_automatically...");
            var invoice = await new InvoiceService().CreateAsync(new InvoiceCreateOptions
            {
                Customer = customer.Id,
                CollectionMethod = "charge_automatically", // Charges the saved Visa immediately
                AutomaticTax = new InvoiceAutomaticTaxOptions { Enabled = true } // <-- ENABLE STRIPE TAX
            });

            invoice = await new InvoiceService().FinalizeInvoiceAsync(invoice.Id);

            // STEP 5: Pay the finalized invoice
            Console.WriteLine("5. Paying invoice using saved card...");
            invoice = await new InvoiceService().PayAsync(invoice.Id);

            Console.WriteLine("\n===========================================================");
            Console.WriteLine("SUCCESS! One-Time Invoice with Tax Breakdown:");
            Console.WriteLine($"  Invoice ID       : {invoice.Id}");
            Console.WriteLine($"  Status           : {invoice.Status}");
            Console.WriteLine($"  Subtotal (base)  : ${invoice.Subtotal / 100.0:F2}");
            Console.WriteLine($"  Tax (exclusive)  : ${(invoice.Total - invoice.Subtotal) / 100.0:F2}");
            Console.WriteLine($"  Total (incl tax) : ${invoice.Total / 100.0:F2}");
            Console.WriteLine($"  AutoTax Status   : {invoice.AutomaticTax?.Status}");
            Console.WriteLine("===========================================================\n");

            if (invoice.AutomaticTax?.Status == "requires_location_inputs")
            {
                Console.WriteLine(">>> Tax status is 'requires_location_inputs'.");
                Console.WriteLine(">>> Please enable Stripe Tax in Dashboard > Settings > Tax.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
