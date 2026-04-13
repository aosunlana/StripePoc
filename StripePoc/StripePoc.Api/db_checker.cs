using System;
using Microsoft.Data.Sqlite;

public class DbChecker
{
    public static void Main()
    {
        var dbPath = @"C:\Users\osunl\source\repos\StripePoc\StripePoc.Api\stripe_poc_final.db";
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Payments";
        var count = command.ExecuteScalar();
        Console.WriteLine($"Total Payments: {count}");

        command.CommandText = "SELECT * FROM Payments ORDER BY PaidAt DESC LIMIT 5";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine($"Payment: {reader["Id"]}, Amount: {reader["Amount"]}, Status: {reader["Status"]}, Date: {reader["PaidAt"]}");
        }
    }
}
