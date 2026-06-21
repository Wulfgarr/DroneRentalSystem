using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DroneRental.Api.Contracts.Rentals;
using DroneRental.Api.Tests.TestInfrastructure;
using DroneRental.Core.Enums;

namespace DroneRental.Api.Tests.Rentals
{
    public class GetRentalsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly TestDataHelper _testData;

        public GetRentalsTests()
        {
            _factory = new CustomWebApplicationFactory(); 
            _testData = new TestDataHelper(_factory);
        }

        [Fact]
        public async Task GetRentals_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/rentals");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetRentals_WhenUserIsCustomer_ReturnsForbidden()
        {
            // Arrange
            var client = _factory.CreateClient();

            var token = await _testData.CreateUserTokenAsync(
                UserRole.Customer,
                "get-rentals-customer@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.GetAsync("/api/rentals");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetRentals_WhenUserIsAdmin_ReturnsPaginatedRentals()
        {
            // Arrange
            var client = _factory.CreateClient();

            var token = await _testData.CreateUserTokenAsync(
                UserRole.Admin,
                "get-rentals-admin@tests.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var droneId = await _testData.CreateDroneAsync();

            await _testData.CreateRentalForDroneAsync(
                droneId,
                RentalStatus.Active,
                DateTime.UtcNow.AddHours(1),
                DateTime.UtcNow.AddHours(2));

            await _testData.CreateRentalForDroneAsync(
                droneId,
                RentalStatus.Active,
                DateTime.UtcNow.AddHours(3),
                DateTime.UtcNow.AddHours(4));
            await _testData.CreateRentalForDroneAsync(
                droneId,
                RentalStatus.Active,
                DateTime.UtcNow.AddHours(5),
                DateTime.UtcNow.AddHours(6));

            // Act
            var response = await client.GetAsync("/api/rentals?page=1&pageSize=2");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var items = root.GetProperty("items").EnumerateArray().ToList();

            Assert.Equal(2, items.Count);

            Assert.Equal(3, root.GetProperty("totalCount").GetInt32());
            Assert.Equal(1, root.GetProperty("page").GetInt32());
            Assert.Equal(2, root.GetProperty("pageSize").GetInt32());
            Assert.Equal(2, root.GetProperty("totalPages").GetInt32());
        }

        [Fact]
        public async Task GetRentals_WhithStatusFilter_ReturnsOnlyMatchingRentals()
        {
            var client = _factory.CreateClient();

            var token = await _testData.CreateUserTokenAsync(
                UserRole.Admin,
                "get-rentals-status-admin@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var droneId = await _testData.CreateDroneAsync();
   
            await _testData.CreateRentalForDroneAsync(
                droneId,
                RentalStatus.Active,
                DateTime.UtcNow.AddHours(1),
                DateTime.UtcNow.AddHours(2));

            var cancelledRentalId = await _testData.CreateRentalForDroneAsync(
                droneId,
                RentalStatus.Cancelled,
                DateTime.UtcNow.AddHours(3),
                DateTime.UtcNow.AddHours(4),
                DateTime.UtcNow);

            await _testData.CreateRentalForDroneAsync(
                droneId,
                RentalStatus.Completed,
                DateTime.UtcNow.AddHours(5),
                DateTime.UtcNow.AddHours(6));

            // Act
            var response = await client.GetAsync("/api/rentals?status=Cancelled");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
 
            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var items = root.GetProperty("items").EnumerateArray().ToList();

            Assert.Single(items);
            Assert.Equal(cancelledRentalId, items[0].GetProperty("id").GetGuid());
            Assert.Equal(1, root.GetProperty("totalCount").GetInt32());

        }

        [Fact]
        public async Task GetRentals_WithDroneIdFilter_ReturnsOnlyRentalsForSelectedDrone()
        {
            // Arrange
            var client = _factory.CreateClient();

            var token = await _testData.CreateUserTokenAsync(
                UserRole.Admin,
                "get-rentals-drone-admin@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var targetDroneId = await _testData.CreateDroneAsync();

            var otherDroneId = await _testData.CreateDroneAsync();

            var firstTargetRentalId = await _testData.CreateRentalForDroneAsync(
                targetDroneId,
                RentalStatus.Active,
                DateTime.UtcNow.AddHours(1),
                DateTime.UtcNow.AddHours(2));

            var secondTargetRentalId = await _testData.CreateRentalForDroneAsync(
                targetDroneId,
                RentalStatus.Completed,
                DateTime.UtcNow.AddHours(3),
                DateTime.UtcNow.AddHours(4));

            await _testData.CreateRentalForDroneAsync(
                otherDroneId,
                RentalStatus.Active,
                DateTime.UtcNow.AddHours(5),
                DateTime.UtcNow.AddHours(6));

            // Act
            var response = await client.GetAsync($"/api/rentals?droneId={targetDroneId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var items = root.GetProperty("items").EnumerateArray().ToList();

            Assert.Equal(2, items.Count);

            Assert.Equal(2, root.GetProperty("totalCount").GetInt32());

            var returnedRentalIds = items
                .Select(item => item.GetProperty("id").GetGuid())
                .ToList();

            Assert.Contains(firstTargetRentalId, returnedRentalIds);
            Assert.Contains(secondTargetRentalId, returnedRentalIds);

            Assert.All(items, item =>
            {
                Assert.Equal(targetDroneId, item.GetProperty("droneId").GetGuid());
            });
        }

        [Fact]
        public async Task GetRentals_WithCustomerEmailFilter_ReturnsOnlyMatchingRentals()
        {
            var client = _factory.CreateClient();

            var adminToken = await _testData.CreateUserTokenAsync(
                UserRole.Admin,
                "get-rentals-email-admin@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var targetRental = await _testData.CreateRentalWithTokenAsync(
                UserRole.Customer,
                RentalStatus.Active,
                "target-rental-owner@test.com",
                "target-customer@test.com");

            await _testData.CreateRentalWithTokenAsync(
                UserRole.Customer,
                RentalStatus.Active,
                "other-rental-owner@test.com",
                "other-customer@test.com");

            var response = await client.GetAsync("/api/rentals?customerEmail=target-customer@test.com");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var items = root.GetProperty("items").EnumerateArray().ToList();

            Assert.Single(items);
            Assert.Equal(targetRental.RentalId, items[0].GetProperty("id").GetGuid());
            Assert.Equal("target-customer@test.com", items[0].GetProperty("customerEmail").GetString());
            Assert.Equal(1, root.GetProperty("totalCount").GetInt32());
        }

        [Fact]
        public async Task GetRentals_WithInvalidPage_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();

            var adminToken = await _testData.CreateUserTokenAsync(
                UserRole.Admin,
                "get-rentals-invalid-page-admin@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.GetAsync("/api/rentals?page=0&pageSize=10");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
