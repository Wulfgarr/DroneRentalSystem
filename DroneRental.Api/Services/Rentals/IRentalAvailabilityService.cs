namespace DroneRental.Api.Services.Rentals
{
    public interface IRentalAvailabilityService
    {
        Task<bool> HasConflictingRentalAsync(Guid droneId, DateTime startTime, DateTime endTime);
    }
}
