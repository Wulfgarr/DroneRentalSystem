using DroneRental.Core.Entities;
using DroneRental.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DroneRental.Api.Contracts.Drones;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<ActionResult<DroneResponse>> GetDrone(Guid id)
        {
            var drone = await _context.Drones
                .AsNoTracking()
                .Where(d => d.Id == id)
                .Select(d => new DroneResponse
                {
                    Id = d.Id,
                    Model = d.Model,
                    Brand = d.Brand,
                    PricePerHour = d.PricePerHour,
                    IsAvailable = d.IsAvailable,
                    BatteryLifeMinutes = d.BatteryLifeMinutes,
                    MaxRangeMeters = d.MaxRangeMeters
                })
                .FirstOrDefaultAsync();

            if (drone == null)
            {
                return NotFound();
            }

            return Ok(drone);
        }
        // GET: api/drones - Get all drones from database
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DroneResponse>>> GetDrones()
        {
            var drones = await _context.Drones
                .AsNoTracking()
                .Select(d => new DroneResponse
                {
                    Id = d.Id,
                    Model = d.Model,
                    Brand = d.Brand,
                    PricePerHour = d.PricePerHour,
                    IsAvailable = d.IsAvailable,
                    BatteryLifeMinutes = d.BatteryLifeMinutes,
                    MaxRangeMeters = d.MaxRangeMeters

                })
                .ToListAsync();
                
            return Ok(drones);
        }

        // POST: api/drones - Add new drone to database
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DroneResponse>> CreateDrone(CreateDroneRequest request)
        {


            var drone = new Drone
            {
                Id = Guid.NewGuid(),
                Model = request.Model,
                Brand = request.Brand,
                PricePerHour = request.PricePerHour,
                IsAvailable = request.IsAvailable,
                BatteryLifeMinutes = request.BatteryLifeMinutes,
                MaxRangeMeters = request.MaxRangeMeters

            };

            // Add drone to EF Core memory
            _context.Drones.Add(drone);

            // Save changes to SQLite
            await _context.SaveChangesAsync();

            var response = new DroneResponse
            {
                Id = drone.Id,
                Model = drone.Model,
                Brand = drone.Brand,
                PricePerHour = drone.PricePerHour,
                IsAvailable = drone.IsAvailable,
                BatteryLifeMinutes = drone.BatteryLifeMinutes,
                MaxRangeMeters = drone.MaxRangeMeters
            };

            return CreatedAtAction(nameof(GetDrone), new { id = drone.Id }, response);
        }

        // PUT: api/drones/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateDrone(Guid id, UpdateDroneRequest request)
        {

            var existingDrone = await _context.Drones.FindAsync(id);

            if (existingDrone == null)
            {
                return NotFound();
            }

            existingDrone.Model = request.Model;
            existingDrone.Brand = request.Brand;
            existingDrone.PricePerHour = request.PricePerHour;
            existingDrone.IsAvailable = request.IsAvailable;
            existingDrone.BatteryLifeMinutes = request.BatteryLifeMinutes;
            existingDrone.MaxRangeMeters = request.MaxRangeMeters;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/drones/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
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
