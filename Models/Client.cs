using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace User_service.Models
{
    [Index(nameof(NomUser), nameof(Email), IsUnique = true)]
    public class Client
    {
        public int Id { get; set; }
        [Required]
        public string NomUser { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string MotPasse { get; set; } = null!;
    }
}
