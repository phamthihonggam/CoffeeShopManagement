using CoffeeShopManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly CoffeeShopDbContext _context;

        public UserController(CoffeeShopDbContext context)
        {
            _context = context;
        }


        // =========================================
        // DANH SÁCH KHÁCH HÀNG
        // =========================================

        public async Task<IActionResult> Index(
            string? keyword)
        {
            var query = _context.KhachHangs
                .Include(x => x.HoaDons)
                .AsQueryable();


            // =====================================
            // TÌM KIẾM
            // =====================================

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>

                    x.HoTen.Contains(keyword)

                    ||

                    (x.Email != null &&
                     x.Email.Contains(keyword))

                    ||

                    (x.DienThoai != null &&
                     x.DienThoai.Contains(keyword))
                );
            }


            // =====================================
            // DANH SÁCH
            // =====================================

            var customers = await query
                .OrderByDescending(x => x.NgayTao)
                .ThenByDescending(x => x.MaKh)
                .ToListAsync();


            ViewBag.Keyword =
                keyword;


            // =====================================
            // THỐNG KÊ
            // =====================================

            ViewBag.TotalCustomers =
                await _context.KhachHangs.CountAsync();


            ViewBag.CustomersWithOrders =
                await _context.KhachHangs
                    .CountAsync(x => x.HoaDons.Any());


            ViewBag.NewCustomers =
                await _context.KhachHangs
                    .CountAsync(
                        x => x.NgayTao >= DateTime.Today.AddDays(-30)
                    );


            return View(customers);
        }


        // =========================================
        // CHI TIẾT KHÁCH HÀNG
        // =========================================

        public async Task<IActionResult> Details(int id)
        {
            var customer =
                await _context.KhachHangs

                    .Include(x => x.HoaDons)

                    .FirstOrDefaultAsync(
                        x => x.MaKh == id
                    );


            if (customer == null)
            {
                return NotFound();
            }


            return View(customer);
        }
    }
}