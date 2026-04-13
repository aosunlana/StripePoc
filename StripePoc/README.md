# Stripe POC Deployment & Setup Guide

This guide covers how to set up your Stripe Dashboard for this POC, and how to deploy the Backend to Render and the Frontend to Vercel.

## 1. Stripe Dashboard Setup

### A. API Keys
1. Go to your [Stripe Dashboard](https://dashboard.stripe.com/).
2. Toggle **Test mode** on in the top right.
3. Go to **Developers** -> **API keys**.
4. Note your **Publishable key** (`pk_test_...`) and **Secret key** (`sk_test_...`).
   - The **Publishable key** goes into your Blazor App (e.g., in `CheckoutComponent.razor` or a config file).
   - The **Secret key** goes into your API's `appsettings.json` or Environment Variables.

### B. Webhooks
Your ASP.NET Core API needs to listen to events from Stripe. 
1. In the Stripe Dashboard, go to **Developers** -> **Webhooks**.
2. Click **Add an endpoint**.
3. For local testing, use the [Stripe CLI](https://stripe.com/docs/stripe-cli):
   ```bash
   stripe listen --forward-to https://localhost:7128/api/webhooks/stripe
   ```
   The CLI will print a **Webhook Signing Secret** (`whsec_...`). Add this to your API configuration (`Stripe:WebhookSecret`).
4. For production (Render), set the endpoint URL to `https://your-render-api-url.onrender.com/api/webhooks/stripe`.
   - Select the events to listen to:
     - `invoice.paid`
     - `invoice.payment_failed`
     - `payment_intent.succeeded`
     - `payment_intent.payment_failed`
     - `customer.subscription.deleted`
     - `charge.refunded`
   - Grab the **Webhook Signing Secret** from the dashboard and set it as an Environment Variable in Render.

## 2. Deploying the Backend (API) to Render

We use Docker to deploy the .NET 8 Web API.

1. Create a **New Web Service** on [Render](https://render.com/).
2. Connect your Git repository.
3. For the **Environment**, select **Docker**.
4. Ensure the **Dockerfile path** is set correctly (e.g., `StripePoc.Api/Dockerfile`).
5. **Environment Variables**:
   - Add `Stripe__SecretKey` = `sk_test_...`
   - Add `Stripe__WebhookSecret` = `whsec_...`
   - (Optional) `ASPNETCORE_ENVIRONMENT` = `Production`
6. Click **Create Web Service**. Render will build the Docker container and deploy the API.

> Note the URL of your deployed API (e.g., `https://stripe-poc-api.onrender.com`). You will need this for the Blazor frontend.

## 3. Deploying the Frontend (Blazor WebAssembly) to Vercel

Vercel is great for hosting static sites like Blazor WebAssembly.

1. Ensure `StripePoc.Blazor/Program.cs` is updated to point to your new Render API URL:
   ```csharp
   builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://your-render-api-url.onrender.com/") });
   ```
2. Make sure you have a `vercel.json` file in the `StripePoc.Blazor` root (we've created one for you).
3. Push your repository to GitHub.
4. Go to [Vercel](https://vercel.com/) and create a **New Project**.
5. Import your GitHub repository.
6. Configure the **Build & Development Settings**:
   - **Framework Preset**: Other
   - **Build Command**: `dotnet publish StripePoc.Blazor.csproj -c Release -o release`
   - **Output Directory**: `release/wwwroot`
   - **Install Command**: (Leave blank, .NET SDK is pre-installed on Vercel's build image, though you might need to specify a bash script to install .NET if Vercel defaults don't have .NET 8).
     - *If .NET 8 isn't available natively on Vercel, you can use an Install Command:* `curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh; chmod +x dotnet-install.sh; ./dotnet-install.sh -c 8.0 -InstallDir ./dotnet; export PATH="./dotnet:$PATH"` 
     - *And change Build Command to:* `./dotnet/dotnet publish StripePoc.Blazor.csproj -c Release -o release`
7. Click **Deploy**.

## 4. Testing End-to-End

1. Open your Vercel URL.
2. Select a Service and click **Confirm**.
3. Use a [Stripe Test Card](https://stripe.com/docs/testing) to complete the setup.
4. Go to your Stripe Dashboard -> **Customers** or **Payments** to see the created customer and intent.
5. Watch your API logs in Render to see the Webhook requests rolling in and updating the database.
