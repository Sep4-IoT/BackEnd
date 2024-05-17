
using Application.DAOInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.Model;
using EfcDataAccess;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
});

builder.Services.AddSingleton<List<GreenHouse>>();
builder.Services.AddScoped<IGreenHouseLogic, GreenHouseLogic>();
builder.Services.AddScoped<IGreenHouseDAO, GreenHouseEfcDAO>();

builder.Services.AddDbContext<GreenHouseContext>();

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run("http://0.0.0.0:5047");
