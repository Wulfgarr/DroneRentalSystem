using System.Net;
using System.Net.Http.Headers;
using DroneRental.Api.Services.Auth;
using DroneRental.Api.Tests.TestInfrastructure;
using DroneRental.Core.Entities;
using DroneRental.Core.Enums;
using DroneRental.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DroneRental.Api.Tests.Rentals
{
    public class CompleteRentalTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public CompleteRentalTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;

        }

        [Fact]
        public async Task CompleteRental_WhenAdminCompletesActiveRental_ReturnsOkAndChangesStatusToCompleted()
        {
            // Arrange
            var client = _factory.CreateClient();

            Guid rentalId;

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DroneRentalDbContext>();
                var jwtTokenService = scope.ServiceProvider.GetRequiredService<IJwtTokenService>();

                var admin = new ApplicationUser
                {
                    Email = "admin@test.com",
                    FirstName = "Admin",
                    LastName = "Test",
                    PasswordHash = "not-used-in-this-test",
                    Role = UserRole.Admin
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
                    User = admin,
                    CustomerName = "Test Customer",
                    CustomerEmail = "customer@test.com",
                    StartTime = DateTime.UtcNow.AddHours(1),
                    EndTime = DateTime.UtcNow.AddHours(2),
                    TotalPrice = 100,
                    Status = RentalStatus.Active
                };

                dbContext.Users.Add(admin);
                dbContext.Drones.Add(drone);
                dbContext.Rentals.Add(rental);
                await dbContext.SaveChangesAsync();

                rentalId = rental.Id;

                var token = jwtTokenService.GenerateToken(admin);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

            }

            // Act
            var response = await client.PostAsync($"/api/rentals/{rentalId}/complete", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DroneRentalDbContext>();
                var rentalFromDatabase = await dbContext.Rentals.FindAsync(rentalId);

                Assert.NotNull(rentalFromDatabase);
                Assert.Equal(RentalStatus.Completed, rentalFromDatabase!.Status);
            }
        }

        [Fact]
        public async Task CompleteRental_WhenUserCompletesRental_ReturnsForbidden()
        {
            // Arrange
            var client = _factory.CreateClient();

            Guid rentalId;

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DroneRentalDbContext>();
                var jwtTokenService = scope.ServiceProvider.GetRequiredService<IJwtTokenService>();

                var user = new ApplicationUser
                {
                    Email = "user@test.com",
                    FirstName = "User",
                    LastName = "Test",
                    PasswordHash = "not-used-in-this-test",
                    Role = UserRole.Customer
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
                    CustomerEmail = "customer@test.com",
                    StartTime = DateTime.UtcNow.AddHours(1),
                    EndTime = DateTime.UtcNow.AddHours(2),
                    TotalPrice = 100,
                    Status = RentalStatus.Active
                };

                dbContext.Users.Add(user);
                dbContext.Drones.Add(drone);
                dbContext.Rentals.Add(rental);
                await dbContext.SaveChangesAsync();

                rentalId = rental.Id;

                var token = jwtTokenService.GenerateToken(user);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            // Act
            var response = await client.PostAsync($"/api/rentals/{rentalId}/complete", null);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CompleteRental_WhenRentalIsCancelled_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();

            Guid rentalId;

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DroneRentalDbContext>();
                var jwtTokenService = scope.ServiceProvider.GetRequiredService<IJwtTokenService>();

                var admin = new ApplicationUser
                {
                    Email = "admin-cancelled@test.com",
                    FirstName = "Admin",
                    LastName = "Test",
                    PasswordHash = "not-used-in-this-test",
                    Role = UserRole.Admin
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
                    User = admin,
                    CustomerName = "Test Customer",
                    CustomerEmail = "customer-cancelled@test.com",
                    StartTime = DateTime.UtcNow.AddHours(1),
                    EndTime = DateTime.UtcNow.AddHours(2),
                    TotalPrice = 100,
                    Status = RentalStatus.Cancelled,
                    CancelledAt = DateTime.UtcNow
                };

                dbContext.Users.Add(admin);
                dbContext.Drones.Add(drone);
                dbContext.Rentals.Add(rental);
                await dbContext.SaveChangesAsync();

                rentalId = rental.Id;

                var token = jwtTokenService.GenerateToken(admin);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            // Act
            var response = await client.PostAsync($"/api/rentals/{rentalId}/complete", null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DroneRentalDbContext>();
                var rentalFromDatabase = await dbContext.Rentals.FindAsync(rentalId);

                Assert.NotNull(rentalFromDatabase);
                Assert.Equal(RentalStatus.Cancelled, rentalFromDatabase!.Status);
            }
        }

    }
}
