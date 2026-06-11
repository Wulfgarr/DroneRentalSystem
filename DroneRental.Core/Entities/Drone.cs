using System;
using System.Collections.Generic;
using System.Text;

namespace DroneRental.Core.Entities
{
    public class Drone
    {
        public Guid Id { get; set; }

        public string Model { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;

        public decimal PricePerHour { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int BatteryLifeMinutes { get; set; }
        public int MaxRangeMeters { get; set; }

        public ICollection<Rental> Rental { get; set; } = new List<Rental>();

    }
}
