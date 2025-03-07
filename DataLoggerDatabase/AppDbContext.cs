using DataLoggerDatabase.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using static DataLoggerDatabase.Enums;

namespace DataLoggerDatabase
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser, IdentityRole<long>, long>(options)
    {
        // Define your DbSets for each model
        public DbSet<PipesData> PipesData { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<TankData> TankData { get; set; }
        public DbSet<StationStatus> StationStatus { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<StationType>();
            modelBuilder.HasDbFunction(DateTruncMethod).HasName("date_trunc");

            base.OnModelCreating(modelBuilder);
        }

        private static readonly MethodInfo DateTruncMethod = typeof(AppDbContext).GetRuntimeMethod(nameof(DateTrunc), [typeof(string), typeof(DateTime)])!;
        public static DateTime DateTrunc(string field, DateTime source) => throw new NotSupportedException();

        public static class TruncField
        {
            public const string microseconds = "microseconds";
            public const string milliseconds = "milliseconds";
            public const string second = "second";
            public const string minute = "minute";
            public const string hour = "hour";
            public const string day = "day";
            public const string week = "week";
            public const string month = "month";
            public const string quarter = "quarter";
            public const string year = "year";
            public const string decade = "decade";
            public const string century = "century";
            public const string millennium = "millennium";
        }
    }
}