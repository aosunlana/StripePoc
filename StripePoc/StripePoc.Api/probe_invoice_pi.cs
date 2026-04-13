using System;
using System.Reflection;
using Stripe;

public class InvoiceProbe
{
    public static void Main()
    {
        Console.WriteLine("--- Stripe.Invoice Properties (payment-related) ---");
        var type = typeof(Invoice);
        foreach (var prop in type.GetProperties())
        {
            var name = prop.Name.ToLower();
            if (name.Contains("payment") || name.Contains("charge") || name.Contains("intent") || name.Contains("parent"))
                Console.WriteLine($"{prop.Name} ({prop.PropertyType.Name})");
        }
    }
}
