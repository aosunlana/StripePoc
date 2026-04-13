using System;
using System.Reflection;
using Stripe;

public class SubscriptionV51FullProbe
{
    public static void Main()
    {
        Console.WriteLine("--- Stripe.Subscription Full Property List ---");
        foreach (var prop in typeof(Subscription).GetProperties())
        {
            Console.WriteLine($"{prop.Name} ({prop.PropertyType.Name})");
        }
    }
}
