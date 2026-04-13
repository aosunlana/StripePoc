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
            Console.WriteLine("=== FLOW: CONNECT V2 FULL LIFECYCLE (Modern Platform Model) ===");
            Console.WriteLine("Goal: Register Clinic using V2 API -> Onboard -> Patient pays Clinic directly.\n");

            // 1. Create a Connected Account using the Accounts v2 API (Controller properties)
            Console.WriteLine("1. Creating Unified Account with v2 Controller config...");
            
            // In v2, you define the 'controller' to specify how Stripe handles the account.
            // For non-techy users, we want Stripe to handle onboarding and dashboards.
            var accountOptions = new AccountCreateOptions
            {
                Email = "v2.clinic@example.com",
                Country = "US",
                BusinessProfile = new AccountBusinessProfileOptions { Name = "V2 Modern Wellness" },
                Capabilities = new AccountCapabilitiesOptions
                {
                    CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                    Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
                },
                // THE V2 PROPERTY: This replaces the legacy 'express' vs 'custom' focus.
                // It explicitly tells Stripe: "The Platform manages this account, but Stripe collects data."
                Controller = new AccountControllerOptions
                {
                    StripeDashboard = new AccountControllerStripeDashboardOptions { Type = "express" }, // Let them use the light portal
                    RequirementCollection = "stripe", // Stripe handles the tedious legal forms
                    Losses = new AccountControllerLossesOptions { Payments = "application" }, // Platform is liable for fraud
                    Fees = new AccountControllerFeesOptions { Payer = "application" } // Platform pays Stripe's internal fees
                }
            };

            var account = await new AccountService().CreateAsync(accountOptions);
            Console.WriteLine($"   Account Created: {account.Id}");
            Console.WriteLine($"   Controller Payer: {account.Controller?.Fees?.Payer}\n");

            // 2. Generate Onboarding Link (Modern compatible)
            Console.WriteLine("2. Generating Onboarding Link (Connect v2 Compatible)...");
            var linkOptions = new AccountLinkCreateOptions
            {
                Account = account.Id,
                RefreshUrl = "https://example.com/refresh",
                ReturnUrl = "https://example.com/return",
                Type = "account_onboarding"
            };
            var accountLink = await new AccountLinkService().CreateAsync(linkOptions);
            
            Console.WriteLine("\n===========================================================");
            Console.WriteLine("ACTION REQUIRED: ONBOARD THE V2 CLINIC");
            Console.WriteLine("===========================================================");
            Console.WriteLine("1. Open this URL in your browser:");
            Console.WriteLine($"   {accountLink.Url}");
            Console.WriteLine("2. Complete the form (Choose 'Skip for now').");
            Console.WriteLine("3. Come back here and press ENTER once you have finished.");
            Console.WriteLine("===========================================================\n");

            Console.ReadLine(); // Wait for user

            // 3. Verify Account Status
            Console.WriteLine("3. Verifying Clinic status using v2 status fields...");
            account = await new AccountService().GetAsync(account.Id);
            Console.WriteLine($"   Verification Status: {account.DetailsSubmitted}");
            Console.WriteLine($"   Charges Enabled: {account.ChargesEnabled}");

            if (!account.ChargesEnabled)
            {
                Console.WriteLine("\n[WARNING] Account is not yet ready. Proceeding with Payment Test...");
            }

            // 4. Create a Direct Charge (The Platform Model)
            Console.WriteLine("\n4. Executing Direct Charge (Platform Model)...");
            
            var piOptions = new PaymentIntentCreateOptions
            {
                Amount = 25000, // $250.00
                Currency = "usd",
                Description = "Service Payment via Connect V2",
                PaymentMethod = "pm_card_visa",
                Confirm = true,
                // Setting zero fee explicitly as requested
                ApplicationFeeAmount = 0, 
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true, AllowRedirects = "never" }
            };

            // Use RequestOptions to act on behalf of the connected account
            var requestOptions = new RequestOptions { StripeAccount = account.Id };
            
            var piService = new PaymentIntentService();
            var paymentIntent = await piService.CreateAsync(piOptions, requestOptions);

            Console.WriteLine("\n===========================================================");
            Console.WriteLine("SUCCESS! V2 LIFECYCLE COMPLETE.");
            Console.WriteLine($"PaymentIntent ID: {paymentIntent.Id}");
            Console.WriteLine($"Status: {paymentIntent.Status}");
            Console.WriteLine($"Clinic Received (Gross): ${paymentIntent.Amount / 100.0:F2}");
            Console.WriteLine($"Platform took: $0.00");
            Console.WriteLine("\nVIEW IN DASHBOARD:");
            Console.WriteLine($"https://dashboard.stripe.com/test/connect/accounts/{account.Id}");
            Console.WriteLine("===========================================================\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
