using Microsoft.EntityFrameworkCore;

namespace User_service.Models
{
    [Index(nameof(UserId), IsUnique = true)]
    public class Admin
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
