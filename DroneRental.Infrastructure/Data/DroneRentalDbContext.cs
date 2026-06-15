using DroneRental.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DroneRental.Infrastructure.Data
{
    public class DroneRentalDbContext : DbContext
    {
        // Contstructor: allow data options sending (for example type or path) from API project
        public DroneRentalDbContext(DbContextOptions<DroneRentalDbContext> options) : base(options)
        {
        }

        // DbSet represents database tables
        // Property names (Drones, Rentals) are table names in SQLite
        public DbSet<Drone> Drones { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }


        // Configuration of database columns rules
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set precision for price per hour
            modelBuilder.Entity<Drone>()
                .Property(d => d.PricePerHour)
                .HasPrecision(18, 2);

            // Set precision for rental total price
            modelBuilder.Entity<Rental>()
                .Property(r => r.TotalPrice)
                .HasPrecision(18, 2);

            // Configure Drone -> Rental relationships
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Drone)
                .WithMany(d => d.Rentals)
                .HasForeignKey(r => r.DroneId);

            // Set user email as unique user id
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure User -> Rental relationship
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.User)
                .WithMany(u => u.Rentals)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}
