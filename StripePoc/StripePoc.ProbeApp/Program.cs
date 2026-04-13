using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Stripe;
using Stripe.Checkout;

namespace ProbeApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var typesToProbe = new List<Type>();
            var stripeAssembly = typeof(Stripe.Invoice).Assembly;
            var checkoutAssembly = typeof(Stripe.Checkout.Session).Assembly;

            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    Type t = stripeAssembly.GetType("Stripe." + arg) ?? 
                             checkoutAssembly.GetType("Stripe.Checkout." + arg) ??
                             stripeAssembly.ExportedTypes.FirstOrDefault(et => et.Name == arg);
                    
                    if (t != null) typesToProbe.Add(t);
                    else Console.WriteLine($"Warning: Type {arg} not found.");
                }
            }

            if (!typesToProbe.Any())
            {
                typesToProbe.AddRange(new List<Type>
                {
                    typeof(Invoice),
                    typeof(PaymentIntent),
                    typeof(Subscription),
                    typeof(Charge),
                    typeof(Refund),
                    typeof(Customer),
                    typeof(PaymentMethod),
                    typeof(SetupIntent),
                    typeof(Payout),
                    typeof(Transfer),
                    typeof(Balance),
                    typeof(Dispute),
                    typeof(Session),
                    typeof(PromotionCode),
                    typeof(Coupon),
                    typeof(Source)
                });
            }

            var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Stripe_Event_Structures.md");
            using (var writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("# Stripe Event Structure Analysis");
                writer.WriteLine("\nGenerated on: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                writer.WriteLine("\nThis document lists the properties of common Stripe objects used in webhook events, filtered for identity, financial data, and statuses.");

                foreach (var type in typesToProbe)
                {
                    writer.WriteLine($"\n## {type.Name}");
                    writer.WriteLine("\n| Property | Type | Details |");
                    writer.WriteLine("| :--- | :--- | :--- |");

                    var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .OrderBy(p => p.Name);

                    foreach (var prop in props)
                    {
                        string typeName = GetFormattedTypeName(prop.PropertyType);
                        writer.WriteLine($"| {prop.Name} | {typeName} | {(IsComplex(prop.PropertyType) ? "Object" : "Value")} |");
                    }
                }
            }

            Console.WriteLine($"\nDocumentation generated successfully at: {Path.GetFullPath(outputPath)}");
        }

        private static bool IsRelevant(string name)
        {
            return name.Contains("id") || name.Contains("amount") || name.Contains("currency") || 
                   name.Contains("status") || name.Contains("customer") || name.Contains("date") || 
                   name.Contains("created") || name.Contains("subscription") || name.Contains("invoice") ||
                   name.Contains("success") || name.Contains("period") || name.Contains("refund") ||
                   name.Contains("payment") || name.Contains("method") || name.Contains("meta") ||
                   name.Contains("type") || name.Contains("total") || name.Contains("count") ||
                   name.Contains("url");
        }

        private static string GetFormattedTypeName(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return $"{Nullable.GetUnderlyingType(type).Name}?";
            }
            return type.Name;
        }

        private static bool IsComplex(Type type)
        {
            return !type.IsPrimitive && type != typeof(string) && type != typeof(decimal) && 
                   type != typeof(DateTime) && type != typeof(DateTimeOffset) && 
                   !type.IsEnum && (Nullable.GetUnderlyingType(type) == null || !Nullable.GetUnderlyingType(type).IsPrimitive);
        }
    }
}
