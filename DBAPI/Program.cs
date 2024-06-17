using Domain.Model;
using WebAPI.Data;
using WebAPI.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MongoDB context and repository
builder.Services.AddSingleton<GreenHouseContext>();
builder.Services.AddSingleton<GreenHouseDateListRepository>();

// Register TimerService
builder.Services.AddHostedService<TimerService>();

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

    // Check if the database already has data
    var existingData = await greenHouseRepository.GetAllAsync();
    if (existingData.Any())
    {
        logger.LogInformation("Database already contains initial data, skipping seeding.");
        return;
    }

    var initialData = new List<GreenHouseDateList>
    {
        new GreenHouseDateList(1)
        {
            GreenHouses = new List<GreenHouse>
            {
                new GreenHouse("GreenHouse1", "First Green House", null, null, null, null, null, DateTime.UtcNow)
                {
                    GreenHouseId = 1
                },
            }
        },
        new GreenHouseDateList(10)
        {
            GreenHouses = new List<GreenHouse>
            {
                new GreenHouse("GreenHouse2", "Second Green House", null, null, null, null, null, DateTime.UtcNow)
                {
                    GreenHouseId = 10
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
