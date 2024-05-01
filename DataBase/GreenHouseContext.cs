using Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Database
{
    public class GreenHouseContext : DbContext
    {
        public DbSet<GreenHouse> Greenhouses { get; set; }
        public string DbPath { get; }

        public GreenHouseContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            //var path = Environment.GetFolderPath(folder);
            var path = @"C:\Users\dfold\OneDrive\Skrivebord\BackEnd\Database";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            DbPath = Path.Combine(path, "App.db");
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }
    }
}