namespace DroneRental.Api.Contracts.Drones
{
    public class CreateDroneRequest
    {
        public string Model { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        
        public decimal PricePerHour { get; set; }
        public bool IsAvailable { get; set; } = true;

        public int BatteryLifeMinutes { get; set; }
        public int MaxRangeMeters { get; set; }
    }
}
