using CoffeeShopManagement.Data;
using CoffeeShopManagement.Models;
using CoffeeShopManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CoffeeShopManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly CoffeeShopDbContext _context;

        public HomeController(CoffeeShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // ==========================================
            // BEST SELLER
            // ==========================================

            var bestSeller = await _context.SanPhams
                .OrderBy(x => x.MaSp)
                .Take(8)
                .ToListAsync();


            // ==========================================
            // SẢN PHẨM KHUYẾN MÃI
            // ==========================================

            var promotionProducts = await _context.SanPhams
                .Where(x =>
                    x.DangKhuyenMai == true &&
                    x.GiaKhuyenMai.HasValue
                )
                .OrderBy(x => x.MaSp)
                .Take(4)
                .ToListAsync();


            // ==========================================
            // VIEW MODEL
            // ==========================================

            var vm = new HomeViewModel
            {
                BestSeller = bestSeller,
                PromotionProducts = promotionProducts
            };


            return View(vm);
        }


        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true
        )]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId =
                        Activity.Current?.Id ??
                        HttpContext.TraceIdentifier
                }
            );
        }
    }
}