using System;
using System.Reflection;
using Stripe;

public class ChargeProbe
{
    public static void Main()
    {
        Console.WriteLine("--- Stripe.Charge Properties ---");
        var type = typeof(Charge);
        foreach (var prop in type.GetProperties())
        {
            Console.WriteLine($"{prop.Name} ({prop.PropertyType.Name})");
        }
    }
}
