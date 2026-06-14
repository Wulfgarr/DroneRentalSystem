
using DroneRental.Core.Entities;
using DroneRental.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DroneRental.Api.Contracts.Rentals;
using DroneRental.Core.Enums;

namespace DroneRental.Api.Controllers
{
    [ApiController]
    [Route("api/rentals")]
    public class RentalsController : ControllerBase
    {

        private readonly DroneRentalDbContext _context;

        public RentalsController(DroneRentalDbContext context)
        {
            _context = context;
        }

        // GET: api/rentals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RentalResponse>>> GetRentals()
        {
            var rentals = await _context.Rentals
                .AsNoTracking()
                .ToListAsync();

            var response = rentals
                .Select(MapToRentalResponse)
                .ToList();

            return Ok(response);
        }

        // GET: api/rentals/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<RentalResponse>> GetRental(Guid id)
        {
            var rental = await _context.Rentals
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null)
            {
                return NotFound();
            }

            return Ok(MapToRentalResponse(rental));

        }

        // POST: api/rentals
        [HttpPost]
        public async Task<ActionResult<RentalResponse>> CreateRental(CreateRentalRequest request)
        {
            if (request.DroneId == Guid.Empty)
            {
                return BadRequest("DroneId is required.");
            }

            if (request.StartTime == default)
            {
                return BadRequest("Start time is required.");
            }

            if (request.EndTime == default)
            {
                return BadRequest("End time is required.");
            }

            if (request.EndTime <= request.StartTime)
            {
                return BadRequest("End time must be later than start time.");
            }

            if (request.StartTime < DateTime.UtcNow)
            {
                return BadRequest("Rental cannot start in the past");
            }
            var drone = await _context.Drones
                .FirstOrDefaultAsync(d => d.Id == request.DroneId);
            if (drone == null)
            {
                return BadRequest("Drone does not exist.");
            }

            var hasConflict = await _context.Rentals.AnyAsync(existingRental =>
            existingRental.DroneId == request.DroneId &&
            existingRental.Status != RentalStatus.Cancelled &&
            request.StartTime < existingRental.EndTime &&
            request.EndTime > existingRental.StartTime);

            if (hasConflict)
            {
                return BadRequest("Drone is already rented in this time period.");
            }

            var rentalDuration = request.EndTime - request.StartTime;
            var totalHours = (decimal)rentalDuration.TotalHours;
            var totalPrice = Math.Round(totalHours * drone.PricePerHour, 2);

            var newRental = new Rental
            {
                Id = Guid.NewGuid(),
                DroneId = drone.Id,
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                TotalPrice = totalPrice,
                Status = RentalStatus.Active,
                CancelledAt = null
            };

            _context.Rentals.Add(newRental);
            await _context.SaveChangesAsync();

            var response = MapToRentalResponse(newRental);

            return CreatedAtAction(nameof(GetRental), new { id = response.Id }, response);
        }

        // POST: api/rentals/{id}/cancel
        [HttpPost("{id:guid}/cancel")]
        public async Task<ActionResult<RentalResponse>> CancelRental(Guid id)
        {
            var rental = await _context.Rentals.FindAsync(id);

            if (rental == null)
            {
                return NotFound();
            }

            if (rental.Status == RentalStatus.Cancelled)
            {
                return BadRequest("Rental is already cancelled.");
            }

            if (rental.Status == RentalStatus.Completed)
            {
                return BadRequest("Completed rental cannot be cancelled.");
            }

            rental.Status = RentalStatus.Cancelled;
            rental.CancelledAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var response = MapToRentalResponse(rental);

            return Ok(response);
        }

        // DELETE: api/rentals/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteRental(Guid id)
        {
            var rental = await _context.Rentals.FindAsync(id);

            if (rental == null)
            {
                return NotFound();
            }

            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static RentalResponse MapToRentalResponse(Rental rental)
        {
            return new RentalResponse
            {
                Id = rental.Id,
                DroneId = rental.DroneId,
                CustomerName = rental.CustomerName,
                CustomerEmail = rental.CustomerEmail,
                StartTime = rental.StartTime,
                EndTime = rental.EndTime,
                TotalPrice = rental.TotalPrice,
                Status = rental.Status.ToString(),
                CancelledAt = rental.CancelledAt
            };
        }
    }

}