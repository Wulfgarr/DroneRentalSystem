using System;
using System.Text.Json.Serialization;
using DroneRental.Core.Enums;

namespace DroneRental.Core.Entities
{
    public class Rental
    {
        public Guid Id { get; set; }

        // Drone object reference
        public Guid DroneId { get; set; }
        [JsonIgnore]
        public Drone? Drone { get; set; }
        // User object reference
        public int? UserId { get; set; }
        [JsonIgnore]
        public ApplicationUser? User { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public decimal TotalPrice { get; set; }

        public RentalStatus Status { get; set; } = RentalStatus.Active;
        public DateTime? CancelledAt { get; set; }


    }
}
