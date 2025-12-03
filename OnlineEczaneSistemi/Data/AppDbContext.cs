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
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<PaymentInfo> PaymentInfos { get; set; }
        public DbSet<Medicine> Medicines { get; set; }


    }
}
