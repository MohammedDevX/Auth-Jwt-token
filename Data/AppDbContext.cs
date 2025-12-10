using Microsoft.EntityFrameworkCore;
using User_service.Models;

namespace User_service.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

        public DbSet<Client> Clients { get; set; }
    }
}
