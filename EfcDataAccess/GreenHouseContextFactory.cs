using EfcDataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class GreenHouseContextFactory : IDesignTimeDbContextFactory<GreenHouseContext>
{
    public GreenHouseContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GreenHouseContext>();
        optionsBuilder.UseSqlite("Data Source = ../EfcDataAccess/GreenHouseDatabase.db");

        var context = new GreenHouseContext(optionsBuilder.Options);

        // Test connection
        try
        {
            context.Database.OpenConnection();
            context.Database.CloseConnection();
            Console.WriteLine("Database connection successful.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database connection failed: {ex.Message}");
        }

        return context;
    }
}