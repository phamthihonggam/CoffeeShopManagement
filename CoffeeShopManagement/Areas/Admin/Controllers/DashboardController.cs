using CoffeeShopManagement.Data;
using CoffeeShopManagement.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class DashboardController : Controller
    {
        private readonly CoffeeShopDbContext _context;

        public DashboardController(
            CoffeeShopDbContext context)
        {
            _context = context;
        }


        // =====================================================
        // DASHBOARD ADMIN
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // =================================================
            // TỔNG SẢN PHẨM
            // =================================================

            var totalProducts =
                await _context.SanPhams
                    .CountAsync();


            // =================================================
            // TỔNG ĐƠN HÀNG
            // =================================================

            var totalOrders =
                await _context.HoaDons
                    .CountAsync();


            // =================================================
            // TỔNG KHÁCH HÀNG
            // =================================================

            var totalCustomers =
                await _context.KhachHangs
                    .CountAsync();


            // =================================================
            // TỔNG DOANH THU
            // =================================================

            var totalRevenue =
                await _context.HoaDons
                    .SumAsync(
                        x => (decimal?)x.TongTien
                    )
                ?? 0;


            // =================================================
            // TỔNG NHÂN VIÊN
            // =================================================

            var totalStaff =
                await _context.NhanViens
                    .CountAsync();


            // =================================================
            // GỬI DỮ LIỆU QUA VIEW
            // =================================================

            ViewBag.TotalProducts =
                totalProducts;

            ViewBag.TotalOrders =
                totalOrders;

            ViewBag.TotalCustomers =
                totalCustomers;

            ViewBag.TotalRevenue =
                totalRevenue;

            ViewBag.TotalStaff =
                totalStaff;


            return View();
        }
    }
}