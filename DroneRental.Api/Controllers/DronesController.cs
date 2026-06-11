using DroneRental.Core.Entities;
using DroneRental.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DroneRental.Api.Controllers
{
    [ApiController]
    [Route("api/drones")] // Address will be: api/drones
    public class DronesController : ControllerBase
    {
        private readonly DroneRentalDbContext _context;

        public DronesController(DroneRentalDbContext context)
        {
            _context = context;
        }

        // GET(by Id): api/drones/{id} - Get one drone by id
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Drone>> GetDrone(Guid id)
        {
            var drone = await _context.Drones.FindAsync(id);

            if (drone == null)
            {
                return NotFound();
            }

            return Ok(drone);
        }
        // GET: api/drones - Get all drones from database
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Drone>>> GetDrones()
        {
            var drones = await _context.Drones.ToListAsync();
            return Ok(drones);
        }

        // POST: api/drones - Add new drone to database
        [HttpPost]
        public async Task<ActionResult<Drone>> CreateDrone(Drone drone)
        {
            // Create new id for drone
            drone.Id = Guid.NewGuid();

            // Add drone to EF Core memory
            _context.Drones.Add(drone);

            // Save changes to SQLite
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDrone), new { id = drone.Id }, drone);
        }

        // PUT: api/drones/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateDrone(Guid id, Drone updatedDrone)
        {
            if (id != updatedDrone.Id)
            {
                return BadRequest("Id from URL does not match drone Id from body.");
            }

            var existingDrone = await _context.Drones.FindAsync(id);

            if (existingDrone == null)
            {
                return NotFound();
            }

            existingDrone.Model = updatedDrone.Model;
            existingDrone.Brand = updatedDrone.Brand;
            existingDrone.PricePerHour = updatedDrone.PricePerHour;
            existingDrone.IsAvailable = updatedDrone.IsAvailable;
            existingDrone.BatteryLifeMinutes = updatedDrone.BatteryLifeMinutes;
            existingDrone.MaxRangeMeters = updatedDrone.MaxRangeMeters;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/drones/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteDrone(Guid id)
        {
            var drone = await _context.Drones.FindAsync(id);

            if (drone == null)
            {
                return NotFound();
            }

            _context.Drones.Remove(drone);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
