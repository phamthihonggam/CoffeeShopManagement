using CoffeeShopManagement.Data;
using CoffeeShopManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CoffeeShopManagement.Areas.Delivery.Controllers
{
    [Area("Delivery")]
    [Authorize(Roles = "GiaoHang")]
    public class OrderController : Controller
    {
        private readonly CoffeeShopDbContext _context;


        public OrderController(
            CoffeeShopDbContext context)
        {
            _context = context;
        }


        // =========================================================
        // LẤY MÃ TÀI KHOẢN SHIPPER HIỆN TẠI
        // =========================================================

        private int? GetCurrentAccountId()
        {
            string? id =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );


            if (
                int.TryParse(
                    id,
                    out int maTaiKhoan
                )
            )
            {
                return maTaiKhoan;
            }


            // Fallback Session
            return HttpContext.Session
                .GetInt32("MaTaiKhoan");
        }


        // =========================================================
        // DANH SÁCH ĐƠN GIAO HÀNG
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var maTaiKhoan =
                GetCurrentAccountId();


            if (maTaiKhoan == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new
                    {
                        area = ""
                    }
                );
            }


            // =====================================================
            // ĐƠN CHƯA CÓ SHIPPER NHẬN
            // =====================================================

            var availableOrders =
                await _context.HoaDons

                    .AsNoTracking()

                    .Where(
                        x =>
                            x.MaTaiKhoanGiao == null
                            &&
                            x.TrangThai == "Chờ giao hàng"
                    )

                    .Include(
                        x => x.MaKhNavigation
                    )

                    .OrderBy(
                        x => x.NgayDat
                    )

                    .ToListAsync();


            // =====================================================
            // ĐƠN CỦA SHIPPER HIỆN TẠI
            // =====================================================

            var myOrders =
                await _context.HoaDons

                    .AsNoTracking()

                    .Where(
                        x =>
                            x.MaTaiKhoanGiao
                                == maTaiKhoan.Value

                            &&
                            (
                                x.TrangThai == "Chờ giao hàng"
                                ||
                                x.TrangThai == "Đang giao hàng"
                                ||
                                x.TrangThai ==
                                    "Yêu cầu hủy khi đang giao"
                                ||
                                x.TrangThai == "Giao thất bại"
                            )
                    )

                    .Include(
                        x => x.MaKhNavigation
                    )

                    .OrderByDescending(
                        x => x.NgayNhanGiao
                    )

                    .ThenByDescending(
                        x => x.NgayDat
                    )

                    .ToListAsync();


            ViewBag.AvailableOrders =
                availableOrders;


            ViewBag.MyOrders =
                myOrders;


            return View();
        }


        // =========================================================
        // NHẬN ĐƠN
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Receive(
            int id)
        {
            var maTaiKhoan =
                GetCurrentAccountId();


            if (maTaiKhoan == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new
                    {
                        area = ""
                    }
                );
            }


            var order =
                await _context.HoaDons
                    .FirstOrDefaultAsync(
                        x => x.MaHd == id
                    );


            if (order == null)
            {
                TempData["Toast"] =
                    "Không tìm thấy đơn hàng.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            // =====================================================
            // CHỈ NHẬN ĐƠN CHƯA CÓ SHIPPER
            // =====================================================

            if (order.MaTaiKhoanGiao != null)
            {
                TempData["Toast"] =
                    "Đơn hàng này đã có nhân viên giao hàng nhận.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            if (
                order.TrangThai !=
                "Chờ giao hàng"
            )
            {
                TempData["Toast"] =
                    "Đơn hàng này chưa sẵn sàng để giao.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            // =====================================================
            // GÁN SHIPPER
            // =====================================================

            order.MaTaiKhoanGiao =
                maTaiKhoan.Value;


            order.NgayNhanGiao =
                DateTime.Now;


            await _context.SaveChangesAsync();


            TempData["Toast"] =
                $"Bạn đã nhận đơn #HD{order.MaHd:D5}.";


            return RedirectToAction(
                nameof(Index)
            );
        }


        // =========================================================
        // BẮT ĐẦU GIAO
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartDelivery(
            int id)
        {
            var maTaiKhoan =
                GetCurrentAccountId();


            if (maTaiKhoan == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new
                    {
                        area = ""
                    }
                );
            }


            var order =
                await _context.HoaDons
                    .FirstOrDefaultAsync(
                        x =>
                            x.MaHd == id
                            &&
                            x.MaTaiKhoanGiao
                                == maTaiKhoan.Value
                    );


            if (order == null)
            {
                TempData["Toast"] =
                    "Không tìm thấy đơn hàng của bạn.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            if (
                order.TrangThai !=
                "Chờ giao hàng"
            )
            {
                TempData["Toast"] =
                    "Đơn hàng này không thể bắt đầu giao.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            order.TrangThai =
                "Đang giao hàng";


            order.NgayBatDauGiao =
                DateTime.Now;


            await _context.SaveChangesAsync();


            TempData["Toast"] =
                $"Đã bắt đầu giao đơn #HD{order.MaHd:D5}.";


            return RedirectToAction(
                nameof(Index)
            );
        }


        // =========================================================
        // GIAO THÀNH CÔNG
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delivered(
            int id)
        {
            var maTaiKhoan =
                GetCurrentAccountId();


            if (maTaiKhoan == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new
                    {
                        area = ""
                    }
                );
            }


            var order =
                await _context.HoaDons
                    .FirstOrDefaultAsync(
                        x =>
                            x.MaHd == id
                            &&
                            x.MaTaiKhoanGiao
                                == maTaiKhoan.Value
                    );


            if (order == null)
            {
                TempData["Toast"] =
                    "Không tìm thấy đơn hàng.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            if (
                order.TrangThai !=
                "Đang giao hàng"
            )
            {
                TempData["Toast"] =
                    "Đơn hàng này chưa ở trạng thái đang giao.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            // =====================================================
            // GIAO THÀNH CÔNG
            // =====================================================

            order.TrangThai =
                "Hoàn thành";


            order.NgayGiaoThanhCong =
                DateTime.Now;


            order.LyDoGiaoThatBai =
                null;


            await _context.SaveChangesAsync();


            TempData["Toast"] =
                $"Đơn #HD{order.MaHd:D5} đã giao thành công.";


            return RedirectToAction(
                nameof(Index)
            );
        }


        // =========================================================
        // GIAO THẤT BẠI
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Failed(
            int id,
            string lyDo)
        {
            var maTaiKhoan =
                GetCurrentAccountId();


            if (maTaiKhoan == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new
                    {
                        area = ""
                    }
                );
            }


            lyDo =
                lyDo?.Trim()
                ?? string.Empty;


            if (
                string.IsNullOrWhiteSpace(
                    lyDo
                )
            )
            {
                TempData["Toast"] =
                    "Vui lòng nhập lý do giao thất bại.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            if (lyDo.Length > 500)
            {
                TempData["Toast"] =
                    "Lý do giao thất bại tối đa 500 ký tự.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            var order =
                await _context.HoaDons
                    .FirstOrDefaultAsync(
                        x =>
                            x.MaHd == id
                            &&
                            x.MaTaiKhoanGiao
                                == maTaiKhoan.Value
                    );


            if (order == null)
            {
                TempData["Toast"] =
                    "Không tìm thấy đơn hàng.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            if (
                order.TrangThai !=
                "Đang giao hàng"
            )
            {
                TempData["Toast"] =
                    "Đơn hàng này không thể cập nhật giao thất bại.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            order.TrangThai =
                "Giao thất bại";


            order.LyDoGiaoThatBai =
                lyDo;


            await _context.SaveChangesAsync();


            TempData["Toast"] =
                $"Đã ghi nhận giao thất bại cho đơn #HD{order.MaHd:D5}.";


            return RedirectToAction(
                nameof(Index)
            );
        }


        // =========================================================
        // CHI TIẾT ĐƠN
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Details(
            int id)
        {
            var maTaiKhoan =
                GetCurrentAccountId();


            if (maTaiKhoan == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new
                    {
                        area = ""
                    }
                );
            }


            var order =
                await _context.HoaDons

                    .AsNoTracking()

                    .Include(
                        x => x.MaKhNavigation
                    )

                    .Include(
                        x => x.MaTaiKhoanGiaoNavigation
                    )

                    .Include(
                        x => x.ChiTietHoaDons
                    )
                        .ThenInclude(
                            x => x.MaSpNavigation
                        )

                    .Include(
                        x => x.ChiTietHoaDons
                    )
                        .ThenInclude(
                            x => x.MaComboNavigation
                        )

                    .FirstOrDefaultAsync(
                        x =>
                            x.MaHd == id

                            &&
                            (
                                x.MaTaiKhoanGiao
                                    == maTaiKhoan.Value

                                ||

                                (
                                    x.MaTaiKhoanGiao == null
                                    &&
                                    x.TrangThai ==
                                        "Chờ giao hàng"
                                )
                            )
                    );


            if (order == null)
            {
                TempData["Toast"] =
                    "Bạn không có quyền xem đơn hàng này.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            return View(
                order
            );
        }


        // =========================================================
        // YÊU CẦU HỦY KHI ĐANG GIAO
        //
        // Shipper KHÔNG tự duyệt hủy.
        // Chỉ xác nhận đơn vẫn chưa giao cho khách.
        // Admin/Nhân viên sẽ quyết định cuối cùng.
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmCancelRequest(
            int id)
        {
            var maTaiKhoan =
                GetCurrentAccountId();


            if (maTaiKhoan == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new
                    {
                        area = ""
                    }
                );
            }


            var order =
                await _context.HoaDons
                    .FirstOrDefaultAsync(
                        x =>
                            x.MaHd == id
                            &&
                            x.MaTaiKhoanGiao
                                == maTaiKhoan.Value
                    );


            if (order == null)
            {
                TempData["Toast"] =
                    "Không tìm thấy đơn hàng.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            if (
                order.TrangThai !=
                "Yêu cầu hủy khi đang giao"
            )
            {
                TempData["Toast"] =
                    "Đơn hàng hiện không có yêu cầu hủy khi đang giao.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            // =====================================================
            // Ở bước hiện tại CHƯA đổi thành Đã hủy.
            //
            // Giữ nguyên trạng thái để Admin/Nhân viên
            // nhìn thấy và duyệt.
            // =====================================================

            TempData["Toast"] =
                $"Đã ghi nhận yêu cầu hủy của đơn #HD{order.MaHd:D5}. "
                + "Vui lòng đưa đơn về cửa hàng và chờ Admin/Nhân viên xử lý.";


            return RedirectToAction(
                nameof(Index)
            );
        }
    }
}