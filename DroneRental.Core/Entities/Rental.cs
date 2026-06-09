using System;
using System.Collections.Generic;
using System.Text;

namespace DroneRental.Core.Entities
{
    public class Rental
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalCost { get; set; }

        // Drone object reference
        public Guid DroneID { get; set; }
        public Drone? Drone { get; set; }
    }
}
