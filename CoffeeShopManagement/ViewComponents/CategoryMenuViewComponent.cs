using CoffeeShopManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly CoffeeShopDbContext _context;

        public CategoryMenuViewComponent(CoffeeShopDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.LoaiSanPhams.ToListAsync();

            return View(categories);
        }
    }
}