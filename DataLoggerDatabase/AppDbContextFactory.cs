using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataLoggerDatabase
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            
            // Provide your database connection string here
            optionsBuilder.UseNpgsql("DefaultConnection");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}