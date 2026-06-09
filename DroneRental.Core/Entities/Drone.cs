using System;
using System.Collections.Generic;
using System.Text;

namespace DroneRental.Core.Entities
{
    internal class Drone
    {
        public Guid ID { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; } = string.Empty;
        public decimal PricePerHour { get; set; }
        bool isAvailable { get; set; } = true;
        public int BatteryLifeMinutes { get; set; }
        public int MaxRangeMeters { get; set; }

    }
}
