using System;
using System.Reflection;
using Stripe;

public class RefundOptionsProbe
{
    public static void Main()
    {
        Console.WriteLine("--- Stripe.RefundCreateOptions Properties ---");
        var type = typeof(RefundCreateOptions);
        foreach (var prop in type.GetProperties())
        {
            Console.WriteLine($"{prop.Name}");
        }
    }
}
