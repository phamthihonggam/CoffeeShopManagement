using CoffeeShopManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Areas.Staff.Controllers
{
    // =========================================================
    // ROSALIE COFFEE
    // STAFF - CUSTOMER CONTROLLER
    // =========================================================

    [Area("Staff")]
    [Authorize(Roles = "NhanVien")]
    [Route("[area]/[controller]")]
    public class CustomerController : Controller
    {
        private readonly CoffeeShopDbContext _context;


        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public CustomerController(
            CoffeeShopDbContext context)
        {
            _context = context;
        }


        // =====================================================
        // DANH SÁCH KHÁCH HÀNG
        //
        // URL:
        // /Staff/Customer
        // /Staff/Customer/Index
        // =====================================================

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(
            string? keyword,
            int page = 1,
            int pageSize = 10)
        {
            // =================================================
            // PAGE
            // =================================================

            if (page < 1)
            {
                page = 1;
            }


            // Chỉ cho phép 10 / 20 / 50 dòng
            if (
                pageSize != 10
                &&
                pageSize != 20
                &&
                pageSize != 50
            )
            {
                pageSize = 10;
            }


            // =================================================
            // QUERY
            // =================================================

            var query =
                _context.KhachHangs
                    .Include(x => x.HoaDons)
                    .AsNoTracking()
                    .AsQueryable();


            // =================================================
            // SEARCH
            // Tên / Email / Số điện thoại
            // =================================================

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword =
                    keyword.Trim();


                query =
                    query.Where(x =>

                        x.HoTen.Contains(keyword)

                        ||

                        (
                            x.Email != null
                            &&
                            x.Email.Contains(keyword)
                        )

                        ||

                        (
                            x.DienThoai != null
                            &&
                            x.DienThoai.Contains(keyword)
                        )
                    );
            }


            // =================================================
            // TOTAL ITEMS
            // =================================================

            int totalItems =
                await query.CountAsync();


            // =================================================
            // TOTAL PAGES
            // =================================================

            int totalPages =
                totalItems == 0
                    ? 0
                    : (int)Math.Ceiling(
                        totalItems
                        /
                        (double)pageSize
                    );


            // Nếu page lớn hơn tổng trang
            if (
                totalPages > 0
                &&
                page > totalPages
            )
            {
                page =
                    totalPages;
            }


            // =================================================
            // CUSTOMER LIST
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
                        (page - 1)
                        *
                        pageSize
                    )

                    .Take(
                        pageSize
                    )

                    .ToListAsync();


            // =================================================
            // THỐNG KÊ
            // =================================================


            // Tổng khách hàng
            ViewBag.TotalCustomers =
                await _context
                    .KhachHangs
                    .CountAsync();


            // Khách đã từng đặt hàng
            ViewBag.CustomersWithOrders =
                await _context
                    .KhachHangs
                    .CountAsync(
                        x => x.HoaDons.Any()
                    );


            // Khách mới trong 30 ngày
            ViewBag.NewCustomers =
                await _context
                    .KhachHangs
                    .CountAsync(
                        x =>
                            x.NgayTao
                            >=
                            DateTime.Today
                                .AddDays(-30)
                    );


            // =================================================
            // PAGINATION INFO
            // =================================================

            int startItem =
                totalItems == 0
                    ? 0
                    : (
                        (page - 1)
                        *
                        pageSize
                    )
                    + 1;


            int endItem =
                Math.Min(
                    page * pageSize,
                    totalItems
                );


            // =================================================
            // VIEWBAG
            // =================================================

            ViewBag.Keyword =
                keyword
                ??
                string.Empty;


            ViewBag.CurrentPage =
                page;


            ViewBag.PageSize =
                pageSize;


            ViewBag.TotalItems =
                totalItems;


            ViewBag.TotalPages =
                totalPages;


            ViewBag.StartItem =
                startItem;


            ViewBag.EndItem =
                endItem;


            // =================================================
            // RETURN VIEW
            // =================================================

            return View(
                customers
            );
        }


        // =====================================================
        // CHI TIẾT KHÁCH HÀNG
        //
        // URL:
        // /Staff/Customer/Details/1
        // =====================================================

        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(
            int id)
        {
            // =================================================
            // FIND CUSTOMER
            // =================================================

            var customer =
                await _context
                    .KhachHangs

                    .Include(
                        x => x.HoaDons
                    )

                    .AsNoTracking()

                    .FirstOrDefaultAsync(
                        x => x.MaKh == id
                    );


            // =================================================
            // NOT FOUND
            // =================================================

            if (customer == null)
            {
                return NotFound();
            }


            // =================================================
            // RETURN VIEW
            // =================================================

            return View(
                customer
            );
        }
    }
}