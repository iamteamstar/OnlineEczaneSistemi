using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineEczaneSistemi.Data;

namespace OnlineEczaneSistemi.Controllers
{
    [Authorize(Roles = "Pharmacy")]
    public class PharmacyController : Controller
    {
        private readonly AppDbContext _context;

        public PharmacyController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var pharmacyId = int.Parse(User.FindFirst("NameIdentifier")?.Value);

            var orders = await _context.Orders
                .Where(o => o.PharmacyId == pharmacyId)
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }
    }
}
