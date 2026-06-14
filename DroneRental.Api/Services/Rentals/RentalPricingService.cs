namespace DroneRental.Api.Services.Rentals
{
    public class RentalPricingService : IRentalPricingService
    {
        public decimal CalculateTotalPrice(DateTime startTime, DateTime endTime, decimal pricePerHour)
        {
            var duration = endTime - startTime;
            return (decimal)duration.TotalHours * pricePerHour;
        }
    }
}
