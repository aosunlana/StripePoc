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
            Console.WriteLine("=== FLOW 1: WALLET SETUP ===");
            Console.WriteLine("Goal: Save a customer's credit card for future off-session charges without charging them right now.");

            Console.WriteLine("\n1. Creating Customer...");
            var customer = await new CustomerService().CreateAsync(new CustomerCreateOptions
            {
                Email = "customer.wallet@examplestripe.com",
                Description = "Customer saving card for Emerald Kilonova"
            });
            Console.WriteLine($"   Customer created: {customer.Id}");

            Console.WriteLine("2. Creating SetupIntent...");
            var setupIntent = await new SetupIntentService().CreateAsync(new SetupIntentCreateOptions
            {
                Customer = customer.Id,
                Usage = "off_session" // Vital for charging them later when they aren't on your website
            });
            Console.WriteLine($"   SetupIntent created: {setupIntent.Id}");

            Console.WriteLine("\n===========================================================");
            Console.WriteLine("SUCCESS!");
            Console.WriteLine("The Frontend would now use this ClientSecret to render the Stripe Elements Card Form safely:");
            Console.WriteLine(setupIntent.ClientSecret);
            Console.WriteLine("===========================================================\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
