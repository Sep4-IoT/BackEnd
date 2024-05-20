using System;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Domain.Model;

public class GreenHouseContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<GreenHouse> GreenHouses { get; set; }

    public GreenHouseContext(DbContextOptions<GreenHouseContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var relativePath = Path.Combine("..", "EfcDataAccess", "GreenHouseDatabase.db");
            var absolutePath = Path.GetFullPath(relativePath);

            // Log the resolved path for debugging
            Console.WriteLine($"Resolved database path: {absolutePath}");

            optionsBuilder.UseSqlite($"Data Source={absolutePath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GreenHouse>().HasKey(g => g.GreenHouseId);

        modelBuilder.Entity<GreenHouse>()
            .HasOne(g => g.User)
            .WithMany(u => u.GreenHouses)
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>().HasKey(u => u.UserId);
    }
}