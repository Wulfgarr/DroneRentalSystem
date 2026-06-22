using DroneRental.Api.Tests.TestInfrastructure;
using System.Net.Http.Headers;
using System.Net;
using DroneRental.Core.Enums;


namespace DroneRental.Api.Tests.Rentals
{
    public class DeleteRentalTests
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly TestDataHelper _testData;

        public DeleteRentalTests()
        {
            _factory = new CustomWebApplicationFactory();
            _testData = new TestDataHelper(_factory);
        }

        [Fact]
        public async Task DeleteRental_WithoutToken_ReturnUnauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.DeleteAsync($"/api/rentals/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteRental_WhenUserIsCustomer_ReturnForbidden()
        {
            var client = _factory.CreateClient();

            var rental = await _testData.CreateRentalWithTokenAsync(
                UserRole.Customer,
                RentalStatus.Active,
                "delete-rental-owner@test.com",
                "delete-rental-customer@test.com");

            var customerToken = await _testData.CreateUserTokenAsync(
                UserRole.Customer,
                "delete-rental-customer-user@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", customerToken);

            var response = await client.DeleteAsync($"/api/rentals/{rental.RentalId}");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteRental_WhenUserIsAdminAndRentalExists_ReturnNoContent()
        {
            var client = _factory.CreateClient();

            var rental = await _testData.CreateRentalWithTokenAsync(
                UserRole.Customer,
                RentalStatus.Active,
                "delete-existing-owner@test.com",
                "delete-existing-customer@test.com");

            var adminToken = await _testData.CreateUserTokenAsync(
                UserRole.Admin,
                "delete-rental-admin@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.DeleteAsync($"/api/rentals/{rental.RentalId}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteRental_WhenUserIsAdminAndRentDoesNotExist_ReturnNotFound()
        {
            var client = _factory.CreateClient();

            var adminToken = await _testData.CreateUserTokenAsync(
                UserRole.Admin,
                "delete-missing-rental-admin@test.com");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.DeleteAsync($"api/rentals/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }


}
