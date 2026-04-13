using System;
using System.Threading.Tasks;
using Stripe;

class Program
{
    static async Task Main()
    {
        StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");

        try
        {
            Console.WriteLine("=== FLOW 5: REFUND PREVIOUS PAYMENT ===");
            Console.WriteLine("Goal: Find a previously succeeded transaction and refund it automatically.\n");

            Console.WriteLine("1. Instantly creating and charging a test Customer $30.00...");
             var customer = await new CustomerService().CreateAsync(new CustomerCreateOptions { PaymentMethod = "pm_card_visa", InvoiceSettings = new CustomerInvoiceSettingsOptions { DefaultPaymentMethod = "pm_card_visa" } });
            var paymentIntent = await new PaymentIntentService().CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = 3000, Currency = "usd", Customer = customer.Id, PaymentMethod = "pm_card_visa", OffSession = true, Confirm = true
            });
            Console.WriteLine($"   Payment {paymentIntent.Id} Successful!");

            Console.WriteLine("\n2. Executing Refund...");
            var refund = await new RefundService().CreateAsync(new RefundCreateOptions
            {
                PaymentIntent = paymentIntent.Id
                // You can optionally pass 'Amount' to do a partial refund!
            });

            Console.WriteLine("\n===========================================================");
            Console.WriteLine("SUCCESS!");
            Console.WriteLine($"Refund ID: {refund.Id}");
            Console.WriteLine($"Status: {refund.Status}");
            Console.WriteLine("===========================================================\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
