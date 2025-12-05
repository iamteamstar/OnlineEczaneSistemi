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
            var courierId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .Where(o => o.CourierId == courierId &&
                           (o.Status == "WaitingCourier" || o.Status == "OnTheWay"))
                .ToListAsync();

            return View(orders);
        }

        // 📌 Sipariş detay sayfası
        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            return View(order);
        }

        // 📌 Kuryenin teslimata başlaması
        public async Task<IActionResult> StartDelivery(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            order.Status = "OnTheWay";
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderDetail", new { id });
        }

        public async Task<IActionResult> CompletePreparing(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            order.Status = "WaitingCourier";

            var courier = await _context.Users
                .Where(u => u.Role == "Courier")
                .FirstOrDefaultAsync();

            if (courier != null)
                order.CourierId = courier.UserId;

            await _context.SaveChangesAsync();

            return RedirectToAction("OrderDetail", new { id });
        }
        [Authorize(Roles = "Courier")]
        public async Task<IActionResult> CompleteDelivery(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return NotFound();

            // Durumu teslim edildi yap
            order.Status = "Delivered";

            // Teslim zamanını kayıt altına almak istersen:
            order.CreatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Teslimat başarıyla tamamlandı!";
            return RedirectToAction("Dashboard");
        }


        [Authorize(Roles = "Courier")]
        public async Task<IActionResult> DeliveredOrders()
        {
            var courierId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var orders = await _context.Orders
                .Where(o => o.CourierId == courierId && o.Status == "Delivered")
                .Include(o => o.User) // Hasta bilgisi geliyor
                .Include(o => o.Items) // Gerekirse ürünler
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }



        public async Task<IActionResult> AcceptOrder(int id)
        {
            var courierId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var order = await _context.Orders.FindAsync(id);

            order.CourierId = courierId;   // 🔥 Bu çok önemli
            order.Status = "OnTheWay";

            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }
    

    }
}
