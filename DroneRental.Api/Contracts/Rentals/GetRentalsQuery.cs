using DroneRental.Core.Enums;

namespace DroneRental.Api.Contracts.Rentals
{
    public class GetRentalsQuery
    {
        public RentalStatus? Status {  get; set; }
        
        public Guid? DroneId { get; set; }

        public string? CustomerEmail { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

    }
}
