using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DatabaseTest
{
    public static class DatabaseConnection
    {
        // Connection needs ServerAddress
        //  Database Name
        //  Authentication, either Window or Database

        // The connection should be managed, aka opened then closed and flushed.
         
        // Should be able to add Commands with or without parameters.
        // Could use enums as an easy means

        // Should be able to access data from commands

        // Text against SQL injection

        // For now our database works exclusively as a Login
        // It should contain the username and password at a bare min.

        // Database should be static class with helper fuctions...

        // SQL Injection - Strecth Goal
        // Password Hasing - Stretch Goal
        // Personal Preferences - Stretch Goal
        // USer Image - Stretch Goal

        //public static void CheckLogin(string user, string pass) {
        //    try {
        //        using (SqlConnection conn = new SqlConnection()) {
        //            conn.ConnectionString = "Server=tcp:wipserver.database.windows.net,1433;" +
        //                "Initial Catalog=WIPUsers;" +
        //                "Persist Security Info=False;" +
        //                "User ID=WIPStudios;" +
        //                "Password=741stQRHMsNn14D5;" +
        //                "MultipleActiveResultSets=False;" +
        //                "Encrypt=True;" +
        //                "TrustServerCertificate=False;" +
        //                "Connection Timeout=30;";

        //            Console.WriteLine("Attempting to connect");
        //            conn.Open();
        //            Console.WriteLine("Connected to Azure SQL Server and Database");

        //            SqlCommand findUser = new SqlCommand("SELECT username from Users WHERE username = @name", conn);
        //            findUser.Parameters.AddWithValue("@name", user);
        //            using (SqlDataReader reader = findUser.ExecuteReader()) {
        //                if (reader.Read()) {
        //                    Console.WriteLine(reader[0]);
        //                }
        //            }
        //        }
        //    } catch(SqlException e) {
        //        Console.WriteLine("\n" + e.Message);
        //    }
            
        //}

        //public static void Connect() {
        //    using(SqlConnection conn = new SqlConnection(
        //        "Server=localhost\\SQLEXPRESS;" +
        //        "Database=AP;" +
        //        "Trusted_Connection=true;"))
        //    {
        //        Console.WriteLine("Attempting to connect");
        //        conn.Open();
        //        Console.WriteLine("Connected to Server and Database");


        //        SqlCommand command = new SqlCommand("SELECT * from [ap.series]", conn);
        //        using(SqlDataReader reader = command.ExecuteReader()) {
        //            while (reader.Read()) {
        //                //Console.WriteLine(reader[0]);

        //                for (int i = 0; i < reader.FieldCount; i++) {
        //                    Console.WriteLine(reader[i]);
        //                }

        //            }
        //        }

        //        // Database Connections must be authenticated
        //        // Windows authentication requires the OS to authenticate the the connection. This means Trusted_Connection must be true
        //        // Database authentication requires standard login, Username and Password

        //        // Oppening and Closing Connection is expensive, Windows creates a Connection Pool to eliminate speed
        //        // However if the connection is different in any way, a new connection must be made.

        //        //SqlCommand command = new SqlCommand("SELECT * FROM TableName", conn);
        //        // Variables can be used and must be indicated with an @ sign
        //        // You must then add a parameter of how to interpret the variable.


        //        //using (SqlDataReader reader = command.ExecuteReader()) {
        //        // Executes the command and returns a Sql Reader
        //        // A SQL reader is a tool that helps to understand the data returned from the query

        //        // while (reader.Read()) {
        //        //for(int i = 0; i < reader.FieldCount; i++) {
        //        //Console.WriteLine(reader[i]);
        //        //}

        //        //}
        //        //}


        //        //SqlCommand insertCommand = new SqlCommand("INSERT INTO TableName " +
        //        // "(FirstColumn, SecondColumn, ThirdColumn, ForthColumn) VALUES (@0, @1, @2, @3)", conn);
        //        // Add parameters to send the data

        //        // Sql Error and SQL exception
        //        // SQL exception is more common to use with a try catch

        //    }
        //}
    }
}
