using CoffeeShopManagement.Data;
using CoffeeShopManagement.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class UserController : Controller
    {
        private readonly CoffeeShopDbContext _context;

        public UserController(
            CoffeeShopDbContext context)
        {
            _context = context;
        }


        // =====================================================
        // DANH SÁCH KHÁCH HÀNG
        // SEARCH + PAGINATION
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            string? keyword,
            int page = 1,
            int pageSize = 10)
        {
            if (page < 1)
            {
                page = 1;
            }


            var allowedPageSizes =
                new[]
                {
                    10,
                    20,
                    50
                };


            if (!allowedPageSizes.Contains(pageSize))
            {
                pageSize = 10;
            }


            var query =
                _context.KhachHangs
                    .Include(x => x.HoaDons)
                    .AsNoTracking()
                    .AsQueryable();


            // =================================================
            // TÌM KIẾM
            // =================================================

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword =
                    keyword.Trim();


                query =
                    query.Where(x =>

                        x.HoTen.Contains(keyword)

                        ||

                        (x.Email != null &&
                         x.Email.Contains(keyword))

                        ||

                        (x.DienThoai != null &&
                         x.DienThoai.Contains(keyword))
                    );
            }


            // =================================================
            // TỔNG KẾT QUẢ SAU KHI TÌM
            // =================================================

            var totalItems =
                await query.CountAsync();


            var totalPages =
                (int)Math.Ceiling(
                    totalItems / (double)pageSize
                );


            if (totalPages > 0 &&
                page > totalPages)
            {
                page =
                    totalPages;
            }


            // =================================================
            // PHÂN TRANG
            // =================================================

            var customers =
                await query
                    .OrderByDescending(
                        x => x.NgayTao
                    )
                    .ThenByDescending(
                        x => x.MaKh
                    )
                    .Skip(
                        (page - 1) * pageSize
                    )
                    .Take(pageSize)
                    .ToListAsync();


            // =================================================
            // SEARCH / PAGINATION
            // =================================================

            ViewBag.Keyword =
                keyword;

            ViewBag.CurrentPage =
                page;

            ViewBag.PageSize =
                pageSize;

            ViewBag.TotalItems =
                totalItems;

            ViewBag.TotalPages =
                totalPages;

            ViewBag.StartItem =
                totalItems == 0
                    ? 0
                    : ((page - 1) * pageSize) + 1;

            ViewBag.EndItem =
                Math.Min(
                    page * pageSize,
                    totalItems
                );


            // =================================================
            // THỐNG KÊ TOÀN HỆ THỐNG
            // =================================================

            ViewBag.TotalCustomers =
                await _context.KhachHangs
                    .CountAsync();


            ViewBag.CustomersWithOrders =
                await _context.KhachHangs
                    .CountAsync(
                        x => x.HoaDons.Any()
                    );


            ViewBag.NewCustomers =
                await _context.KhachHangs
                    .CountAsync(
                        x =>
                            x.NgayTao >=
                            DateTime.Today.AddDays(-30)
                    );


            return View(
                customers
            );
        }


        // =====================================================
        // CHI TIẾT KHÁCH HÀNG
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var customer =
                await _context.KhachHangs
                    .Include(x => x.HoaDons)
                    .AsNoTracking()
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