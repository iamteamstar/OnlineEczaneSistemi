using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineEczaneSistemi.Data;
using OnlineEczaneSistemi.Models;
using OnlineEczaneSistemi.Models.ViewModels;

namespace OnlineEczaneSistemi.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly PasswordHasher<User> _passwordHasher;
        public bool DisableSignIn { get; set; } = false;

        public AccountController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
            _passwordHasher = new PasswordHasher<User>();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(Register register)
        {
            if (!ModelState.IsValid)
                return View(register);

            // Email zaten var mı?
            if (await _context.Users.AnyAsync(u => u.Email == register.Email))
            {
                ModelState.AddModelError("", "Bu e-posta ile kayıtlı bir kullanıcı var.");
                return View(register);
            }

            // Yeni kullanıcı oluştur
            var user = new User
            {
                FullName = register.FullName,
                Email = register.Email,
                Role = "User",
                IsActive = true
            };
            user.Password = _passwordHasher.HashPassword(user, register.Password);

            // Profil resmi yükleme
            if (register.ProfileImage != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads", "profiles");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + Path.GetExtension(register.ProfileImage.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await register.ProfileImage.CopyToAsync(stream);
                }

                user.ImageUrl = "/uploads/profiles/" + fileName;
            }

            // DB'ye kaydet
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

           

            return RedirectToAction("Index", "Home");

        }
    }
}
