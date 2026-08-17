using CoffeeShopManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Controllers
{
    public class ProductsController : Controller
    {
        private readonly CoffeeShopDbContext _context;

        public ProductsController(CoffeeShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dsSanPham = await _context.SanPhams
                                         .Include(x => x.MaLoaiNavigation)
                                         .ToListAsync();

            return View(dsSanPham);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var sp = await _context.SanPhams
                                   .Include(x => x.MaLoaiNavigation)
                                   .FirstOrDefaultAsync(x => x.MaSp == id);

            if (sp == null)
                return NotFound();

            return View(sp);
        }
    }
}