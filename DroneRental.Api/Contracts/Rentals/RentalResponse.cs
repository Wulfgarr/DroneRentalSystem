namespace DroneRental.Api.Contracts.Rentals
{
    public class RentalResponse
    {
        public Guid Id { get; set; }
        public Guid DroneId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = string.Empty;
        public DateTime? CancelledAt { get; set; }
    }
}
