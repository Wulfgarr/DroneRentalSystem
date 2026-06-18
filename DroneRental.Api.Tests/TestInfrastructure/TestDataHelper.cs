using DroneRental.Api.Services.Auth;
using DroneRental.Core.Entities;
using DroneRental.Core.Enums;
using DroneRental.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace DroneRental.Api.Tests.TestInfrastructure
{
    public class TestDataHelper
    {
        private readonly CustomWebApplicationFactory _factory;

        public TestDataHelper(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public async Task<(Guid RentalId, string Token)> CreateRentalWithTokenAsync(
        UserRole userRole,
        RentalStatus rentalStatus,
        string userEmail,
        string customerEmail,
        DateTime? cancelledAt = null)
        {
            using var scope = _factory.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<DroneRentalDbContext>();
            var jwtTokenService = scope.ServiceProvider.GetRequiredService<IJwtTokenService>();

            var user = new ApplicationUser
            {
                Email = userEmail,
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "not-used-in-this-test",
                Role = userRole
            };

            var drone = new Drone
            {
                Id = Guid.NewGuid(),
                Brand = "DJI",
                Model = "Mini 4 Pro",
                PricePerHour = 100,
                BatteryLifeMinutes = 45,
                MaxRangeMeters = 10000
            };

            var rental = new Rental
            {
                Id = Guid.NewGuid(),
                DroneId = drone.Id,
                User = user,
                CustomerName = "Test Customer",
                CustomerEmail = customerEmail,
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2),
                TotalPrice = 100,
                Status = rentalStatus,
                CancelledAt = cancelledAt
            };

            dbContext.Users.Add(user);
            dbContext.Drones.Add(drone);
            dbContext.Rentals.Add(rental);
            await dbContext.SaveChangesAsync();

            var token = jwtTokenService.GenerateToken(user);

            return (rental.Id, token);
        }

        public async Task<RentalStatus?> GetRentalStatusAsync(Guid rentalId)
        {
            using var scope = _factory.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<DroneRentalDbContext>();
            var rental = await dbContext.Rentals.FindAsync(rentalId);

            return rental?.Status;
        }

        public string CreateTokenForUser(UserRole userRole, string email)
        {
            using var scope = _factory.Services.CreateScope();

            var jwtTokenService = scope.ServiceProvider.GetRequiredService<IJwtTokenService>();

            var user = new ApplicationUser
            {
                Id = Random.Shared.Next(1, 1000000),
                Email = email,
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "not-used-in-this-test",
                Role = userRole
            };

            return jwtTokenService.GenerateToken(user);
        }
    }
}
