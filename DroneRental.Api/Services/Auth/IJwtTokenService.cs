using DroneRental.Core.Entities;

namespace DroneRental.Api.Services.Auth
{
    public interface IJwtTokenService
    {
        string GenerateToken(ApplicationUser user);
    }
}
