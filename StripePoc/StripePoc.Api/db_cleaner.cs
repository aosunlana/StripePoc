using System;
using Microsoft.Data.Sqlite;

public class DbCleaner
{
    public static void Main()
    {
        var dbPath = @"C:\Users\osunl\source\repos\StripePoc\StripePoc.Api\stripe_poc_final.db";
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();

        // Clear in dependency order (child tables first)
        var commands = new[]
        {
            "DELETE FROM Payments;",
            "DELETE FROM PaymentIntents;",
            "DELETE FROM Subscriptions;",
            "DELETE FROM PaymentMethods;",
            "DELETE FROM PaymentAccounts;",
        };

        foreach (var cmdText in commands)
        {
            using var command = connection.CreateCommand();
            command.CommandText = cmdText;
            int rows = command.ExecuteNonQuery();
            Console.WriteLine($"✓ {cmdText.PadRight(35)} Rows cleared: {rows}");
        }

        Console.WriteLine("\n✅ Database ready for demo! Accounts and Businesses preserved.");
    }
}
