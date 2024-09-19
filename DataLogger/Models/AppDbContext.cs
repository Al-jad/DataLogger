using Microsoft.EntityFrameworkCore;

namespace DataLogger.Models
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<PipesData> PipesData { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<TankData> TankData { get; set; }
        
    }
}
