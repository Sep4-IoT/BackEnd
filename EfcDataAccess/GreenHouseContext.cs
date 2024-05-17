using System.IO;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess
{
    public class GreenHouseContext : DbContext
    {
        public DbSet<GreenHouse> GreenHouses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dbPath = Path.Combine(baseDirectory, "/root/Backend/EfcDataAccess/GreenHouseDatabase.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GreenHouse>().HasKey(g => g.GreenHouseId);
        }
    }
}