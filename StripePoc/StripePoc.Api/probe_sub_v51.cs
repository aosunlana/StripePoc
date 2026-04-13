using System;
using System.Reflection;
using Stripe;

public class SubscriptionV51Probe
{
    public static void Main()
    {
        Console.WriteLine("--- Stripe.Subscription Properties (v51 check) ---");
        var type = typeof(Subscription);
        foreach (var prop in type.GetProperties())
        {
            if (prop.Name.ToLower().Contains("invoice"))
                Console.WriteLine($"{prop.Name} ({prop.PropertyType.Name})");
        }
    }
}
