using DroneRental.Infrastructure.Data;
using System.Net;
using System.Net.Http.Headers;
using DroneRental.Core.Enums;
using DroneRental.Api.Tests.TestInfrastructure;
using Xunit;

namespace DroneRental.Api.Tests.Rentals
{
    public class CancelRentalTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly TestDataHelper _testData;

        public CancelRentalTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _testData = new TestDataHelper(factory);
        }

        [Fact]
        public async Task CancelRental_WhenOwnerCancelsActiveRental_ReturnsOkAndChangesStatusToCancelled()
        {
            // Arrange
            var client = _factory.CreateClient();

            var (rentalId, token) = await _testData.CreateRentalWithTokenAsync(
                UserRole.Customer,
                RentalStatus.Active,
                "owner-cancel@test.com",
                "customer-cancel@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PostAsync($"/api/rentals/{rentalId}/cancel", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var rental = await _testData.GetRentalAsync(rentalId);

            Assert.NotNull(rental);
            Assert.Equal(RentalStatus.Cancelled, rental!.Status);
            Assert.NotNull(rental.CancelledAt);
        }


        [Fact]
        public async Task CancelRental_WhenAdminCancelsSomeoneElsesActiveRental_ReturnsOkAndChangesStatusToCancelled()
        {
            // Arrange
            var client = _factory.CreateClient();

            var (rentalId, _) = await _testData.CreateRentalWithTokenAsync(
                UserRole.Customer,
                RentalStatus.Active,
                "rental-owner@test.com",
                "customer@test.com");

            var adminToken = await _testData.CreateUserTokenAsync(
                UserRole.Admin,
                "admin-cancel@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            // Act
            var response = await client.PostAsync($"/api/rentals/{rentalId}/cancel", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var rentalStatus = await _testData.GetRentalStatusAsync(rentalId);
            Assert.Equal(RentalStatus.Cancelled, rentalStatus);
        }

        [Fact]
        public async Task CancelRental_WhenCustomerCancelsSomeoneElsesRental_ReturnsForbiddenAndDoesNotChangeStatus()
        {
            // Arrange
            var client = _factory.CreateClient();

            var (rentalId, _) = await _testData.CreateRentalWithTokenAsync(
                UserRole.Customer,
                RentalStatus.Active,
                "real-owner@test.com",
                "real-customer@test.com");

            var otherCustomerToken = await _testData.CreateUserTokenAsync(
                UserRole.Customer,
                "other-customer@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", otherCustomerToken);

            // Act
            var response = await client.PostAsync($"/api/rentals/{rentalId}/cancel", null);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

            var rentalStatus = await _testData.GetRentalStatusAsync(rentalId);
            Assert.Equal(RentalStatus.Active, rentalStatus);
        }

        [Fact]
        public async Task CancelRental_WhenRentalIsAlreadyCancelled_ReturnsBadRequestAndKeepsStatusCancelled()
        {
            // Arrange
            var client = _factory.CreateClient();

            var (rentalId, token) = await _testData.CreateRentalWithTokenAsync(
                UserRole.Customer,
                RentalStatus.Cancelled,
                "already-cancelled-owner@test.com",
                "already-cancelled-customer@test.com",
                cancelledAt: DateTime.UtcNow.AddMinutes(-10));

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PostAsync($"/api/rentals/{rentalId}/cancel", null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var rentalStatus = await _testData.GetRentalStatusAsync(rentalId);
            Assert.Equal(RentalStatus.Cancelled, rentalStatus);
        }

        [Fact]
        public async Task CancelRental_WhenRentalIsCompleted_ReturnsBadRequestAndKeepsStatusCompleted()
        {
            // Arrange
            var client = _factory.CreateClient();

            var (rentalId, token) = await _testData.CreateRentalWithTokenAsync(
                UserRole.Customer,
                RentalStatus.Completed,
                "completed-owner@test.com",
                "completed-customer@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.PostAsync($"/api/rentals/{rentalId}/cancel", null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var rentalStatus = await _testData.GetRentalStatusAsync(rentalId);
            Assert.Equal(RentalStatus.Completed, rentalStatus);
        }

        [Fact]
        public async Task CancelRental_WhenRentalDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();

            var adminToken = await _testData.CreateUserTokenAsync(
                UserRole.Admin,
                "admin-cancel-not-found@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var nonExistingRentalId = Guid.NewGuid();

            // Act
            var response = await client.PostAsync($"/api/rentals/{nonExistingRentalId}/cancel", null);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


    }
}
