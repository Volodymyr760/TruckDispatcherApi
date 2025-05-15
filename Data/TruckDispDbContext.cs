using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TruckDispatcherApi.Models;

namespace TruckDispatcherApi.Data
{
    public class TruckDispDbContext : IdentityDbContext<User>
    {
        public TruckDispDbContext(DbContextOptions<TruckDispDbContext> options)
            : base(options) { this.ChangeTracker.LazyLoadingEnabled = false; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

        public DbSet<Broker> Brokers { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Driver> Drivers { get; set; }

        public DbSet<Heatmap> Heatmaps { get; set; }
        public DbSet<HeatmapState> HeatmapStates { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<ImportLoad> ImportLoads { get; set; }

        public DbSet<Load> Loads { get; set; }

        public DbSet<MailTemplate> MailTemplates { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Pricepackage> Pricepackages { get; set; }

        public DbSet<Subscriber> Subscribers { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<SubscriberSubscription> SubscriberSubscriptions { get; set; }

        public DbSet<Truck> Trucks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity => { entity.ToTable(name: "Users"); });

            modelBuilder.Entity<IdentityRole>(entity => { entity.ToTable(name: "Roles"); });

            modelBuilder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles"); });

            modelBuilder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims"); });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogins"); });

            modelBuilder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserTokens"); });

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaims"); });

            modelBuilder.Entity<SubscriberSubscription>(entity => { entity.HasNoKey(); });
        }
    }
}
