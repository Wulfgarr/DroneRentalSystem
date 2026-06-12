using System.ComponentModel.DataAnnotations;

namespace DroneRental.Api.Contracts.Rentals
{
    public class CreateRentalRequest
    {
        public Guid DroneId { get; set; }
        [Required]
        [StringLength(150)]
        public string CustomerName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string CustomerEmail { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
