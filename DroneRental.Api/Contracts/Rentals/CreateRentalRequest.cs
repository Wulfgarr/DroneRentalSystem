namespace DroneRental.Api.Contracts.Rentals
{
    public class CreateRentalRequest
    {
        public Guid DroneId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
