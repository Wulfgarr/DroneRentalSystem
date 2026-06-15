using DroneRental.Core.Enums;

namespace DroneRental.Core.Entities
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Customer;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }
}
