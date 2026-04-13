using System;
using System.Reflection;
using Stripe;

public class ParentProbe
{
    public static void Main()
    {
        Console.WriteLine("--- Stripe.InvoiceParent Properties ---");
        var type = typeof(InvoiceParent);
        foreach (var prop in type.GetProperties())
        {
            Console.WriteLine($"{prop.Name} ({prop.PropertyType.Name})");
        }
        
        Console.WriteLine("\n--- Stripe.InvoiceParentSubscriptionDetails Properties ---");
        foreach (var prop in typeof(InvoiceParentSubscriptionDetails).GetProperties())
        {
            Console.WriteLine($"{prop.Name} ({prop.PropertyType.Name})");
        }
    }
}
