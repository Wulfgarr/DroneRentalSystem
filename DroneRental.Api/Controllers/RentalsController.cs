
using DroneRental.Core.Entities;
using DroneRental.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<IEnumerable<Rental>>> GetRentals()
        {
            var rentals = await _context.Rentals
                .AsNoTracking()
                .ToListAsync();

            return Ok(rentals);
        }

        // GET: api/rentals/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Rental>> GetRental(Guid id)
        {
            var rental = await _context.Rentals
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null)
            {
                return NotFound();
            }

            return Ok(rental);

        }

        // POST: api/rentals
        [HttpPost]
        public async Task<ActionResult<Rental>> CreateRental(Rental rental)
        {
            var drone = await _context.Drones
                .FirstOrDefaultAsync(d => d.Id == rental.DroneId);
            if (drone == null)
            {
                return BadRequest("Drone does not exist.");
            }

            if (rental.EndTime <= rental.StartTime)
            {
                return BadRequest("End time must be later then start time");
            }

            if (rental.StartTime < DateTime.UtcNow)
            {
                return BadRequest("Rental cannot start in the past");
            }

            var hasConflict = await _context.Rentals.AnyAsync(existingRental =>
            existingRental.DroneId == rental.DroneId &&
            rental.StartTime < existingRental.EndTime &&
            rental.EndTime > existingRental.StartTime);

            if (hasConflict)
            {
                return BadRequest("Drone is already rent in this time peroid.");
            }

            var rentalDuration = rental.EndTime - rental.StartTime;
            var totalHours = (decimal)rentalDuration.TotalHours;
            var totalPrice = Math.Round(totalHours * drone.PricePerHour, 2);

            var newRental = new Rental
            {
                Id = Guid.NewGuid(),
                DroneId = drone.Id,
                CustomerName = rental.CustomerName,
                CustomerEmail = rental.CustomerEmail,
                StartTime = rental.StartTime,
                EndTime = rental.EndTime,
                TotalPrice = totalPrice
            };

            _context.Rentals.Add(newRental);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRental), new { id = newRental.Id }, newRental);
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