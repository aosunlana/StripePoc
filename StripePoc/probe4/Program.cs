using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe;

class Program
{
    static async Task Main(string[] args)
    {
        // Using your exact Test Secret Key
        StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");

        try
        {
            Console.WriteLine("1. Creating Customer with Email...");
            var customerOptions = new CustomerCreateOptions { Email = "aged.customer@examplestripe.com" };
            var customer = await new CustomerService().CreateAsync(customerOptions);
            Console.WriteLine($"   Customer created: {customer.Id}\n");

            Console.WriteLine("2. Creating the Service Price...");
            var price = await new PriceService().CreateAsync(new PriceCreateOptions
            {
                Currency = "usd",
                UnitAmount = 1500, // $15.00
                Recurring = new PriceRecurringOptions { Interval = "week" },
                ProductData = new PriceProductDataOptions { Name = "Weekly Nursing Care" }
            });
            Console.WriteLine($"   Price created: {price.Id}\n");

            Console.WriteLine("3. Employee Initiating Subscription (Send Invoice to Customer)...");
            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                Items = new List<SubscriptionItemOptions> { new SubscriptionItemOptions { Price = price.Id } },
                CollectionMethod = "send_invoice",
                DaysUntilDue = 3, // Customer has 3 days to approve/pay
                Expand = new List<string> { "latest_invoice" } // Need to get the generated invoice!
            };

            var subscription = await new SubscriptionService().CreateAsync(subscriptionOptions);
            Console.WriteLine($"   Subscription created: {subscription.Id}");
            Console.WriteLine($"   Status: {subscription.Status} (Notice it is incomplete/past_due because it's unpaid!)\n");

            Console.WriteLine("4. Retrieving the Customer's Email Invoice Link...");
            var latestInvoice = subscription.LatestInvoice; 
            
            if (latestInvoice != null)
            {
                var invoiceService = new InvoiceService();
                
                // Finalize the invoice to ensure it can be paid by the customer
                // (Usually Stripe auto-finalizes initial subscription invoices, but let's be sure or send it)
                if (latestInvoice.Status == "draft")
                {
                   latestInvoice = await invoiceService.FinalizeInvoiceAsync(latestInvoice.Id);
                }

                // Stripe has a special method to manually trigger the "Send Invoice" email in an API call!
                Console.WriteLine("   Triggering Stripe to Send Email to Customer...");
                latestInvoice = await invoiceService.SendInvoiceAsync(latestInvoice.Id);
                
                Console.WriteLine("\n===========================================================");
                Console.WriteLine("SUCCESS! Stripe has queued the invoice email.");
                Console.WriteLine($"The customer's email would contain this Exact Payment Link:");
                Console.WriteLine(latestInvoice.HostedInvoiceUrl);
                Console.WriteLine("===========================================================\n");
            }
            else
            {
                Console.WriteLine("   No latest invoice found!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
