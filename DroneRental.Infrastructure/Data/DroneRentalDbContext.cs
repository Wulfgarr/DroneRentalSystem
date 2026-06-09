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

        // Configuration of database columns rules
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set precision for prices
            modelBuilder.Entity<Drone>()
                .Property(d => d.PricePerHour)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Rental>()
                .Property(r => r.TotalCost)
                .HasPrecision(18, 2);
        }


    }
}
