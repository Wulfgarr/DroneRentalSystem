using DroneRental.Core.Entities;
using DroneRental.Core.Enums;
using DroneRental.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace DroneRental.Api.Data

{
    public class DevelopmentDataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<DroneRentalDbContext>();

            await context.Database.MigrateAsync();

            var passwordHasher = new PasswordHasher<ApplicationUser>();

            var admin = await context.Users
                .FirstOrDefaultAsync(u => u.Email == "admin@dronerental.dev");

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    Email = "admin@dronerental.dev",
                    FirstName = "Admin",
                    LastName = "User",
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow
                };

                admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin123!");

                context.Users.Add(admin);
            }

            var customer = await context.Users
                .FirstOrDefaultAsync(u => u.Email == "customer@dronerental.dev");

            if (customer == null)
            {
                customer = new ApplicationUser
                {
                    Email = "customer@dronerental.dev",
                    FirstName = "Customer",
                    LastName = "User",
                    Role = UserRole.Customer,
                    CreatedAt = DateTime.UtcNow
                };

                customer.PasswordHash = passwordHasher.HashPassword(customer, "Customer123!");

                context.Users.Add(customer);
            }

            await context.SaveChangesAsync();

            var mavicId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var airId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var miniId = Guid.Parse("33333333-3333-3333-3333-333333333333");

            if (!await context.Drones.AnyAsync(d => d.Id == mavicId))
            {
                context.Drones.Add(new Drone
                {
                    Id = mavicId,
                    Brand = "DJI",
                    Model = "Mavic 3 Pro",
                    PricePerHour = 120,
                    IsAvailable = true,
                    BatteryLifeMinutes = 43,
                    MaxRangeMeters = 28000
                });
            }

            if (!await context.Drones.AnyAsync(d => d.Id == airId))
            {
                context.Drones.Add(new Drone
                {
                    Id = airId,
                    Brand = "DJI",
                    Model = "Air 3",
                    PricePerHour = 85,
                    IsAvailable = true,
                    BatteryLifeMinutes = 46,
                    MaxRangeMeters = 20000
                });
            }

            if (!await context.Drones.AnyAsync(d => d.Id == miniId))
            {
                context.Drones.Add(new Drone
                {
                    Id = miniId,
                    Brand = "DJI",
                    Model = "Mini 4 Pro",
                    PricePerHour = 55,
                    IsAvailable = true,
                    BatteryLifeMinutes = 34,
                    MaxRangeMeters = 18000
                });
            }

            await context.SaveChangesAsync();

            var now = DateTime.UtcNow;

            var activeRentalId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var cancelledRentalId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            var completedRentalId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

            if (!await context.Rentals.AnyAsync(r => r.Id == activeRentalId))
            {
                context.Rentals.Add(new Rental
                {
                    Id = activeRentalId,
                    DroneId = mavicId,
                    UserId = customer.Id,
                    CustomerName = $"{customer.FirstName} {customer.LastName}",
                    CustomerEmail = customer.Email,
                    StartTime = now.AddDays(2).Date.AddHours(10),
                    EndTime = now.AddDays(2).Date.AddHours(12),
                    TotalPrice = 240,
                    Status = RentalStatus.Active,
                    CancelledAt = null
                });
            }


            if (!await context.Rentals.AnyAsync(r => r.Id == cancelledRentalId))
            {
                context.Rentals.Add(new Rental
                {
                    Id = cancelledRentalId,
                    DroneId = airId,
                    UserId = customer.Id,
                    CustomerName = $"{customer.FirstName} {customer.LastName}",
                    CustomerEmail = customer.Email,
                    StartTime = now.AddDays(3).Date.AddHours(14),
                    EndTime = now.AddDays(3).Date.AddHours(16),
                    TotalPrice = 170,
                    Status = RentalStatus.Cancelled,
                    CancelledAt = now
                });
            }

            if (!await context.Rentals.AnyAsync(r => r.Id == completedRentalId))
            {
                context.Rentals.Add(new Rental
                {
                    Id = completedRentalId,
                    DroneId = miniId,
                    UserId = customer.Id,
                    CustomerName = $"{customer.FirstName} {customer.LastName}",
                    CustomerEmail = customer.Email,
                    StartTime = now.AddDays(-3).Date.AddHours(9),
                    EndTime = now.AddDays(-3).Date.AddHours(11),
                    TotalPrice = 110,
                    Status = RentalStatus.Completed,
                    CancelledAt = null
                });
            }
            await context.SaveChangesAsync();
        }
    }
}
