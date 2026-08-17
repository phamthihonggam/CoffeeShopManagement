using CoffeeShopManagement.Data;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopManagement.Controllers
{
    public class TestController : Controller
    {
        private readonly CoffeeShopDbContext _context;

        public TestController(CoffeeShopDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var count = _context.LoaiSanPhams.Count();

            return Content($"Có {count} loại sản phẩm");
        }
    }
}