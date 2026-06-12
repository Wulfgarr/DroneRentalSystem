
using DroneRental.Core.Entities;
using DroneRental.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DroneRental.Api.Contracts.Rentals;

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
                .Select(r => new RentalResponse
                {
                    Id = r.Id,
                    DroneId = r.DroneId,
                    CustomerName = r.CustomerName,
                    CustomerEmail = r.CustomerEmail,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    TotalPrice = r.TotalPrice
                })
                .ToListAsync();

            return Ok(rentals);
        }

        // GET: api/rentals/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<RentalResponse>> GetRental(Guid id)
        {
            var rental = await _context.Rentals
                .AsNoTracking()
                .Where(r => r.Id == id)
                .Select(r => new RentalResponse
                {
                    Id = r.Id,
                    DroneId = r.DroneId,
                    CustomerName = r.CustomerName,
                    CustomerEmail = r.CustomerEmail,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    TotalPrice = r.TotalPrice
                })
                .FirstOrDefaultAsync();

            if (rental == null)
            {
                return NotFound();
            }

            return Ok(rental);

        }

        // POST: api/rentals
        [HttpPost]
        public async Task<ActionResult<RentalResponse>> CreateRental(CreateRentalRequest request)
        {
            var drone = await _context.Drones
                .FirstOrDefaultAsync(d => d.Id == request.DroneId);
            if (drone == null)
            {
                return BadRequest("Drone does not exist.");
            }

            if (request.EndTime <= request.StartTime)
            {
                return BadRequest("End time must be later then start time.");
            }

            if (request.StartTime < DateTime.UtcNow)
            {
                return BadRequest("Rental cannot start in the past.");
            }

            var hasConflict = await _context.Rentals.AnyAsync(existingRental =>
            existingRental.DroneId == request.DroneId &&
            request.StartTime < existingRental.EndTime &&
            request.EndTime > existingRental.StartTime);

            if (hasConflict)
            {
                return BadRequest("Drone is already rented in this time peroid.");
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
                TotalPrice = totalPrice
            };

            _context.Rentals.Add(newRental);
            await _context.SaveChangesAsync();

            var response = new RentalResponse
            {
                Id = newRental.Id,
                DroneId = newRental.DroneId,
                CustomerName = newRental.CustomerName,
                CustomerEmail = newRental.CustomerEmail,
                StartTime = newRental.StartTime,
                EndTime = newRental.EndTime,
                TotalPrice = totalPrice
            };
            return CreatedAtAction(nameof(GetRental), new { id = response.Id }, response);
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
    }

}