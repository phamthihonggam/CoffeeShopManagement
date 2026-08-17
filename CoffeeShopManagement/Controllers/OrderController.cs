using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouseManagement.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}