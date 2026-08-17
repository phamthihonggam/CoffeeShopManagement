using CoffeeShopManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly CoffeeShopDbContext _context;


        public OrderController(
            CoffeeShopDbContext context)
        {
            _context = context;
        }


        // =========================================================
        // DANH SÁCH ĐƠN HÀNG
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            string? keyword,
            string? status)
        {
            var query =
                _context.HoaDons

                    .Include(
                        x => x.MaKhNavigation
                    )

                    .Include(
                        x => x.MaTaiKhoanGiaoNavigation
                    )

                    .AsQueryable();


            // =====================================================
            // TÌM KIẾM
            // =====================================================

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword =
                    keyword.Trim();


                query =
                    query.Where(
                        x =>
                            x.MaHd
                                .ToString()
                                .Contains(keyword)

                            ||

                            x.MaKhNavigation
                                .HoTen
                                .Contains(keyword)

                            ||

                            (
                                x.MaKhNavigation.Email != null
                                &&
                                x.MaKhNavigation.Email.Contains(keyword)
                            )

                            ||

                            (
                                x.MaKhNavigation.DienThoai != null
                                &&
                                x.MaKhNavigation.DienThoai.Contains(keyword)
                            )

                            ||

                            (
                                x.HoTenNguoiNhan != null
                                &&
                                x.HoTenNguoiNhan.Contains(keyword)
                            )

                            ||

                            (
                                x.DienThoaiNguoiNhan != null
                                &&
                                x.DienThoaiNguoiNhan.Contains(keyword)
                            )
                    );
            }


            // =====================================================
            // LỌC TRẠNG THÁI
            // =====================================================

            if (!string.IsNullOrWhiteSpace(status))
            {
                query =
                    query.Where(
                        x => x.TrangThai == status
                    );
            }


            // =====================================================
            // LẤY DANH SÁCH
            // =====================================================

            var orders =
                await query

                    .OrderByDescending(
                        x => x.NgayDat
                    )

                    .ThenByDescending(
                        x => x.MaHd
                    )

                    .ToListAsync();


            // =====================================================
            // GIỮ GIÁ TRỊ SEARCH / FILTER
            // =====================================================

            ViewBag.Keyword =
                keyword;

            ViewBag.Status =
                status;


            // =====================================================
            // THỐNG KÊ NHANH
            // =====================================================

            ViewBag.TotalOrders =
                await _context.HoaDons
                    .CountAsync();


            ViewBag.PendingOrders =
                await _context.HoaDons
                    .CountAsync(
                        x =>
                            x.TrangThai == "Chờ xác nhận"
                            ||
                            x.TrangThai == "Chờ xử lý"
                            ||
                            x.TrangThai == "Đã thanh toán"
                    );


            ViewBag.CompletedOrders =
                await _context.HoaDons
                    .CountAsync(
                        x =>
                            x.TrangThai == "Hoàn thành"
                    );


            ViewBag.TotalRevenue =
                await _context.HoaDons

                    .Where(
                        x =>
                            x.TrangThai == "Hoàn thành"
                    )

                    .SumAsync(
                        x => x.TongTien ?? 0
                    );


            ViewBag.CancelRequests =
                await _context.HoaDons
                    .CountAsync(
                        x =>
                            x.TrangThai == "Yêu cầu hủy"
                            ||
                            x.TrangThai == "Yêu cầu hủy khi đang giao"
                    );


            ViewBag.WaitingDelivery =
                await _context.HoaDons
                    .CountAsync(
                        x =>
                            x.TrangThai == "Chờ giao hàng"
                    );


            return View(
                orders
            );
        }


        // =========================================================
        // CHI TIẾT ĐƠN HÀNG
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Details(
            int id)
        {
            var order =
                await _context.HoaDons

                    // KHÁCH HÀNG
                    .Include(
                        x => x.MaKhNavigation
                    )

                    // NHÂN VIÊN GIAO HÀNG
                    .Include(
                        x => x.MaTaiKhoanGiaoNavigation
                    )

                    // CHI TIẾT HÓA ĐƠN + SẢN PHẨM
                    .Include(
                        x => x.ChiTietHoaDons
                    )
                        .ThenInclude(
                            x => x.MaSpNavigation
                        )

                    // CHI TIẾT HÓA ĐƠN + COMBO
                    .Include(
                        x => x.ChiTietHoaDons
                    )
                        .ThenInclude(
                            x => x.MaComboNavigation
                        )

                    .FirstOrDefaultAsync(
                        x => x.MaHd == id
                    );


            if (order == null)
            {
                return NotFound();
            }


            return View(
                order
            );
        }


        // =========================================================
        // CẬP NHẬT TRẠNG THÁI ĐƠN HÀNG
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int id,
            string trangThai)
        {
            var order =
                await _context.HoaDons
                    .FirstOrDefaultAsync(
                        x => x.MaHd == id
                    );


            if (order == null)
            {
                TempData["ErrorMessage"] =
                    "Không tìm thấy đơn hàng.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            string currentStatus =
                order.TrangThai?.Trim()
                ?? string.Empty;


            trangThai =
                trangThai?.Trim()
                ?? string.Empty;


            // =====================================================
            // KHÔNG CHO DÙNG UPDATE STATUS ĐỂ XỬ LÝ HỦY
            // =====================================================

            if (
                currentStatus == "Yêu cầu hủy"
                ||
                currentStatus == "Yêu cầu hủy khi đang giao"
            )
            {
                TempData["ErrorMessage"] =
                    "Đơn đang có yêu cầu hủy. Vui lòng dùng chức năng duyệt hoặc từ chối hủy.";

                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id = order.MaHd
                    }
                );
            }


            if (
                currentStatus == "Đã hủy"
                ||
                currentStatus == "Đã hủy - Chờ hoàn tiền"
                ||
                currentStatus == "Đã hủy - Đã hoàn tiền"
                ||
                currentStatus == "Hoàn thành"
            )
            {
                TempData["ErrorMessage"] =
                    "Đơn hàng đã kết thúc nên không thể cập nhật trạng thái.";

                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id = order.MaHd
                    }
                );
            }


            // =====================================================
            // DANH SÁCH TRẠNG THÁI ADMIN ĐƯỢC PHÉP CHUYỂN
            // =====================================================

            var validStatuses =
                new[]
                {
                    "Đã xác nhận",
                    "Đang xử lý",
                    "Đang chuẩn bị",
                    "Chờ giao hàng"
                };


            if (
                string.IsNullOrWhiteSpace(trangThai)
                ||
                !validStatuses.Contains(trangThai)
            )
            {
                TempData["ErrorMessage"] =
                    "Trạng thái đơn hàng không hợp lệ.";

                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id = order.MaHd
                    }
                );
            }


            // =====================================================
            // KIỂM TRA LUỒNG TRẠNG THÁI
            //
            // Checkout hiện có:
            // Chờ xác nhận (COD)
            // Đã thanh toán (VNPAY)
            //
            // Sau đó:
            // Đã xác nhận
            // -> Đang xử lý
            // -> Đang chuẩn bị
            // -> Chờ giao hàng
            // =====================================================

            bool transitionAllowed =
                false;


            if (
                currentStatus == "Chờ xác nhận"
                ||
                currentStatus == "Chờ xử lý"
                ||
                currentStatus == "Đã thanh toán"
            )
            {
                transitionAllowed =
                    trangThai == "Đã xác nhận";
            }
            else if (
                currentStatus == "Đã xác nhận"
            )
            {
                transitionAllowed =
                    trangThai == "Đang xử lý";
            }
            else if (
                currentStatus == "Đang xử lý"
            )
            {
                transitionAllowed =
                    trangThai == "Đang chuẩn bị";
            }
            else if (
                currentStatus == "Đang chuẩn bị"
            )
            {
                transitionAllowed =
                    trangThai == "Chờ giao hàng";
            }


            if (!transitionAllowed)
            {
                TempData["ErrorMessage"] =
                    $"Không thể chuyển đơn từ \"{currentStatus}\" sang \"{trangThai}\".";

                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id = order.MaHd
                    }
                );
            }


            // =====================================================
            // KHI CHUYỂN SANG CHỜ GIAO HÀNG
            // RESET THÔNG TIN SHIPPER CŨ NẾU CÓ
            // =====================================================

            if (
                trangThai == "Chờ giao hàng"
            )
            {
                order.MaTaiKhoanGiao =
                    null;

                order.NgayNhanGiao =
                    null;

                order.NgayBatDauGiao =
                    null;

                order.NgayGiaoThanhCong =
                    null;

                order.LyDoGiaoThatBai =
                    null;
            }


            order.TrangThai =
                trangThai;


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Đã cập nhật đơn hàng #HD{order.MaHd:D5} thành \"{trangThai}\".";


            return RedirectToAction(
                nameof(Details),
                new
                {
                    id = order.MaHd
                }
            );
        }


        // =========================================================
        // DUYỆT YÊU CẦU HỦY
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveCancel(
            int id)
        {
            var order =
                await _context.HoaDons
                    .FirstOrDefaultAsync(
                        x => x.MaHd == id
                    );


            if (order == null)
            {
                TempData["ErrorMessage"] =
                    "Không tìm thấy đơn hàng.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            string currentStatus =
                order.TrangThai?.Trim()
                ?? string.Empty;


            if (
                currentStatus != "Yêu cầu hủy"
                &&
                currentStatus != "Yêu cầu hủy khi đang giao"
            )
            {
                TempData["ErrorMessage"] =
                    "Đơn hàng hiện không có yêu cầu hủy cần duyệt.";

                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id = order.MaHd
                    }
                );
            }


            bool paidOnline =
                string.Equals(
                    order.PhuongThucThanhToan,
                    "VNPAY",
                    StringComparison.OrdinalIgnoreCase
                );


            order.NgayXuLyHuy =
                DateTime.Now;


            // =====================================================
            // ĐƠN ĐÃ THANH TOÁN ONLINE
            // =====================================================

            if (paidOnline)
            {
                order.TrangThai =
                    "Đã hủy - Chờ hoàn tiền";

                order.TrangThaiHoanTien =
                    "Chờ hoàn tiền";

                order.SoTienHoan =
                    order.TongTien ?? 0;

                order.NgayHoanTien =
                    null;
            }
            else
            {
                // =================================================
                // COD
                // =================================================

                order.TrangThai =
                    "Đã hủy";

                order.TrangThaiHoanTien =
                    "Không cần hoàn tiền";

                order.SoTienHoan =
                    0;

                order.NgayHoanTien =
                    null;
            }


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                paidOnline
                    ? $"Đã duyệt hủy đơn #HD{order.MaHd:D5}. Đơn đang chờ hoàn tiền."
                    : $"Đã duyệt hủy đơn #HD{order.MaHd:D5}.";


            return RedirectToAction(
                nameof(Details),
                new
                {
                    id = order.MaHd
                }
            );
        }


        // =========================================================
        // TỪ CHỐI YÊU CẦU HỦY
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectCancel(
            int id)
        {
            var order =
                await _context.HoaDons
                    .FirstOrDefaultAsync(
                        x => x.MaHd == id
                    );


            if (order == null)
            {
                TempData["ErrorMessage"] =
                    "Không tìm thấy đơn hàng.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            string currentStatus =
                order.TrangThai?.Trim()
                ?? string.Empty;


            if (
                currentStatus != "Yêu cầu hủy"
                &&
                currentStatus != "Yêu cầu hủy khi đang giao"
            )
            {
                TempData["ErrorMessage"] =
                    "Đơn hàng hiện không có yêu cầu hủy cần xử lý.";

                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id = order.MaHd
                    }
                );
            }


            // =====================================================
            // KHÔI PHỤC TRẠNG THÁI GẦN NHẤT
            //
            // Nếu đang giao và khách xin hủy
            // => quay lại Đang giao hàng.
            //
            // Nếu yêu cầu hủy trước giao
            // => quay lại Đang xử lý.
            //
            // Với đơn VNPAY vừa thanh toán xong mà yêu cầu hủy
            // => quay lại Đã thanh toán.
            // =====================================================

            if (
                currentStatus == "Yêu cầu hủy khi đang giao"
            )
            {
                order.TrangThai =
                    "Đang giao hàng";
            }
            else if (
                string.Equals(
                    order.PhuongThucThanhToan,
                    "VNPAY",
                    StringComparison.OrdinalIgnoreCase
                )
                &&
                !order.NgayBatDauGiao.HasValue
            )
            {
                order.TrangThai =
                    "Đã thanh toán";
            }
            else
            {
                order.TrangThai =
                    "Đang xử lý";
            }


            order.NgayXuLyHuy =
                DateTime.Now;


            // Yêu cầu bị từ chối thì không còn trạng thái hoàn tiền
            if (
                order.TrangThaiHoanTien == "Chờ hoàn tiền"
            )
            {
                order.TrangThaiHoanTien =
                    null;

                order.SoTienHoan =
                    null;

                order.NgayHoanTien =
                    null;
            }


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Đã từ chối yêu cầu hủy đơn #HD{order.MaHd:D5}.";


            return RedirectToAction(
                nameof(Details),
                new
                {
                    id = order.MaHd
                }
            );
        }


        // =========================================================
        // XÁC NHẬN ĐÃ HOÀN TIỀN
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmRefund(
            int id)
        {
            var order =
                await _context.HoaDons
                    .FirstOrDefaultAsync(
                        x => x.MaHd == id
                    );


            if (order == null)
            {
                TempData["ErrorMessage"] =
                    "Không tìm thấy đơn hàng.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            if (
                order.TrangThai != "Đã hủy - Chờ hoàn tiền"
                ||
                order.TrangThaiHoanTien != "Chờ hoàn tiền"
            )
            {
                TempData["ErrorMessage"] =
                    "Đơn hàng này hiện không ở trạng thái chờ hoàn tiền.";

                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id = order.MaHd
                    }
                );
            }


            order.TrangThai =
                "Đã hủy - Đã hoàn tiền";

            order.TrangThaiHoanTien =
                "Đã hoàn tiền";

            order.NgayHoanTien =
                DateTime.Now;

            order.NgayXuLyHuy ??=
                DateTime.Now;


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Đã xác nhận hoàn tiền cho đơn #HD{order.MaHd:D5}.";


            return RedirectToAction(
                nameof(Details),
                new
                {
                    id = order.MaHd
                }
            );
        }
    }
}