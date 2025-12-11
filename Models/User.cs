using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using User_service.Enums;

namespace User_service.Models
{
    [Index(nameof(NomUser), nameof(Email), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string NomUser { get; set; } = null!;
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string MotPasse { get; set; } = null!;
        public Role Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public Client? Client { get; set; }
        public Admin? Admin { get; set; }
    }
}
