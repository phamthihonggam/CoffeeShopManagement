using CoffeeShopManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "NhanVien")]
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
        // SEARCH + FILTER + PAGINATION
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            string? keyword,
            string? status,
            int page = 1,
            int pageSize = 10)
        {
            // =====================================================
            // PAGE
            // =====================================================

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


            // =====================================================
            // QUERY
            // =====================================================

            var query =
                _context
                    .HoaDons

                    .Include(
                        x => x.MaKhNavigation
                    )

                    .Include(
                        x => x.MaTaiKhoanGiaoNavigation
                    )

                    .AsNoTracking()

                    .AsQueryable();


            // =====================================================
            // SEARCH
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

                            (
                                x.MaKhNavigation != null
                                &&
                                x.MaKhNavigation.HoTen
                                    .Contains(keyword)
                            )

                            ||

                            (
                                x.MaKhNavigation != null
                                &&
                                x.MaKhNavigation.Email != null
                                &&
                                x.MaKhNavigation.Email
                                    .Contains(keyword)
                            )

                            ||

                            (
                                x.MaKhNavigation != null
                                &&
                                x.MaKhNavigation.DienThoai != null
                                &&
                                x.MaKhNavigation.DienThoai
                                    .Contains(keyword)
                            )

                            ||

                            (
                                x.HoTenNguoiNhan != null
                                &&
                                x.HoTenNguoiNhan
                                    .Contains(keyword)
                            )

                            ||

                            (
                                x.DienThoaiNguoiNhan != null
                                &&
                                x.DienThoaiNguoiNhan
                                    .Contains(keyword)
                            )
                    );
            }


            // =====================================================
            // FILTER STATUS
            // =====================================================

            if (!string.IsNullOrWhiteSpace(status))
            {
                status =
                    status.Trim();


                query =
                    query.Where(
                        x =>
                            x.TrangThai == status
                    );
            }


            // =====================================================
            // TOTAL
            // =====================================================

            var totalItems =
                await query.CountAsync();


            var totalPages =
                (int)Math.Ceiling(
                    totalItems
                    /
                    (double)pageSize
                );


            if (
                totalPages > 0
                &&
                page > totalPages
            )
            {
                page =
                    totalPages;
            }


            // =====================================================
            // PAGINATION
            // =====================================================

            var orders =
                await query

                    .OrderByDescending(
                        x => x.NgayDat
                    )

                    .ThenByDescending(
                        x => x.MaHd
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


            // =====================================================
            // SEARCH / FILTER INFO
            // =====================================================

            ViewBag.Keyword =
                keyword;

            ViewBag.Status =
                status;


            // =====================================================
            // PAGINATION INFO
            // =====================================================

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


            // =====================================================
            // QUICK STATISTICS
            // =====================================================

            ViewBag.TotalOrders =
                await _context
                    .HoaDons
                    .CountAsync();


            ViewBag.PendingOrders =
                await _context
                    .HoaDons
                    .CountAsync(
                        x =>
                            x.TrangThai == "Chờ xác nhận"
                            ||
                            x.TrangThai == "Chờ xử lý"
                            ||
                            x.TrangThai == "Đã thanh toán"
                    );


            ViewBag.CompletedOrders =
                await _context
                    .HoaDons
                    .CountAsync(
                        x =>
                            x.TrangThai == "Hoàn thành"
                    );


            ViewBag.TotalRevenue =
                await _context
                    .HoaDons

                    .Where(
                        x =>
                            x.TrangThai == "Hoàn thành"
                    )

                    .SumAsync(
                        x =>
                            x.TongTien ?? 0
                    );


            ViewBag.CancelRequests =
                await _context
                    .HoaDons
                    .CountAsync(
                        x =>
                            x.TrangThai == "Yêu cầu hủy"
                            ||
                            x.TrangThai ==
                                "Yêu cầu hủy khi đang giao"
                    );


            ViewBag.WaitingDelivery =
                await _context
                    .HoaDons
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
                await _context
                    .HoaDons

                    // KHÁCH HÀNG
                    .Include(
                        x => x.MaKhNavigation
                    )

                    // NHÂN VIÊN GIAO HÀNG
                    .Include(
                        x => x.MaTaiKhoanGiaoNavigation
                    )

                    // SẢN PHẨM
                    .Include(
                        x => x.ChiTietHoaDons
                    )
                    .ThenInclude(
                        x => x.MaSpNavigation
                    )

                    // COMBO
                    .Include(
                        x => x.ChiTietHoaDons
                    )
                    .ThenInclude(
                        x => x.MaComboNavigation
                    )

                    .AsNoTracking()

                    .FirstOrDefaultAsync(
                        x =>
                            x.MaHd == id
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
        // CẬP NHẬT TRẠNG THÁI
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int id,
            string trangThai)
        {
            var order =
                await _context
                    .HoaDons
                    .FirstOrDefaultAsync(
                        x =>
                            x.MaHd == id
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
                ??
                string.Empty;


            trangThai =
                trangThai?.Trim()
                ??
                string.Empty;


            // =====================================================
            // ĐANG CÓ YÊU CẦU HỦY
            // =====================================================

            if (
                currentStatus == "Yêu cầu hủy"
                ||
                currentStatus ==
                    "Yêu cầu hủy khi đang giao"
            )
            {
                TempData["ErrorMessage"] =
                    "Đơn đang có yêu cầu hủy. "
                    +
                    "Vui lòng duyệt hoặc từ chối yêu cầu hủy trước.";


                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id = order.MaHd
                    }
                );
            }


            // =====================================================
            // ĐƠN ĐÃ KẾT THÚC
            // =====================================================

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
            // TRẠNG THÁI NHÂN VIÊN ĐƯỢC PHÉP
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
            // LUỒNG XỬ LÝ
            //
            // Chờ xác nhận / Chờ xử lý / Đã thanh toán
            //                  ↓
            //              Đã xác nhận
            //                  ↓
            //              Đang xử lý
            //                  ↓
            //             Đang chuẩn bị
            //                  ↓
            //             Chờ giao hàng
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
                    $"Không thể chuyển đơn từ "
                    +
                    $"\"{currentStatus}\" "
                    +
                    $"sang \"{trangThai}\".";


                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id = order.MaHd
                    }
                );
            }


            // =====================================================
            // CHỜ GIAO HÀNG
            // RESET THÔNG TIN SHIPPER CŨ
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


            await _context
                .SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Đã cập nhật đơn hàng "
                +
                $"#HD{order.MaHd:D5} "
                +
                $"thành \"{trangThai}\".";


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
                await _context
                    .HoaDons
                    .FirstOrDefaultAsync(
                        x =>
                            x.MaHd == id
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
                ??
                string.Empty;


            if (
                currentStatus != "Yêu cầu hủy"
                &&
                currentStatus !=
                    "Yêu cầu hủy khi đang giao"
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
            // VNPAY
            // NHÂN VIÊN DUYỆT HỦY
            // ADMIN SẼ XÁC NHẬN HOÀN TIỀN SAU
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


            // =====================================================
            // COD
            // =====================================================

            else
            {
                order.TrangThai =
                    "Đã hủy";

                order.TrangThaiHoanTien =
                    "Không cần hoàn tiền";

                order.SoTienHoan =
                    0;

                order.NgayHoanTien =
                    null;
            }


            await _context
                .SaveChangesAsync();


            TempData["SuccessMessage"] =
                paidOnline

                    ? $"Đã duyệt hủy đơn "
                      +
                      $"#HD{order.MaHd:D5}. "
                      +
                      "Đơn đang chờ Admin xác nhận hoàn tiền."

                    : $"Đã duyệt hủy đơn "
                      +
                      $"#HD{order.MaHd:D5}.";


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
                await _context
                    .HoaDons
                    .FirstOrDefaultAsync(
                        x =>
                            x.MaHd == id
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
                ??
                string.Empty;


            if (
                currentStatus != "Yêu cầu hủy"
                &&
                currentStatus !=
                    "Yêu cầu hủy khi đang giao"
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
            // HỦY KHI ĐANG GIAO
            // =====================================================

            if (
                currentStatus ==
                    "Yêu cầu hủy khi đang giao"
            )
            {
                order.TrangThai =
                    "Đang giao hàng";
            }


            // =====================================================
            // VNPAY ĐÃ THANH TOÁN
            // =====================================================

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


            // =====================================================
            // TRƯỜNG HỢP KHÁC
            // =====================================================

            else
            {
                order.TrangThai =
                    "Đang xử lý";
            }


            order.NgayXuLyHuy =
                DateTime.Now;


            // =====================================================
            // XÓA THÔNG TIN HOÀN TIỀN CŨ
            // =====================================================

            if (
                order.TrangThaiHoanTien ==
                    "Chờ hoàn tiền"
            )
            {
                order.TrangThaiHoanTien =
                    null;

                order.SoTienHoan =
                    null;

                order.NgayHoanTien =
                    null;
            }


            await _context
                .SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Đã từ chối yêu cầu hủy đơn "
                +
                $"#HD{order.MaHd:D5}.";


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