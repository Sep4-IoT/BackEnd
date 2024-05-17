using System;
using System.IO;
using System.Reflection;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess
{
    public class GreenHouseContext : DbContext
    {
        public DbSet<GreenHouse> GreenHouses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use the directory of the executing assembly as the base directory
            string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dbDirectory = Path.Combine(baseDirectory, "EfcDataAccess");

            // Ensure the directory exists
            if (!Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }

            string dbPath = Path.Combine(dbDirectory, "GreenHouseDatabase.db");

            // Print the database path for debugging
            Console.WriteLine($"Database path: {dbPath}");

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GreenHouse>().HasKey(g => g.GreenHouseId);
        }

        public void PrintDatabasePath()
        {
            string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dbDirectory = Path.Combine(baseDirectory, "EfcDataAccess");
            string dbPath = Path.Combine(dbDirectory, "GreenHouseDatabase.db");
            Console.WriteLine($"Using database path: {dbPath}");
        }
    }
}