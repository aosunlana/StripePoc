using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe;

class Program
{
    static async Task Main()
    {
        // Using your exact Test Secret Key
        StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");

        try
        {
            Console.WriteLine("=== FLOW: STRIPE CONNECT ONBOARDING (Multi-tenant Registration) ===");
            Console.WriteLine("Goal: Abstract the registration for a 'Tenant' (e.g. a new Clinic/Business) in a multi-tenant app.\n");

            // STEP 1: Create a Connected Account for the Tenant
            // We use 'Express' because it's perfect for non-techy users: 
            // Stripe handles the identity verification but YOU control the UI.
            Console.WriteLine("1. Creating a Connected Account (Express) for a new Tenant...");
            var accountOptions = new AccountCreateOptions
            {
                Type = "express", 
                Country = "US",
                Email = "new.clinic.owner@example.com", // Pre-fill their email
                BusinessProfile = new AccountBusinessProfileOptions
                {
                    Name = "Emerald City Wellness Clinic" // Pre-fill their business name
                },
                Capabilities = new AccountCapabilitiesOptions
                {
                    CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                    Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
                }
            };

            var account = await new AccountService().CreateAsync(accountOptions);
            Console.WriteLine($"   Connected Account Created: {account.Id}");
            Console.WriteLine($"   Status: {account.DetailsSubmitted} (details_submitted = false means they haven't finished onboarding)\n");

            // STEP 2: Generate an Account Link (The Onboarding URL)
            // You take this URL and give it to the Tenant in your React app.
            Console.WriteLine("2. Generating an Account Link (The seamless onboarding URL)...");
            var linkOptions = new AccountLinkCreateOptions
            {
                Account = account.Id,
                RefreshUrl = "https://your-app.com/re-onboard", // Where to go if the link expires
                ReturnUrl = "https://your-app.com/onboarding-complete", // Where to go when they finish
                Type = "account_onboarding"
            };

            var accountLink = await new AccountLinkService().CreateAsync(linkOptions);
            
            Console.WriteLine("\n===========================================================");
            Console.WriteLine("SUCCESS! Your backend has 'abstracted' the account creation.");
            Console.WriteLine("\nTHE SEAMLESS EXPERIENCE:");
            Console.WriteLine("1. Your React app shows a button: 'Link your Business to get Paid'");
            Console.WriteLine("2. When clicked, it opens this secure, branded link:");
            Console.WriteLine($"\n{accountLink.Url}");
            Console.WriteLine("\n3. The non-techy user just follows the simple steps (ID, Bank info).");
            Console.WriteLine("4. They are automatically redirected back to your ReturnUrl when done.");
            Console.WriteLine("===========================================================\n");

            // STEP 3: Checking the status (In a real app, you'd do this via Webhooks)
            Console.WriteLine("3. Checking if the account is ready for payments...");
            var retrievedAccount = await new AccountService().GetAsync(account.Id);
            
            if (retrievedAccount.DetailsSubmitted)
            {
                Console.WriteLine("   Account is FULLY VERIFIED and ready to charge customers!");
            }
            else
            {
                Console.WriteLine("   Account still needs onboarding details before it can receive funds.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
