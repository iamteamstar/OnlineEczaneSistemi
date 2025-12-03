using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineEczaneSistemi.Data;
using OnlineEczaneSistemi.Models;
using OnlineEczaneSistemi.Models.ViewModels;
using System.Security.Claims;

namespace OnlineEczaneSistemi.Controllers
{
    [Authorize(Roles = "User")]  // sadece hasta giriş yapınca erişir
    public class PatientController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PatientController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 📌 Reçete yükleme formu (GET)
        [HttpGet]
        public async Task<IActionResult> UploadPrescription()
        {
            var pharmacies = await _context.Users
                .Where(u => u.Role == "Pharmacy" && u.IsActive)
                .ToListAsync();

            var vm = new PrescriptionUploadVM
            {
                Pharmacies = pharmacies
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UploadPrescription(PrescriptionUploadVM model)
        {
            // Bu satır sorunu kesin çözer
            ModelState.Remove("Pharmacies");

            if (!ModelState.IsValid)
            {
                model.Pharmacies = await _context.Users
                    .Where(u => u.Role == "Pharmacy" && u.IsActive)
                    .ToListAsync();

                return View(model);
            }

            string fileUrl = null;

            if (model.PrescriptionFile != null)
            {
                var folder = Path.Combine(_env.WebRootPath, "prescriptions");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(model.PrescriptionFile.FileName);
                var filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await model.PrescriptionFile.CopyToAsync(stream);

                fileUrl = "/prescriptions/" + fileName;
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var order = new Order
            {
                UserId = userId,
                PharmacyId = model.PharmacyId,
                Notes = model.Notes,
                DeliveryAddress = model.DeliveryAddress,
                PrescriptionUrl = fileUrl,
                Status = "PendingPharmacy",
                CreatedAt = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderSuccess");
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}
