using Domain.Model;
using WebAPI.Data;
using WebAPI.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MongoDB context and repository
builder.Services.AddSingleton<GreenHouseContext>();
builder.Services.AddSingleton<GreenHouseDateListRepository>();

// Add HttpClient for IOTController
builder.Services.AddHttpClient("IOTController", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["IOTControllerBaseUrl"]);
});

var app = builder.Build();

// Seed the database
await SeedDatabaseAsync(app.Services, app.Logger);

async Task SeedDatabaseAsync(IServiceProvider services, ILogger logger)
{
    using var scope = services.CreateScope();
    var greenHouseRepository = scope.ServiceProvider.GetRequiredService<GreenHouseDateListRepository>();

    var initialData = new List<GreenHouseDateList>
    {
        new GreenHouseDateList("1")
        {
            GreenHouses = new List<GreenHouse>
            {
                new GreenHouse("GreenHouse1", "First Green House", 24.0, 250.0, 500.0, 2.0, true, DateTime.UtcNow.AddMinutes(-10))
                {
                    GreenHouseId = "1"
                }, 
                new GreenHouse("GreenHouse1", "First Green House", 25.5, 310.0, 410.0, 61.0, false, DateTime.UtcNow)
                {
                    GreenHouseId = "1"
                },
            }
        },
        new GreenHouseDateList("10")
        {
            GreenHouses = new List<GreenHouse>
            {
                new GreenHouse("GreenHouse2", "Second Green House", 26.0, 320.0, 420.0, 65.0, true, DateTime.UtcNow)
                {
                    GreenHouseId = "10"
                }
            }
        }
    };

    foreach (var greenHouseDateList in initialData)
    {
        logger.LogInformation($"Adding GreenHouseDateList: {greenHouseDateList.Id}");
        await greenHouseRepository.AddAsync(greenHouseDateList);
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

// Simulate adding new data every 10 minutes by duplicating the earliest data
var timer = new System.Threading.Timer(async (e) =>
{
    using var scope = app.Services.CreateScope();
    var greenHouseRepository = scope.ServiceProvider.GetRequiredService<GreenHouseDateListRepository>();

    foreach (var id in new[] { "1", "10" })
    {
        var greenHouseDateList = await greenHouseRepository.GetByIdAsync(id);
        if (greenHouseDateList != null)
        {
            var earliestGreenHouse = greenHouseDateList.GreenHouses.OrderBy(gh => gh.Date).FirstOrDefault();
            if (earliestGreenHouse != null)
            {
                var newGreenHouse = new GreenHouse(
                    earliestGreenHouse.GreenHouseName,
                    earliestGreenHouse.Description,
                    earliestGreenHouse.Temperature,
                    earliestGreenHouse.LightIntensity,
                    earliestGreenHouse.Co2Levels,
                    earliestGreenHouse.Humidity,
                    earliestGreenHouse.IsWindowOpen,
                    DateTime.UtcNow
                )
                {
                    GreenHouseId = earliestGreenHouse.GreenHouseId
                };

                greenHouseDateList.GreenHouses.Add(newGreenHouse);
                await greenHouseRepository.UpdateAsync(greenHouseDateList);
            }
        }
    }
}, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
