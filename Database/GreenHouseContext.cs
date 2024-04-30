using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class GreenHouseContext
{
    public DbSet<GreenHouse> Greenhouses { get; set; }
    public DbSet<Window> Windows { get; set; }
    
    public string DbPath { get; }

    public GreenHouseContext(){
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "App.db"); 
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}