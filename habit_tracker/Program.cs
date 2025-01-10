using System;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Globalization;

namespace habit_tracker
{
    class Program
    {
        static string connectionString = @"Data Source=habit_tracker.db";
        static void Main(string[] args)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = 
                    @"CREATE TABLE IF NOT EXISTS drinking_water (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Day TEXT,
                        Quantity INTEGER               
                        )";
                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
            
            GetUserInput();
        }

        static void GetUserInput()
        {
            Console.Clear();
            bool closeApp = false;
            while (closeApp == false)
            {
                Console.WriteLine("\n\nMAIN MENU");
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("\nType 0 to close application");
                Console.WriteLine("Type 1 to view all records");
                Console.WriteLine("Type 2 to insert record");
                Console.WriteLine("Type 3 to update record");
                Console.WriteLine("Type 4 to delete record");
                Console.WriteLine("-------------------------------------\n");

                String command = Console.ReadLine();

                switch (command)
                {
                    case "0":
                        Console.WriteLine("\nGoodbye!\n");
                        closeApp = true;
                        Environment.Exit(0);
                        break;
                    case "1":
                        GetAllRecords();
                        break;
                    case "2":
                        Insert();
                        break;
                    case "3":
                        Update();
                        break;
                    case "4":
                        Delete();
                        break;
                    default:
                        Console.WriteLine("\nInvalid command. Please type a number 0-4\n");
                        break;
                }
            }
        }

        private static void GetAllRecords()
        {
            Console.Clear();
            
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"SELECT * FROM drinking_water";
                List<DrinkingWater> tableData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                        new DrinkingWater
                        {
                           Id = reader.GetInt32(0),
                           Day = DateTime.ParseExact(reader.GetString(1), "mm-dd-yy", new CultureInfo("en-US")),
                           Quantity = reader.GetInt32(2)
                        });
                    }
                }
                else
                {
                    Console.WriteLine("No rows found");
                }
        
                connection.Close();
                
                Console.WriteLine("\n------------------------------------");
                foreach (var dw in tableData)
                {
                    Console.WriteLine($"{dw.Id} - {dw.Day.ToString("d")} - Quantity: {dw.Quantity}");
                }
                Console.WriteLine("-------------------------------------\n");
            }
        }

        private static void Insert()
        {
            string date = GetDateInput();

            int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choice (no decimals)\n\n");
            
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"INSERT INTO drinking_water(day, quantity) VALUES ('{date}','{quantity}')";
                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        internal static void Update()
        {
            GetAllRecords();
            
            var recordId =
                GetNumberInput(
                    "\n\nPlease type the number of the record you want to update or press 0 to go to the main menu");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = '{recordId}')";
                int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (checkQuery == 0)
                {
                    Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist\n\n");
                    connection.Close();
                    Update();
                }

                string date = GetDateInput();
                int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choice (no decimals)\n\n");

                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"UPDATE drinking_water SET Day = '{date}', Quantity = '{quantity}' WHERE Id = '{recordId}'";
                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        private static void Delete()
        {
            Console.Clear();
            GetAllRecords();

            var recordId =
                GetNumberInput(
                    "\n\nPlease type the number of the record you want to delete or press 0 to go to the main menu");
            
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"DELETE FROM drinking_water WHERE Id = '{recordId}'";
               
                int rowCount = tableCmd.ExecuteNonQuery();

                if (rowCount == 0)
                {
                    Console.WriteLine($"\n\nRecord with Id '{recordId}' does not exist \n\n");
                    Delete();
                }

                connection.Close();
            }
        }

        internal static string GetDateInput()
        {
            Console.WriteLine("\n\nPlease insert the date: (Format: mm-dd-yy). Type 0 to return to the main menu.");
           
            string dateInput = Console.ReadLine();
            
            if (dateInput == "0")
            {
                GetUserInput();
            }

            while (!DateTime.TryParseExact(dateInput, "d", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                Console.WriteLine("\n\nInvalid date. (Format: dd-mm-yy). Type 0 to return to main menu or try again\n\n");
                dateInput = Console.ReadLine();
            }

            return dateInput;
        }

        internal static int GetNumberInput(string message)
        {
            Console.WriteLine(message);
            string numberInput = Console.ReadLine();
            if (numberInput == "0")
            {
                GetUserInput();
            }

            while (!Int32.TryParse(numberInput, out _) || Convert.ToInt32(numberInput) < 0)
            {
                Console.WriteLine("\n\nInvalid number. Try again\n\n");
                numberInput = Console.ReadLine();
            }

            int finalInput = Convert.ToInt32(numberInput);

            return finalInput;
        }
    }

    public class DrinkingWater
    {
        public int Id { get; set; }
        
        public DateTime Day { get; set; } 
        
        public int Quantity { get; set; }
    }
    
}

