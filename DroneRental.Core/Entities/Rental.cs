using System;
using System.Text.Json.Serialization;

namespace DroneRental.Core.Entities
{
    public class Rental
    {
        public Guid Id { get; set; }

        // Drone object reference
        public Guid DroneId { get; set; }
        [JsonIgnore]
        public Drone? Drone { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public decimal TotalPrice { get; set; }


    }
}
