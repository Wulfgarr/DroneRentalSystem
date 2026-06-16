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

            var (rentalId, token) = await CreateRentalWithTokenAsync(
                UserRole.Admin,
                RentalStatus.Active,
                "admin@test.com",
                "customer@test.com");

            client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PostAsync($"/api/rentals/{rentalId}/complete", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var rentalStatus = await GetRentalStatusAsync(rentalId);
            Assert.Equal(RentalStatus.Completed, rentalStatus);
            }

        [Fact]
        public async Task CompleteRental_WhenUserCompletesRental_ReturnsForbidden()
        {
            // Arrange
            var client = _factory.CreateClient();

            var (rentalId, token) = await CreateRentalWithTokenAsync(
                UserRole.Customer,
                RentalStatus.Active,
                "user@test.com",
                "customer-user@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

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

            var (rentalId, token) = await CreateRentalWithTokenAsync(
                UserRole.Admin,
                RentalStatus.Cancelled,
                "admin-cancelled@test.com",
                "customer-cancelled@test.com",
                DateTime.UtcNow);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PostAsync($"/api/rentals/{rentalId}/complete", null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var rentalStatus = await GetRentalStatusAsync(rentalId);
            Assert.Equal(RentalStatus.Cancelled, rentalStatus);
        }

        [Fact]
        public async Task CompleteRental_WhenRentalIsAlreadyCompleted_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();

            var (rentalId, token) = await CreateRentalWithTokenAsync(
                UserRole.Admin,
                RentalStatus.Completed,
                "admin-completed@test.com",
                "customer-completed@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PostAsync($"/api/rentals/{rentalId}/complete", null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var rentalStatus = await GetRentalStatusAsync(rentalId);
            Assert.Equal(RentalStatus.Completed, rentalStatus);
        }

        [Fact]
        public async Task CompleteRental_WhenRentalDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();

            var token = CreateTokenForUser(
                UserRole.Admin,
                "admin-notfound@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var missingRentalId = Guid.NewGuid();

            // Act
            var response = await client.PostAsync($"/api/rentals/{missingRentalId}/complete", null);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CompleteRental_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var rentalId = Guid.NewGuid();

            // Act
            var response = await client.PostAsync($"/api/rentals/{rentalId}/complete", null);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private async Task<(Guid RentalId, string Token)> CreateRentalWithTokenAsync(
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

        private async Task<RentalStatus?> GetRentalStatusAsync(Guid rentalId)
        {
            using var scope = _factory.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<DroneRentalDbContext>();
            var rental = await dbContext.Rentals.FindAsync(rentalId);

            return rental?.Status;
        }

        private string CreateTokenForUser(UserRole userRole, string email)
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
