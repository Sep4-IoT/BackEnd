using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess
{
    public class GreenHouseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<GreenHouse> GreenHouses { get; set; }

        public GreenHouseContext(DbContextOptions<GreenHouseContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=../EfcDataAccess/GreenHouseDatabase.db");
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GreenHouse>().HasKey(g => g.GreenHouseId);
            modelBuilder.Entity<GreenHouse>()
                .HasOne(greenHouse => greenHouse.Owner)
                .WithMany()
                .HasForeignKey(greenhouse => greenhouse.OwnerId);
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
        }
    }
}