using DataLoggerDatabase.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLoggerDatabase
{
    public class AppDbContext : DbContext
    {
        // Define a constructor that accepts DbContextOptions
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Define your DbSets for each model
        public DbSet<PipesData> PipesData { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<TankData> TankData { get; set; }
        public DbSet<StationStatus> StationStatus { get; set; }
    }
}