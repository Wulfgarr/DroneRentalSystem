using System;

namespace DroneRental.Core.Entities
{
    public class Rental
    {
        public Guid Id { get; set; }

        // Drone object reference
        public Guid DroneId { get; set; }
        public Drone? Drone { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public decimal TotalPrice { get; set; }


    }
}
