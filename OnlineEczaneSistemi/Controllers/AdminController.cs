using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineEczaneSistemi.Data;
using OnlineEczaneSistemi.Models;
using OnlineEczaneSistemi.Models.ViewModels;

namespace OnlineEczaneSistemi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // -------------------------------------------------------
        // 📌 DASHBOARD (Yeni rollerle genişletildi!)
        // -------------------------------------------------------
        public async Task<IActionResult> Dashboard()
        {
            var model = new AdminDashboard();

            model.TotalUsers = await _context.Users.CountAsync();
            model.ActiveUsers = await _context.Users.Where(u => u.IsActive).CountAsync();
            model.PassiveUsers = await _context.Users.Where(u => !u.IsActive).CountAsync();
            model.AdminCount = await _context.Users.Where(u => u.Role == "Admin").CountAsync();

            model.AdminRoleCount = await _context.Users.CountAsync(u => u.Role == "Admin");
            model.UserRoleCount = await _context.Users.CountAsync(u => u.Role == "User");

            // Yeni roller
            model.PharmacyRoleCount = await _context.Users.CountAsync(u => u.Role == "Pharmacy");
            model.CourierRoleCount = await _context.Users.CountAsync(u => u.Role == "Courier");

            model.LastUsers = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToListAsync();

            var today = DateTime.Now.Date;

            model.Last7DaysStats = await _context.Users
                .Where(u => u.CreatedAt >= today.AddDays(-6))
                .GroupBy(u => u.CreatedAt.Date)
                .Select(g => new DailyRegisterStats
                {
                    Date = g.Key.ToString("dd.MM"),
                    Count = g.Count()
                })
                .ToListAsync();

            return View(model);
        }

        // -------------------------------------------------------
        // 📌 KULLANICI LİSTELEME (Pharmacy & Courier eklenmesine hazır)
        // -------------------------------------------------------
        public async Task<IActionResult> Index(string search, string role, string status)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u =>
                    u.FullName.Contains(search) ||
                    u.Email.Contains(search));
            }

            if (!string.IsNullOrEmpty(role) && role != "all")
            {
                query = query.Where(u => u.Role == role);
            }

            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                if (status == "active")
                    query = query.Where(u => u.IsActive);
                else if (status == "passive")
                    query = query.Where(u => !u.IsActive);
            }

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            return View(users);
        }

        // -------------------------------------------------------
        // 📌 KULLANICI DURUMU DEĞİŞTİRME
        // -------------------------------------------------------
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MakeAdmin(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound();

            user.Role = "Admin";
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MakeUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound();

            user.Role = "User";
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound();

            return View(user);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, User model, IFormFile? newImage)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Role = model.Role;
            user.IsActive = model.IsActive;

            if (newImage != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads", "profiles");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + Path.GetExtension(newImage.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await newImage.CopyToAsync(stream);

                user.ImageUrl = "/uploads/profiles/" + fileName;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = user.UserId });
        }

        // -------------------------------------------------------
        // 📌 ECZANE & KURYE KAYIT TALEBİ YÖNETİMİ
        // -------------------------------------------------------

        // Eczane başvurularını listele
        public async Task<IActionResult> PharmacyRequests()
        {
            var list = await _context.PharmacyRegistrationRequests
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return View(list);
        }

        // Kurye başvurularını listele
        public async Task<IActionResult> CourierRequests()
        {
            var list = await _context.CourierRegistrationRequests
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return View(list);
        }

        // Eczane başvurusunu onayla
        public async Task<IActionResult> ApprovePharmacy(int id)
        {
            var req = await _context.PharmacyRegistrationRequests.FindAsync(id);
            if (req == null) return NotFound();

            req.Status = "Approved";

            // Otomatik kullanıcı oluştur
            var user = new User
            {
                FullName = req.PharmacyName,
                Email = req.Email,
                Role = "Pharmacy",
                IsActive = true,
                Password = "123456" // Admin daha sonra değiştirilmesini sağlayabilir
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("PharmacyRequests");
        }

        // Kurye başvurusunu onayla
        public async Task<IActionResult> ApproveCourier(int id)
        {
            var req = await _context.CourierRegistrationRequests.FindAsync(id);
            if (req == null) return NotFound();

            req.Status = "Approved";

            var user = new User
            {
                FullName = req.FullName,
                Email = req.Email,
                Role = "Courier",
                IsActive = true,
                Password = "123456"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("CourierRequests");
        }

        public async Task<IActionResult> RejectPharmacy(int id)
        {
            var req = await _context.PharmacyRegistrationRequests.FindAsync(id);
            if (req == null) return NotFound();

            req.Status = "Rejected";
            await _context.SaveChangesAsync();

            return RedirectToAction("PharmacyRequests");
        }

        public async Task<IActionResult> RejectCourier(int id)
        {
            var req = await _context.CourierRegistrationRequests.FindAsync(id);
            if (req == null) return NotFound();

            req.Status = "Rejected";
            await _context.SaveChangesAsync();

            return RedirectToAction("CourierRequests");
        }
    }
}
