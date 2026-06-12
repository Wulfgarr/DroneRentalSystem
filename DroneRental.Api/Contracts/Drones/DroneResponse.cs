namespace DroneRental.Api.Contracts.Drones
{
    public class DroneResponse
    {
        public Guid Id { get; set; }
        public string Model { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;

        public decimal PricePerHour { get; set; }
        public bool IsAvailable { get; set; }

        public int BatteryLifeMinutes { get; set; }
        public int MaxRangeMeters { get; set; }
    }
}
