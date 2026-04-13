using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StripePoc.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Smart Address: Detect if we are local or on Render
string apiBaseUri = builder.HostEnvironment.BaseAddress.Contains("localhost") 
    ? "https://localhost:7265/" 
    : "https://stripe-poc-api.onrender.com/";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUri) });
builder.Services.AddScoped<StripePoc.Blazor.Services.ApiService>();

await builder.Build().RunAsync();
