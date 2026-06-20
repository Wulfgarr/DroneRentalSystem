
using DroneRental.Core.Entities;
using DroneRental.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DroneRental.Api.Contracts.Rentals;
using DroneRental.Api.Services.Rentals;
using DroneRental.Core.Enums;
using DroneRental.Api.Contracts.Common;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DroneRental.Api.Controllers
{
    [ApiController]
    [Route("api/rentals")]
    public class RentalsController : ControllerBase
    {

        private readonly DroneRentalDbContext _context;
        private readonly IRentalPricingService _rentalPricingService;
        private readonly IRentalAvailabilityService _rentalAvailabilityService;

        public RentalsController(
            DroneRentalDbContext context,
            IRentalPricingService rentalPricingService,
            IRentalAvailabilityService rentalAvailabilityService)
        {
            _context = context;
            _rentalPricingService = rentalPricingService;
            _rentalAvailabilityService = rentalAvailabilityService;
        }

        // GET: api/rentals
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginatedResponse<RentalResponse>>> GetRentals(
            [FromQuery] GetRentalsQuery query)
        {
            if (query.Page < 1)
            {
                return BadRequest("Page must be greater than or equal to 1.");
            }

            if (query.PageSize < 1)
            {
                return BadRequest("PageSize must be greater than or equal to 1.");
            }

            if (query.PageSize > 100)
            {
                return BadRequest("PageSize cannot be greater than 100");
            }

            var rentalsQuery = _context.Rentals
                .AsNoTracking()
                .AsQueryable();

            if (query.Status.HasValue)
            {
                rentalsQuery = rentalsQuery
                    .Where(r => r.Status == query.Status.Value);
            }

            if (query.DroneId.HasValue)
            {
                rentalsQuery = rentalsQuery
                    .Where(r => r.DroneId == query.DroneId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.CustomerEmail))
            {
                var customerEmail = query.CustomerEmail.Trim().ToLower();

                rentalsQuery = rentalsQuery
                    .Where(r => r.CustomerEmail.ToLower() == customerEmail);
            }

            var totalCount = await rentalsQuery.CountAsync();

            var rentals = await rentalsQuery
                .OrderBy(r => r.StartTime)
                .ThenBy(r => r.Id)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var response = new PaginatedResponse<RentalResponse>
            {
                Items = rentals
                    .Select(MapToRentalResponse)
                    .ToList(),
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            };

            return Ok(response);
        }

        // GET: api/rentals/{id}
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<RentalResponse>> GetRental(Guid id)
        {
            var rental = await _context.Rentals
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user token.");
            }

            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && rental.UserId != userId)
            {
                return Forbid();
            }

            return Ok(MapToRentalResponse(rental));

        }

        // GET: api/rentals/my
        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RentalResponse>>> GetMyRentals()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user token.");
            }

            var rentals = await _context.Rentals
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .ToListAsync();

            var response = rentals
                .Select(MapToRentalResponse)
                .ToList();

            return Ok(response);
        }

        // POST: api/rentals
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RentalResponse>> CreateRental(CreateRentalRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user token.");
            }

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

            var hasConflict = await _rentalAvailabilityService.HasConflictingRentalAsync(
                request.DroneId,
                request.StartTime,
                request.EndTime);

            if (hasConflict)
            {
                return BadRequest("Drone is already rented in this time period.");
            }

            var totalPrice = _rentalPricingService.CalculateTotalPrice(
                request.StartTime,
                request.EndTime,
                drone.PricePerHour);

            var newRental = new Rental
            {
                Id = Guid.NewGuid(),
                DroneId = drone.Id,
                UserId = userId,
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
        [Authorize]
        public async Task<ActionResult<RentalResponse>> CancelRental(Guid id)
        {
            var rental = await _context.Rentals.FindAsync(id);

            if (rental == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user token.");
            }

            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && rental.UserId != userId)
            {
                return Forbid();
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

        // POST: api/rentals/{id}/complete
        [HttpPost("{id:guid}/complete")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RentalResponse>> CompleteRental(Guid id)
        {
            var rental = await _context.Rentals.FindAsync(id);

            if (rental == null)
            {
                return NotFound();
            }

            if (rental.Status == RentalStatus.Completed)
            {
                return BadRequest("Rental is already completed.");
            }

            if (rental.Status == RentalStatus.Cancelled)
            {
                return BadRequest("Cancelled rental cannot be completed.");
            }

            rental.Status = RentalStatus.Completed;

            await _context.SaveChangesAsync();

            var response = MapToRentalResponse(rental);

            return Ok(response);
        }

        // DELETE: api/rentals/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
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