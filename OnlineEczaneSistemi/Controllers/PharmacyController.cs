using Microsoft.AspNetCore.Mvc;

namespace OnlineEczaneSistemi.Controllers
{
    public class PharmacyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
