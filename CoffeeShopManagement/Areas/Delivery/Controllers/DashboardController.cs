using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopManagement.Areas.Delivery.Controllers
{
    [Area("Delivery")]
    [Authorize(Roles = "GiaoHang")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}