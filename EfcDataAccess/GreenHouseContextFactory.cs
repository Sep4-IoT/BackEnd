using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class GreenHouseContextFactory : IDesignTimeDbContextFactory<GreenHouseContext>
{
    public GreenHouseContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GreenHouseContext>();
        var relativePath = Path.Combine("..", "EfcDataAccess", "GreenHouseDatabase.db");
        var absolutePath = Path.GetFullPath(relativePath);
        optionsBuilder.UseSqlite($"Data Source={absolutePath}");

        return new GreenHouseContext(optionsBuilder.Options);
    }
}