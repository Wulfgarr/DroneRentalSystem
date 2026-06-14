namespace DroneRental.Api.Services.Rentals
{
    public interface IRentalPricingService
    {
        decimal CalculateTotalPrice(DateTime startTime, DateTime endTime, decimal pricePerHour);
    }
}
