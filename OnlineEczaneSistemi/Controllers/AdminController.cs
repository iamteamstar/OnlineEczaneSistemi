using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AdminController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // -------------------------------------------------------
        // 📌 DASHBOARD
        // -------------------------------------------------------
        public async Task<IActionResult> Dashboard()
        {
            var model = new AdminDashboard
            {
                TotalUsers = await _context.Users.CountAsync(),
                ActiveUsers = await _context.Users.CountAsync(u => u.IsActive),
                PassiveUsers = await _context.Users.CountAsync(u => !u.IsActive),
                AdminCount = await _context.Users.CountAsync(u => u.Role == "Admin"),
                AdminRoleCount = await _context.Users.CountAsync(u => u.Role == "Admin"),
                UserRoleCount = await _context.Users.CountAsync(u => u.Role == "User"),
                PharmacyRoleCount = await _context.Users.CountAsync(u => u.Role == "Pharmacy"),
                CourierRoleCount = await _context.Users.CountAsync(u => u.Role == "Courier"),
                LastUsers = await _context.Users.OrderByDescending(u => u.CreatedAt).Take(5).ToListAsync()
            };

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
        // 📌 USERS LIST
        // -------------------------------------------------------
        public async Task<IActionResult> Index(string search, string role, string status)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));

            if (!string.IsNullOrEmpty(role) && role != "all")
                query = query.Where(u => u.Role == role);

            if (!string.IsNullOrEmpty(status) && status != "all")
                query = query.Where(u => status == "active" ? u.IsActive : !u.IsActive);

            var users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync();
            return View(users);
        }

        // -------------------------------------------------------
        // 📌 TOGGLE STATUS
        // -------------------------------------------------------
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // -------------------------------------------------------
        // 📌 ROLE CHANGES
        // -------------------------------------------------------
        public async Task<IActionResult> MakeAdmin(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Role = "Admin";
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MakeUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Role = "User";
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // -------------------------------------------------------
        // 📌 DETAIL & EDIT
        // -------------------------------------------------------
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, User model, IFormFile? newImage)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Role = model.Role;
            user.IsActive = model.IsActive;

            if (newImage != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads", "profiles");
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
        // 📌 ECZANE & KURYE REQUESTS
        // -------------------------------------------------------
        public async Task<IActionResult> PharmacyRequests()
        {
            var list = await _context.PharmacyRegistrationRequests.OrderByDescending(x => x.CreatedAt).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> CourierRequests()
        {
            var list = await _context.CourierRegistrationRequests.OrderByDescending(x => x.CreatedAt).ToListAsync();
            return View(list);
        }

        // -------------------------------------------------------
        // 📌 APPROVE PHARMACY
        // -------------------------------------------------------
        public async Task<IActionResult> ApprovePharmacy(int id)
        {
            var req = await _context.PharmacyRegistrationRequests.FindAsync(id);
            if (req == null) return NotFound();

            req.Status = "Approved";

            var user = new User
            {
                FullName = req.PharmacyName,
                Email = req.Email,
                Role = "Pharmacy",
                IsActive = true
            };

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, req.Password);
            Console.WriteLine("ADMİN ONAYI - ŞİFRE: " + req.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("PharmacyRequests");
        }


        // -------------------------------------------------------
        // 📌 APPROVE COURIER
        // -------------------------------------------------------
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
                IsActive = true
            };

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, req.Password);

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
