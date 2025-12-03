using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineEczaneSistemi.Data;
using OnlineEczaneSistemi.Models;
using OnlineEczaneSistemi.Models.ViewModels;
using System.Security.Claims;

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
        public IActionResult Register(string role)
        {
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("ChooseLogin");

            ViewBag.Role = role;

            // Eğer Eczane ise kendi formuna yönlendir
            if (role == "Pharmacy")
                return RedirectToAction("RegisterRequest", "Pharmacy");

            // Eğer Kurye ise kendi formuna yönlendir
            if (role == "Courier")
                return RedirectToAction("RegisterRequest", "Courier");

            // Default hasta kaydı
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Register(Register register, string role)
        {
            if (role != "User")
                return RedirectToAction("ChooseLogin"); // güvenlik için

            if (!ModelState.IsValid)
                return View(register);

            var user = new User
            {
                FullName = register.FullName,
                Email = register.Email,
                Role = "User",
                IsActive = true
            };

            user.Password = _passwordHasher.HashPassword(user, register.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await SignInUser(user);
            return RedirectToAction("Index", "Home");
        }


        public IActionResult Login(string role, string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(role))
            {
                // Eğer rol belirtilmediyse ChooseLogin sayfasına geri gönder
                return RedirectToAction("ChooseLogin");
            }

            ViewBag.Role = role;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login login, string? role, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(login);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "E-posta veya parola hatalı.");
                return View(login);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError("", "Bu hesap pasif durumdadır.");
                return View(login);
            }

            // ROLE gerekli DEĞİL — sadece role seçilmişse kontrol et
            if (!string.IsNullOrWhiteSpace(role) && user.Role != role)
            {
                ModelState.AddModelError("", "Bu hesap seçilen giriş türü ile uyumlu değildir.");
                return View(login);
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, login.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "E-posta veya parola hatalı.");
                return View(login);
            }

            await SignInUser(user, login.RememberMe);

            return user.Role switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Pharmacy" => RedirectToAction("Dashboard", "Pharmacy"),
                "Courier" => RedirectToAction("Dashboard", "Courier"),
                _ => RedirectToAction("Index", "Home")
            };
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult AccessDenied()//rol yetkisi
        {
            return View();
        }
        private async Task SignInUser(User user, bool rememberMe = false)
        {
            if (DisableSignIn)
                return; // Test modunda çalışmasın
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = rememberMe }
            );
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return NotFound();

            return View(user);
        }
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return NotFound();

            return View(user);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(User model, IFormFile? newImage, string? newPassword)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return NotFound();

            // Ad – Email güncelle
            user.FullName = model.FullName;
            user.Email = model.Email;

            // Yeni parola geldiyse
            if (!string.IsNullOrEmpty(newPassword))
            {
                user.Password = _passwordHasher.HashPassword(user, newPassword);
            }

            // Yeni profil resmi geldiyse
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

                user.ImageUrl = "/uploads/profiles/" + fileName;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Profile");
        }
        public IActionResult ChooseLogin()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
