using Microsoft.EntityFrameworkCore;

namespace DuckDoku.Api
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players => Set<Player>();

        public DbSet<Device> Devices => Set<Device>();
        
        public DbSet<LevelProgress> LevelProgress => Set<LevelProgress>();
        public DbSet<LevelSession> LevelSession => Set<LevelSession>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>()
                .HasIndex(device => device.Token)
                .IsUnique();
            
            modelBuilder.Entity<LevelProgress>()
                .HasIndex(progress => new { progress.PlayerId, progress.LevelId })
                .IsUnique();
        }
    }

    public class Player
    {
        public Guid Id { get; set; }
        public string? DisplayName { get; set; } 
        public DateTime CreatedAt { get; set; }
    }

    public class Device
    {
        public Guid Id { get; set; }

        public string DeviceId { get; set; } = "";

        public Guid PlayerId { get; set; }

        public DateTime LastSeenAt { get; set; }
        
        public string Token { get; set; } = "";
    }
}
