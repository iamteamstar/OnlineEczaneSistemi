using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineEczaneSistemi.Data;
using OnlineEczaneSistemi.Models;
using System.Security.Claims;

namespace OnlineEczaneSistemi.Controllers
{
    [Authorize(Roles = "Pharmacy")]  // Varsayılan yetki
    public class PharmacyController : Controller
    {
        private readonly AppDbContext _context;

        public PharmacyController(AppDbContext context)
        {
            _context = context;
        }

        // 🟢 ECZANE KAYIT FORMU (HERKESE AÇIK)
        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegisterRequest()
        {
            return View();
        }

        // 🟢 ECZANE KAYIT FORMU (POST) — HERKESE AÇIK
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterRequest(PharmacyRegistrationRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Status = "Pending";
            model.CreatedAt = DateTime.Now;

            _context.PharmacyRegistrationRequests.Add(model);
            await _context.SaveChangesAsync();
            Console.WriteLine("GELEN ŞİFRE: " + model.Password);

            return RedirectToAction("RegisterSuccess");
        }

        // 🟢 Kayıt başarılı ekranı da herkes görebilmeli
        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegisterSuccess()
        {
            return View();
        }

        // 🟡 SADECE ECZANE GİRİŞ YAPINCA ERİŞEBİLİR
        [Authorize(Roles = "Pharmacy")]
        public async Task<IActionResult> Dashboard()
        {
            var pharmacyId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
);

            var orders = await _context.Orders
                .Where(o => o.PharmacyId == pharmacyId)
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }
    }
}
