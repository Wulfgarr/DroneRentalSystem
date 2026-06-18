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

    }
}
