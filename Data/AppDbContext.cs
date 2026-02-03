using Microsoft.EntityFrameworkCore;
using StreamApp.Models;

namespace StreamApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<WatchProgress> WatchProgresses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=streamapp.db");
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WatchProgress>()
                .HasIndex(w => w.FilePath)
                .IsUnique();
        }
    }
}
