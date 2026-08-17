using CoffeeShopManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Controllers
{
    public class ContactController : Controller
    {
        private readonly CoffeeShopDbContext _context;

        public ContactController(CoffeeShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var branches = await _context.ChiNhanhs
                .Where(x => x.TrangThai)
                .OrderBy(x => x.Quan)
                .ToListAsync();

            return View(branches);
        }
    }
}