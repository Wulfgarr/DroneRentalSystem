using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DroneRental.Api.Contracts.Rentals;
using DroneRental.Api.Tests.TestInfrastructure;
using DroneRental.Core.Enums;

namespace DroneRental.Api.Tests.Rentals
{
    public class CreateRentalTests
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly TestDataHelper _testData;

        public CreateRentalTests()
        {
            _factory = new CustomWebApplicationFactory();
            _testData = new TestDataHelper(_factory);
        }

        [Fact]
        public async Task CreateRental_WhenRequestIsValidAndUserIsAuthenticated_ReturnsCreated()
        {
            // Arrange
            var client = _factory.CreateClient();

            var droneId = await _testData.CreateDroneAsync();

            var token = await _testData.CreateUserTokenAsync(
                UserRole.Customer,
                "create-rental-user@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var request = new CreateRentalRequest
            {
                DroneId = droneId,
                CustomerName = "Test Customer",
                CustomerEmail = "customer@test.com",
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2)
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/rentals", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateRental_WhenRentalOverlapsExistingActiveRental_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();

            var droneId = await _testData.CreateDroneAsync();

            var existingStart = DateTime.UtcNow.AddHours(2);
            var existingEnd = DateTime.UtcNow.AddHours(4);

            await _testData.CreateRentalForDroneAsync(
                droneId,
                RentalStatus.Active,
                existingStart,
                existingEnd);

            var token = await _testData.CreateUserTokenAsync(
                UserRole.Customer,
                "overlap-user@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var request = new CreateRentalRequest
            {
                DroneId = droneId,
                CustomerName = "Overlap Customer",
                CustomerEmail = "overlap@test.com",
                StartTime = DateTime.UtcNow.AddHours(3),
                EndTime = DateTime.UtcNow.AddHours(5)
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/rentals", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateRental_WhenRentalOverlapsCancelledRental_ReturnsCreated()
        {
            // Arrange
            var client = _factory.CreateClient();

            var droneId = await _testData.CreateDroneAsync();

            var existingStart = DateTime.UtcNow.AddHours(2);
            var existingEnd = DateTime.UtcNow.AddHours(4);

            await _testData.CreateRentalForDroneAsync(
                droneId,
                RentalStatus.Cancelled,
                existingStart,
                existingEnd,
                cancelledAt: DateTime.UtcNow.AddMinutes(-10));

            var token = await _testData.CreateUserTokenAsync(
                UserRole.Customer,
                "cancelled-overlap-user@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var request = new CreateRentalRequest
            {
                DroneId = droneId,
                CustomerName = "New Customer",
                CustomerEmail = "new-customer@test.com",
                StartTime = DateTime.UtcNow.AddHours(3),
                EndTime = DateTime.UtcNow.AddHours(5)
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/rentals", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}