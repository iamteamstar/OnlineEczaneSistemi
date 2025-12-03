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
    .Include(o => o.User)          // HASTA BİLGİSİ GELMELİ
    .Include(o => o.Items)         // Sipariş kalemleri
    .OrderByDescending(o => o.CreatedAt)
    .ToListAsync();

            return View(orders);
        }
        [Authorize(Roles = "Pharmacy")]
        public async Task<IActionResult> Orders()
        {
            var pharmacyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var orders = await _context.Orders
                .Where(o => o.PharmacyId == pharmacyId)
                .Include(o => o.User)
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }
        [Authorize(Roles = "Pharmacy")]
        public async Task<IActionResult> PriceOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            return View(order);
        }
        [HttpPost]
        [Authorize(Roles = "Pharmacy")]
        public async Task<IActionResult> PriceOrder(int OrderId, string Notes, decimal TotalPrice)
        {
            var order = await _context.Orders.FindAsync(OrderId);
            if (order == null) return NotFound();

            order.Notes = Notes;
            order.TotalPrice = TotalPrice;
            order.Status = "Preparing";

            await _context.SaveChangesAsync();

            return RedirectToAction("Orders");
        }
        [Authorize(Roles = "Pharmacy")]
        public async Task<IActionResult> CallCourier(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            order.Status = "WaitingCourier";

            await _context.SaveChangesAsync();

            return RedirectToAction("Orders");
        }
        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            var pharmacyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // 📌 Eczanenin stoktaki ilaçları
            ViewBag.Medicines = await _context.Medicines
                .Where(m => m.PharmacyId == pharmacyId && m.Stock > 0)
                .ToListAsync();

            return View(order);
        }


        [HttpPost]
        public async Task<IActionResult> AddItem(int OrderId, int MedicineId, int Quantity)
        {
            var med = await _context.Medicines.FindAsync(MedicineId);
            if (med == null) return BadRequest("İlaç bulunamadı.");

            if (med.Stock < Quantity)
                return BadRequest("Yetersiz stok!");

            // Stok düş
            med.Stock -= Quantity;

            var item = new OrderItem
            {
                OrderId = OrderId,
                MedicineName = med.Name,
                Quantity = Quantity,
                UnitPrice = med.Price
            };

            _context.OrderItems.Add(item);

            // Sipariş toplamını güncelle
            var order = await _context.Orders.FindAsync(OrderId);
            order.TotalPrice += (med.Price * Quantity);

            await _context.SaveChangesAsync();

            return RedirectToAction("OrderDetail", new { id = OrderId });
        }


        public async Task<IActionResult> StartPreparing(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            order.Status = "Preparing";
            await _context.SaveChangesAsync();
            return RedirectToAction("OrderDetail", new { id });
        }

        public async Task<IActionResult> CompletePreparing(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            // 🔥 Sistemde aktif bir kurye bulalım
            var courier = await _context.Users
                .Where(u => u.Role == "Courier" && u.IsActive)
                .OrderBy(u => u.CreatedAt) // İleride akıllı dağıtım için düzenlenebilir
                .FirstOrDefaultAsync();

            if (courier == null)
            {
                TempData["Error"] = "Sistemde aktif bir kurye bulunamadı!";
                return RedirectToAction("OrderDetail", new { id });
            }

            // 🔥 Kurye atanıyor (en kritik satır!)
            order.CourierId = courier.UserId;

            // Sipariş durumu kurye bekliyor'a alınır
            order.Status = "WaitingCourier";

            await _context.SaveChangesAsync();

            return RedirectToAction("OrderDetail", new { id });
        }

        [Authorize(Roles = "Pharmacy")]
        public async Task<IActionResult> Stock()
        {
            var pharmacyId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var medicines = await _context.Medicines
                .Where(m => m.PharmacyId == pharmacyId)
                .ToListAsync();

            return View(medicines);
        }

        [HttpPost]
        public async Task<IActionResult> AddMedicine(string Name, int Stock, decimal Price)
        {
            var pharmacyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var m = new Medicine
            {
                Name = Name,
                Stock = Stock,
                Price = Price,
                PharmacyId = pharmacyId
            };

            _context.Medicines.Add(m);
            await _context.SaveChangesAsync();

            return RedirectToAction("Stock");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStock(int MedicineId, int Stock)
        {
            var medicine = await _context.Medicines.FindAsync(MedicineId);
            medicine.Stock = Stock;

            await _context.SaveChangesAsync();
            return RedirectToAction("Stock");
        }
        public async Task<IActionResult> DeleteMedicine(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            _context.Medicines.Remove(medicine);

            await _context.SaveChangesAsync();
            return RedirectToAction("Stock");
        }


    }
}
