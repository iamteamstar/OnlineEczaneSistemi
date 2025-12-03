using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineEczaneSistemi.Data;
using OnlineEczaneSistemi.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace OnlineEczaneSistemi.Controllers
{
    public class CourierController : Controller
    {
        private readonly AppDbContext _context;

        public CourierController(AppDbContext context)
        {
            _context = context;
        }

       
        [HttpGet]
        public IActionResult RegisterRequest()
        {
            return View();
        }

      
        [HttpPost]
        public async Task<IActionResult> RegisterRequest(CourierRegistrationRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Status = "Pending";
            model.CreatedAt = DateTime.Now;

            _context.CourierRegistrationRequests.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("RegisterSuccess");
        }

        [HttpGet]
        public IActionResult RegisterSuccess()
        {
            return View();
        }

       
        [Authorize(Roles = "Courier")]
        public async Task<IActionResult> Dashboard()
        {
            var courierId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var orders = await _context.Orders
                .Where(o => o.CourierId == courierId &&
                           (o.Status == "WaitingCourier" || o.Status == "OnTheWay"))
                .Include(o => o.Items)
                .ToListAsync();

            return View(orders);
        }
    }
}
