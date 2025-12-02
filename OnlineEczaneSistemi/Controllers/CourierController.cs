using Microsoft.AspNetCore.Mvc;

namespace OnlineEczaneSistemi.Controllers
{
    public class CourierController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
