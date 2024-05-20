using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;

public class GreenHouseContextFactory : IDesignTimeDbContextFactory<GreenHouseContext>
{
    public GreenHouseContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GreenHouseContext>();

        var relativePath = Path.Combine("..", "EfcDataAccess", "GreenHouseDatabase.db");
        var absolutePath = Path.GetFullPath(relativePath);

        // Log the resolved path for debugging
        Console.WriteLine($"Resolved database path: {absolutePath}");

        optionsBuilder.UseSqlite($"Data Source={absolutePath}");

        return new GreenHouseContext(optionsBuilder.Options);
    }
}
