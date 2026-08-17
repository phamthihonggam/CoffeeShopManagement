using CoffeeShopManagement.Data;
using CoffeeShopManagement.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize("STATISTICS_VIEW")]
    public class StatisticsController : Controller
    {
        private readonly CoffeeShopDbContext _context;

        public StatisticsController(
            CoffeeShopDbContext context)
        {
            _context = context;
        }


        // =========================================
        // THỐNG KÊ
        // =========================================

        public async Task<IActionResult> Index()
        {
            // =====================================
            // TỔNG ĐƠN HÀNG
            // =====================================

            var totalOrders =
                await _context.HoaDons.CountAsync();


            // =====================================
            // ĐƠN HOÀN THÀNH
            // =====================================

            var completedOrders =
                await _context.HoaDons.CountAsync(
                    x => x.TrangThai == "Hoàn thành"
                );


            // =====================================
            // ĐƠN CHỜ XÁC NHẬN
            // =====================================

            var pendingOrders =
                await _context.HoaDons.CountAsync(
                    x => x.TrangThai == "Chờ xác nhận"
                );


            // =====================================
            // ĐƠN ĐÃ HỦY
            // =====================================

            var cancelledOrders =
                await _context.HoaDons.CountAsync(
                    x => x.TrangThai == "Đã hủy"
                );


            // =====================================
            // TỔNG DOANH THU
            // CHỈ TÍNH ĐƠN ĐÃ HOÀN THÀNH
            // =====================================

            var totalRevenue =
                await _context.HoaDons

                    .Where(
                        x => x.TrangThai == "Hoàn thành"
                    )

                    .SumAsync(
                        x => x.TongTien ?? 0
                    );


            // =====================================
            // TỔNG KHÁCH HÀNG
            // =====================================

            var totalCustomers =
                await _context.KhachHangs.CountAsync();


            // =====================================
            // TỔNG SẢN PHẨM
            // =====================================

            var totalProducts =
                await _context.SanPhams.CountAsync();


            // =====================================
            // DOANH THU 7 NGÀY GẦN NHẤT
            // CHỈ TÍNH ĐƠN HOÀN THÀNH
            // =====================================

            var today =
                DateTime.Today;

            var startDate =
                today.AddDays(-6);


            var recentOrders =
                await _context.HoaDons

                    .Where(x =>
                        x.TrangThai == "Hoàn thành"
                        &&
                        x.NgayDat.HasValue
                        &&
                        x.NgayDat.Value.Date >= startDate
                        &&
                        x.NgayDat.Value.Date <= today
                    )

                    .ToListAsync();


            // =====================================
            // DỮ LIỆU BIỂU ĐỒ
            // =====================================

            var revenueLabels =
                new List<string>();

            var revenueValues =
                new List<decimal>();


            for (int i = 0; i < 7; i++)
            {
                var date =
                    startDate.AddDays(i);


                revenueLabels.Add(
                    date.ToString("dd/MM")
                );


                var revenue =
                    recentOrders

                        .Where(x =>
                            x.NgayDat.HasValue
                            &&
                            x.NgayDat.Value.Date == date
                        )

                        .Sum(
                            x => x.TongTien ?? 0
                        );


                revenueValues.Add(
                    revenue
                );
            }


            // =====================================
            // GỬI DỮ LIỆU QUA VIEW
            // =====================================

            ViewBag.TotalOrders =
                totalOrders;

            ViewBag.CompletedOrders =
                completedOrders;

            ViewBag.PendingOrders =
                pendingOrders;

            ViewBag.CancelledOrders =
                cancelledOrders;

            ViewBag.TotalRevenue =
                totalRevenue;

            ViewBag.TotalCustomers =
                totalCustomers;

            ViewBag.TotalProducts =
                totalProducts;

            ViewBag.RevenueLabels =
                revenueLabels;

            ViewBag.RevenueValues =
                revenueValues;


            return View();
        }
    }
}