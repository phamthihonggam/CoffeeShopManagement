using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopManagement.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
