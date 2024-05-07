using Application.DAOInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.Model;
using FileData;
using FileData.DAOs;
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
builder.Services.AddScoped<IGreenHouseDAO, GreenHouseFileDAO>();

builder.Services.AddSingleton<List<User>>();
builder.Services.AddScoped<IUserLogic, UserLogic>();
builder.Services.AddScoped<IUserDAO, UserFileDAO>();

builder.Services.AddScoped<FileContext>();
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

app.Run();
