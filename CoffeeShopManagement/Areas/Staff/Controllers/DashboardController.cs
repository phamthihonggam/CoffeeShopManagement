using CoffeeShopManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "NhanVien")]
    public class DashboardController : Controller
    {
        private readonly CoffeeShopDbContext _context;

        public DashboardController(
            CoffeeShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // =========================================
            // TỔNG ĐƠN HÀNG
            // =========================================

            var totalOrders =
                await _context.HoaDons
                    .CountAsync();


            // =========================================
            // TỔNG SẢN PHẨM
            // =========================================

            var totalProducts =
                await _context.SanPhams
                    .CountAsync();


            // =========================================
            // TỔNG KHÁCH HÀNG
            // =========================================

            var totalCustomers =
                await _context.KhachHangs
                    .CountAsync();


            // =========================================
            // DOANH THU
            // =========================================

            var totalRevenue =
                await _context.HoaDons
                    .SumAsync(
                        x => (decimal?)x.TongTien
                    )
                ?? 0;


            // =========================================
            // TÊN NHÂN VIÊN
            // =========================================

            var staffName =
                HttpContext.Session
                    .GetString("HoTen")
                ?? User.Identity?.Name
                ?? "Nhân viên";


            // =========================================
            // VIEWBAG
            // =========================================

            ViewBag.TotalOrders =
                totalOrders;

            ViewBag.TotalProducts =
                totalProducts;

            ViewBag.TotalCustomers =
                totalCustomers;

            ViewBag.TotalRevenue =
                totalRevenue;

            ViewBag.StaffName =
                staffName;


            return View();
        }
    }
}