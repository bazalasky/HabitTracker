using System;
using System.Data;
using Microsoft.Data.Sqlite;
    
namespace habit_tracker
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=habit_tracker.db";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = 
                    @"CREATE TABLE IF NOT EXISTS drinking_water (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        day TEXT,
                        Quantity INTEGER               
                        )";
                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }
    }
}

