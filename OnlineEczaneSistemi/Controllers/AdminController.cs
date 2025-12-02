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
        public async Task<IActionResult> Dashboard()
        {
            var model = new AdminDashboard();

            model.TotalUsers = await _context.Users.CountAsync();
            model.ActiveUsers = await _context.Users.Where(u => u.IsActive).CountAsync();
            model.PassiveUsers = await _context.Users.Where(u => !u.IsActive).CountAsync();
            model.AdminCount = await _context.Users.Where(u => u.Role == "Admin").CountAsync();

            model.AdminRoleCount = await _context.Users.CountAsync(u => u.Role == "Admin");
            model.UserRoleCount = await _context.Users.CountAsync(u => u.Role == "User");

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

        public async Task<IActionResult> Index(string search, string role, string status)
        {
            var query = _context.Users.AsQueryable();

            // ARAMA (Ad / Email)
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u =>
                    u.FullName.Contains(search) ||
                    u.Email.Contains(search));
            }

            // ROLE GÖRE FİLTRELEME
            if (!string.IsNullOrEmpty(role) && role != "all")
            {
                query = query.Where(u => u.Role == role);
            }

            // DURUMA GÖRE FİLTRELEME
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

        public async Task<IActionResult> ToggleStatus(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MakeAdmin(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
                return NotFound();

            user.Role = "Admin";
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> MakeUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
                return NotFound();

            user.Role = "User";
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
                return NotFound();

            return View(user);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
                return NotFound();

            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, User model, IFormFile? newImage)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
                return NotFound();

            // Temel bilgiler
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Role = model.Role;
            user.IsActive = model.IsActive;

            // Yeni resim yüklendiyse
            if (newImage != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads", "profiles");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + Path.GetExtension(newImage.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await newImage.CopyToAsync(stream);
                }

                // Eski resmi silmek istersen buraya kod koyabiliriz

                user.ImageUrl = "/uploads/profiles/" + fileName;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = user.UserId });
        }

    }
}
