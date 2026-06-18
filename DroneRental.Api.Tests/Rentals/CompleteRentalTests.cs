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
        private readonly TestDataHelper _testData;

        public CompleteRentalTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _testData = new TestDataHelper(factory);

        }

        [Fact]
        public async Task CompleteRental_WhenAdminCompletesActiveRental_ReturnsOkAndChangesStatusToCompleted()
        {
            // Arrange
            var client = _factory.CreateClient();

            var (rentalId, token) = await _testData.CreateRentalWithTokenAsync(
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

            var rentalStatus = await _testData.GetRentalStatusAsync(rentalId);
            Assert.Equal(RentalStatus.Completed, rentalStatus);
            }

        [Fact]
        public async Task CompleteRental_WhenUserCompletesRental_ReturnsForbidden()
        {
            // Arrange
            var client = _factory.CreateClient();

            var (rentalId, token) = await _testData.CreateRentalWithTokenAsync(
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

            var (rentalId, token) = await _testData.CreateRentalWithTokenAsync(
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

            var rentalStatus = await _testData.GetRentalStatusAsync(rentalId);
            Assert.Equal(RentalStatus.Cancelled, rentalStatus);
        }

        [Fact]
        public async Task CompleteRental_WhenRentalIsAlreadyCompleted_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();

            var (rentalId, token) = await _testData.CreateRentalWithTokenAsync(
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

            var rentalStatus = await _testData.GetRentalStatusAsync(rentalId);
            Assert.Equal(RentalStatus.Completed, rentalStatus);
        }

        [Fact]
        public async Task CompleteRental_WhenRentalDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();

            var token = _testData.CreateTokenForUser(
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



    }
}
