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
        public DbSet<EnergyState> Energy => Set<EnergyState>();
        public DbSet<CurrencyState> Currency => Set<CurrencyState>();
        public DbSet<HintsState> Hints => Set<HintsState>();
        public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>()
                .HasIndex(device => device.Token)
                .IsUnique();

            modelBuilder.Entity<Device>()
                .HasIndex(device => device.DeviceId)
                .IsUnique();

            modelBuilder.Entity<LevelProgress>()
                .HasIndex(progress => new { progress.PlayerId, progress.LevelId })
                .IsUnique();

            modelBuilder.Entity<EnergyState>()
                .HasKey(state => state.PlayerId);

            modelBuilder.Entity<EnergyState>()
                .Property(state => state.Version)
                .IsRowVersion();

            modelBuilder.Entity<CurrencyState>()
                .HasKey(state => state.PlayerId);

            modelBuilder.Entity<CurrencyState>()
                .Property(state => state.Version)
                .IsRowVersion();

            modelBuilder.Entity<HintsState>()
                .HasKey(state => state.PlayerId);

            modelBuilder.Entity<HintsState>()
                .Property(state => state.Version)
                .IsRowVersion();

            modelBuilder.Entity<IdempotencyRecord>()
                .HasKey(record => record.RequestId);
        }
    }

    public class EnergyState
    {
        public Guid PlayerId { get; set; }
        public int Value { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public uint Version { get; set; }
    }

    public class CurrencyState
    {
        public Guid PlayerId { get; set; }
        public int Balance { get; set; }
        public uint Version { get; set; }
    }

    public class HintsState
    {
        public Guid PlayerId { get; set; }
        public int Count { get; set; }
        public uint Version { get; set; }
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

    public class IdempotencyRecord
    {
        public Guid RequestId { get; set; }
        public Guid PlayerId { get; set; }
        public string ResponseBody { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}