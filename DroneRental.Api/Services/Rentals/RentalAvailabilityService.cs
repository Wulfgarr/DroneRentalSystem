using DroneRental.Core.Enums;
using DroneRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DroneRental.Api.Services.Rentals
{
    public class RentalAvailabilityService : IRentalAvailabilityService
    {
        private readonly DroneRentalDbContext _context;

        public RentalAvailabilityService(DroneRentalDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasConflictingRentalAsync(
            Guid droneId,
            DateTime startTime,
            DateTime endTime)
        {
            return await _context.Rentals.AnyAsync(existingRental =>
            existingRental.DroneId == droneId &&
            existingRental.Status != RentalStatus.Cancelled &&
            startTime < existingRental.EndTime &&
            endTime > existingRental.EndTime);
        }
    }
}
