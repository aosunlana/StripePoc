using System;
using System.Reflection;
using Stripe;

public class InvoicePaymentProbe
{
    public static void Main()
    {
        Console.WriteLine("--- Stripe.InvoicePayment Properties ---");
        foreach (var prop in typeof(InvoicePayment).GetProperties())
            Console.WriteLine($"{prop.Name} ({prop.PropertyType.Name})");

        Console.WriteLine("\n--- Stripe.InvoicePaymentPayment Properties ---");
        foreach (var prop in typeof(InvoicePaymentPayment).GetProperties())
            Console.WriteLine($"{prop.Name} ({prop.PropertyType.Name})");
    }
}
