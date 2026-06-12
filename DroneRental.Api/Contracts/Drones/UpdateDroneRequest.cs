using System.ComponentModel.DataAnnotations;

namespace DroneRental.Api.Contracts.Drones
{
    public class UpdateDroneRequest
    {
        [Required]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "100000")]
        public decimal PricePerHour { get; set; }
        public bool IsAvailable { get; set; } = true;

        [Range(1, 10000)]
        public int BatteryLifeMinutes { get; set; }
        [Range(1, 1000000)]
        public int MaxRangeMeters { get; set; }
    }
}
