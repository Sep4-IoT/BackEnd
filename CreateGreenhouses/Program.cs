using System;
using System.Linq;
using EfcDataAccess;
using Domain.Model;

public class Program
{
    public static void Main(string[] args)
    {
        // Create a new instance of the DbContext
        using (var context = new GreenHouseContext())
        {
            // Ensure the database is created
            context.Database.EnsureCreated();

            // Create two GreenHouse objects
            var greenHouse1 = new GreenHouse
            {
                GreenHouseName = "Tropical Paradise",
                Description = "A greenhouse for tropical plants",
                Temperature = 25.5,
                LightIntensity = 75.0,
                Co2Levels = 400.0,
                Humidity = 80.0,
                IsWindowOpen = false
            };

            var greenHouse2 = new GreenHouse
            {
                GreenHouseName = "Desert Oasis",
                Description = "A greenhouse for desert plants",
                Temperature = 30.0,
                LightIntensity = 90.0,
                Co2Levels = 350.0,
                Humidity = 20.0,
                IsWindowOpen = true
            };

            // Add the GreenHouse objects to the DbContext
            context.GreenHouses.Add(greenHouse1);
            context.GreenHouses.Add(greenHouse2);

            // Save the changes to the database
            context.SaveChanges();

            // Output the assigned IDs for debugging
            Console.WriteLine($"Greenhouse 1 ID: {greenHouse1.GreenHouseId}");
            Console.WriteLine($"Greenhouse 2 ID: {greenHouse2.GreenHouseId}");

            // Query and print all GreenHouses in the database
            var greenHouses = context.GreenHouses.ToList();
            Console.WriteLine("Greenhouses in the database:");
            foreach (var gh in greenHouses)
            {
                Console.WriteLine($"ID: {gh.GreenHouseId}, Name: {gh.GreenHouseName}, Description: {gh.Description}, Temperature: {gh.Temperature}, Light Intensity: {gh.LightIntensity}, CO2 Levels: {gh.Co2Levels}, Humidity: {gh.Humidity}, Is Window Open: {gh.IsWindowOpen}");
            }
        }

        Console.WriteLine("Greenhouses have been added to the database and printed.");
    }
}
