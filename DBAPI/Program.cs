using Domain.Model;
using WebAPI.Data;
using WebAPI.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MongoDB context and repository
builder.Services.AddSingleton<GreenHouseContext>();
builder.Services.AddSingleton<GreenHouseRepository>();

var app = builder.Build();

// Seed the database
await SeedDatabaseAsync(app.Services, app.Logger);

async Task SeedDatabaseAsync(IServiceProvider services, ILogger logger)
{
    using var scope = services.CreateScope();
    var greenHouseRepository = scope.ServiceProvider.GetRequiredService<GreenHouseRepository>();

    // Check if collection is empty
    var greenHouses = await greenHouseRepository.GetAllAsync();
    if (!greenHouses.Any())
    {
        var initialData = new List<GreenHouse>
        {
            new GreenHouse("GreenHouse1", "First Green House", 25.0, 300.0, 400.0, 60.0, false)
            {
                Id = "1"
            },
            new GreenHouse("GreenHouse2", "Second Green House", 26.0, 320.0, 420.0, 65.0, true)
            {
                Id = "10"
            }
        };

        foreach (var greenHouse in initialData)
        {
            logger.LogInformation($"Adding GreenHouse: {greenHouse.GreenHouseName}");
            await greenHouseRepository.AddAsync(greenHouse);
        }
    }
    else
    {
        logger.LogInformation("GreenHouses collection is not empty, skipping seeding.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();