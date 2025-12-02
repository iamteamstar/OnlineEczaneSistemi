using Microsoft.EntityFrameworkCore;
using OnlineEczaneSistemi.Models;

namespace OnlineEczaneSistemi.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
              : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<PharmacyRegistrationRequest> PharmacyRegistrationRequests { get; set; }
        public DbSet<CourierRegistrationRequest> CourierRegistrationRequests { get; set; }
    }
}
