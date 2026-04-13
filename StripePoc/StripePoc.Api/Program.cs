using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using StripePoc.Api.Data;
using StripePoc.Api.Brokers.DateTimes;
using StripePoc.Api.Brokers.Loggings;
using StripePoc.Api.Brokers.Payments;
using StripePoc.Api.Brokers.Storages;
using StripePoc.Api.Services.Foundations.PaymentAccounts;
using StripePoc.Api.Services.Foundations.PaymentMethods;
using StripePoc.Api.Services.Foundations.Subscriptions;
using StripePoc.Api.Services.Foundations.PaymentIntents;
using StripePoc.Api.Services.Foundations.Payments;
using StripePoc.Api.Services.Foundations.StripePayments;
using StripePoc.Api.Services.Orchestrations.AccountSetups;
using StripePoc.Api.Services.Orchestrations.Subscriptions;
using StripePoc.Api.Services.Orchestrations.OneTimePayments;
using StripePoc.Api.Services.Orchestrations.Webhooks;
using StripePoc.Api.Services.Foundations.Businesses;
using StripePoc.Api.Services.Foundations.Accounts;
using StripePoc.Api.Services.Orchestrations.Accounts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Brokers
builder.Services.AddScoped<IPaymentBroker, PaymentBroker>();
builder.Services.AddScoped<ILoggingBroker, LoggingBroker>();
builder.Services.AddScoped<IDateTimeBroker, DateTimeBroker>();
// Database Configuration
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? builder.Configuration["DATABASE_URL"];

builder.Services.AddDbContext<IStorageBroker, StorageBroker>(options =>
{
    if (!string.IsNullOrEmpty(connectionString) && 
        (connectionString.StartsWith("postgres://") || connectionString.Contains("Host=")))
    {
        // Handle Render's DATABASE_URL format if necessary (PostgreSQL)
        options.UseNpgsql(connectionString);
    }
    else
    {
        // Local fallback (SQLite)
        options.UseSqlite(string.IsNullOrEmpty(connectionString) ? "Data Source=stripe_poc_v3.db" : connectionString);
    }
});

// Foundation Services
builder.Services.AddScoped<IPaymentAccountService, PaymentAccountService>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IPaymentIntentService, PaymentIntentService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IStripePaymentService, StripePaymentService>();
builder.Services.AddScoped<IBusinessService, BusinessService>();
builder.Services.AddScoped<IAccountService, AccountService>();

// Orchestration Services
builder.Services.AddScoped<IAccountSetupOrchestrationService, AccountSetupOrchestrationService>();
builder.Services.AddScoped<ISubscriptionOrchestrationService, SubscriptionOrchestrationService>();
builder.Services.AddScoped<IOneTimePaymentOrchestrationService, OneTimePaymentOrchestrationService>();
builder.Services.AddScoped<IWebhookOrchestrationService, WebhookOrchestrationService>();
builder.Services.AddScoped<IAccountOrchestrationService, AccountOrchestrationService>();

// Enable CORS for Blazor WASM
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorCors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("BlazorCors");

// Seed Database
await DatabaseSeeder.SeedAsync(app.Services);

app.UseAuthorization();
app.MapControllers();

app.Run();
