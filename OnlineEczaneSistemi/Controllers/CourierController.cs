using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineEczaneSistemi.Data;

namespace OnlineEczaneSistemi.Controllers
{
    [Authorize(Roles = "Courier")]
    public class CourierController : Controller
    {
        private readonly AppDbContext _context;

        public CourierController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var courierId = int.Parse(User.FindFirst("NameIdentifier")?.Value);

            var orders = await _context.Orders
                .Where(o => o.CourierId == courierId && o.Status == "WaitingCourier" || o.Status == "OnTheWay")
                .Include(o => o.Items)
                .ToListAsync();

            return View(orders);
        }
    }
}
