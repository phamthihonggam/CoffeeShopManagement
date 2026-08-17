using CoffeeShopManagement.Data;
using CoffeeShopManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Controllers
{
    public class ReviewController : Controller
    {
        private readonly CoffeeShopDbContext _context;

        public ReviewController(
            CoffeeShopDbContext context)
        {
            _context = context;
        }


        // =========================================================
        // FORM ĐÁNH GIÁ
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Create(
            int maHd,
            int maSp)
        {
            // =============================================
            // KIỂM TRA ĐĂNG NHẬP
            // =============================================

            var maKh =
                HttpContext.Session.GetInt32("MaKH");


            if (maKh == null)
            {
                TempData["Toast"] =
                    "Vui lòng đăng nhập để đánh giá sản phẩm.";

                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }


            // =============================================
            // KIỂM TRA HÓA ĐƠN CÓ THUỘC KHÁCH HÀNG KHÔNG
            // =============================================

            var hoaDon =
                await _context.HoaDons
                    .FirstOrDefaultAsync(
                        x =>
                            x.MaHd == maHd
                            &&
                            x.MaKh == maKh.Value
                    );


            if (hoaDon == null)
            {
                TempData["Toast"] =
                    "Không tìm thấy đơn hàng.";

                return RedirectToAction(
                    "OrderHistory",
                    "Account"
                );
            }


            // =============================================
            // CHỈ ĐƠN HOÀN THÀNH
            // MỚI ĐƯỢC ĐÁNH GIÁ
            // =============================================

            bool canReview =
                hoaDon.TrangThai == "Hoàn thành";


            if (!canReview)
            {
                TempData["Toast"] =
                    "Chỉ đơn hàng đã hoàn thành và khách đã nhận hàng mới được đánh giá.";

                return RedirectToAction(
                    "OrderHistory",
                    "Account"
                );
            }


            // =============================================
            // KIỂM TRA SẢN PHẨM CÓ TRONG HÓA ĐƠN KHÔNG
            // =============================================

            var orderItem =
                await _context.ChiTietHoaDons
                    .FirstOrDefaultAsync(
                        x =>
                            x.MaHd == maHd
                            &&
                            x.MaSp == maSp
                    );


            if (orderItem == null)
            {
                TempData["Toast"] =
                    "Sản phẩm không tồn tại trong đơn hàng này.";

                return RedirectToAction(
                    "OrderHistory",
                    "Account"
                );
            }


            // =============================================
            // KIỂM TRA ĐÃ ĐÁNH GIÁ CHƯA
            // =============================================

            var reviewed =
                await _context.DanhGiaSanPhams
                    .AnyAsync(
                        x =>
                            x.MaKh == maKh.Value
                            &&
                            x.MaSp == maSp
                            &&
                            x.MaHd == maHd
                    );


            if (reviewed)
            {
                TempData["Toast"] =
                    "Bạn đã đánh giá sản phẩm này rồi.";

                return RedirectToAction(
                    "OrderHistory",
                    "Account"
                );
            }


            // =============================================
            // LẤY SẢN PHẨM
            // =============================================

            var product =
                await _context.SanPhams
                    .FirstOrDefaultAsync(
                        x => x.MaSp == maSp
                    );


            if (product == null)
            {
                return NotFound();
            }


            // =============================================
            // TRUYỀN THÔNG TIN SANG VIEW
            // =============================================

            ViewBag.MaHd =
                maHd;

            ViewBag.MaSp =
                maSp;

            ViewBag.TenSp =
                product.TenSp;

            ViewBag.HinhAnh =
                product.HinhAnh;


            return View();
        }


        // =========================================================
        // GỬI ĐÁNH GIÁ
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int maHd,
            int maSp,
            int soSao,
            string? noiDung)
        {
            // =============================================
            // KIỂM TRA ĐĂNG NHẬP
            // =============================================

            var maKh =
                HttpContext.Session.GetInt32("MaKH");


            if (maKh == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }


            // =============================================
            // KIỂM TRA SỐ SAO
            // =============================================

            if (soSao < 1 || soSao > 5)
            {
                TempData["Toast"] =
                    "Vui lòng chọn từ 1 đến 5 sao.";

                return RedirectToAction(
                    nameof(Create),
                    new
                    {
                        maHd,
                        maSp
                    }
                );
            }


            // =============================================
            // GIỚI HẠN NỘI DUNG
            // =============================================

            noiDung =
                noiDung?.Trim();


            if (
                noiDung != null
                &&
                noiDung.Length > 1000
            )
            {
                TempData["Toast"] =
                    "Nội dung đánh giá tối đa 1000 ký tự.";

                return RedirectToAction(
                    nameof(Create),
                    new
                    {
                        maHd,
                        maSp
                    }
                );
            }


            // =============================================
            // KIỂM TRA HÓA ĐƠN
            // =============================================

            var hoaDon =
                await _context.HoaDons
                    .FirstOrDefaultAsync(
                        x =>
                            x.MaHd == maHd
                            &&
                            x.MaKh == maKh.Value
                    );


            if (hoaDon == null)
            {
                TempData["Toast"] =
                    "Không tìm thấy đơn hàng.";

                return RedirectToAction(
                    "OrderHistory",
                    "Account"
                );
            }


            // =============================================
            // KIỂM TRA TRẠNG THÁI
            // CHỈ ĐƠN HOÀN THÀNH MỚI ĐƯỢC ĐÁNH GIÁ
            // =============================================

            bool canReview =
                hoaDon.TrangThai == "Hoàn thành";


            if (!canReview)
            {
                TempData["Toast"] =
                    "Chỉ đơn hàng đã hoàn thành và khách đã nhận hàng mới được đánh giá.";

                return RedirectToAction(
                    "OrderHistory",
                    "Account"
                );
            }


            // =============================================
            // KIỂM TRA SẢN PHẨM THUỘC ĐƠN
            // =============================================

            var purchased =
                await _context.ChiTietHoaDons
                    .AnyAsync(
                        x =>
                            x.MaHd == maHd
                            &&
                            x.MaSp == maSp
                    );


            if (!purchased)
            {
                TempData["Toast"] =
                    "Bạn chưa mua sản phẩm này trong đơn hàng.";

                return RedirectToAction(
                    "OrderHistory",
                    "Account"
                );
            }


            // =============================================
            // KIỂM TRA ĐÁNH GIÁ TRÙNG
            // =============================================

            var alreadyReviewed =
                await _context.DanhGiaSanPhams
                    .AnyAsync(
                        x =>
                            x.MaKh == maKh.Value
                            &&
                            x.MaSp == maSp
                            &&
                            x.MaHd == maHd
                    );


            if (alreadyReviewed)
            {
                TempData["Toast"] =
                    "Bạn đã đánh giá sản phẩm này rồi.";

                return RedirectToAction(
                    "OrderHistory",
                    "Account"
                );
            }


            // =============================================
            // TẠO ĐÁNH GIÁ
            // =============================================

            var review =
                new DanhGiaSanPham
                {
                    MaKh =
                        maKh.Value,

                    MaSp =
                        maSp,

                    MaHd =
                        maHd,

                    SoSao =
                        soSao,

                    NoiDung =
                        noiDung,

                    NgayDanhGia =
                        DateTime.Now
                };


            _context.DanhGiaSanPhams.Add(
                review
            );


            await _context.SaveChangesAsync();


            // =============================================
            // THÔNG BÁO
            // =============================================

            TempData["Toast"] =
                "Cảm ơn bạn đã đánh giá sản phẩm!";


            // =============================================
            // QUAY LẠI LỊCH SỬ ĐƠN HÀNG
            // =============================================

            return RedirectToAction(
                "OrderHistory",
                "Account"
            );
        }

        // =========================================================
        // FORM ĐÁNH GIÁ COMBO
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> CreateCombo(
            int maHd,
            int maCombo)
        {
            var maKh =
                HttpContext.Session.GetInt32("MaKH");


            if (maKh == null)
            {
                TempData["Toast"] =
                    "Vui lòng đăng nhập để đánh giá combo.";

                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }


            var hoaDon =
                await _context.HoaDons
                    .FirstOrDefaultAsync(
                        x =>
                            x.MaHd == maHd
                            &&
                            x.MaKh == maKh.Value
                    );


            if (hoaDon == null)
            {
                TempData["Toast"] =
                    "Không tìm thấy đơn hàng.";

                return RedirectToAction(
                    "OrderHistory",
                    "Account"
                );
            }


            if (
                (hoaDon.TrangThai ?? "").Trim()
                != "Hoàn thành"
            )
            {
                TempData["Toast"] =
                    "Chỉ đơn hàng đã hoàn thành và khách đã nhận hàng mới được đánh giá.";

                return RedirectToAction(
                    "OrderHistory",
                    "Account"
                );
            }


            var orderItem =
                await _context.ChiTietHoaDons
                    .FirstOrDefaultAsync(
                        x =>
                            x.MaHd == maHd
                            &&
                            x.MaCombo == maCombo
                    );


            if (orderItem == null)
            {
                TempData["Toast"] =
                    "Combo không tồn tại trong đơn hàng này.";

                return RedirectToAction(
                    "OrderHistory",
                    "Account"
                );
            }


            var reviewed =
                await _context.DanhGiaCombos
                    .AnyAsync(
                        x =>
                            x.MaKh == maKh.Value
                            &&
                            x.MaCombo == maCombo
                            &&
                            x.MaHd == maHd
                    );


            if (reviewed)
            {
                TempData["Toast"] =
                    "Bạn đã đánh giá combo này rồi.";

                return RedirectToAction(
                    "OrderHistory",
                    "Account"
                );
            }


            var combo =
                await _context.Combos
                    .FirstOrDefaultAsync(
                        x => x.MaCombo == maCombo
                    );


            if (combo == null)
            {
                return NotFound();
            }


            ViewBag.MaHd =
                maHd;

            ViewBag.MaCombo =
                maCombo;

            ViewBag.TenCombo =
                combo.TenCombo;

            ViewBag.HinhAnh =
                combo.HinhAnh;

            ViewBag.GiaBan =
                combo.GiaBan;


            return View(
                "CreateCombo"
            );
        }


        // =========================================================
        // GỬI ĐÁNH GIÁ COMBO
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCombo(
            int maHd,
            int maCombo,
            int soSao,
            string? noiDung)
        {
            var maKh =
                HttpContext.Session.GetInt32("MaKH");


            if (maKh == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }


            if (
                soSao < 1
                ||
                soSao > 5
            )
            {
                TempData["Toast"] =
                    "Vui lòng chọn từ 1 đến 5 sao.";

                return RedirectToAction(
                    nameof(CreateCombo),
                    new
                    {
                        maHd,
                        maCombo
                    }
                );
            }


            noiDung =
                noiDung?.Trim();


            if (
                noiDung != null
                &&
                noiDung.Length > 1000
            )
            {
                TempData["Toast"] =
                    "Nội dung đánh giá tối đa 1000 ký tự.";

                return RedirectToAction(
                    nameof(CreateCombo),
                    new
                    {
                        maHd,
                        maCombo
                    }
                );
            }


            var hoaDon =
                await _context.HoaDons
                    .FirstOrDefaultAsync(
                        x =>
                            x.MaHd == maHd
                            &&
                            x.MaKh == maKh.Value
                    );


            if (hoaDon == null)
            {
                TempData["Toast"] =
                    "Không tìm thấy đơn hàng.";

                return RedirectToAction(
                    "OrderHistory",
                    "Account"
                );
            }


            if (
                (hoaDon.TrangThai ?? "").Trim()
                != "Hoàn thành"
            )
            {
                TempData["Toast"] =
                    "Chỉ đơn hàng đã hoàn thành và khách đã nhận hàng mới được đánh giá.";

                return RedirectToAction(
                    "OrderHistory",
                    "Account"
                );
            }


            var purchased =
                await _context.ChiTietHoaDons
                    .AnyAsync(
                        x =>
                            x.MaHd == maHd
                            &&
                            x.MaCombo == maCombo
                    );


            if (!purchased)
            {
                TempData["Toast"] =
                    "Bạn chưa mua combo này trong đơn hàng.";

                return RedirectToAction(
                    "OrderHistory",
                    "Account"
                );
            }


            var alreadyReviewed =
                await _context.DanhGiaCombos
                    .AnyAsync(
                        x =>
                            x.MaKh == maKh.Value
                            &&
                            x.MaCombo == maCombo
                            &&
                            x.MaHd == maHd
                    );


            if (alreadyReviewed)
            {
                TempData["Toast"] =
                    "Bạn đã đánh giá combo này rồi.";

                return RedirectToAction(
                    "OrderHistory",
                    "Account"
                );
            }


            var review =
                new DanhGiaCombo
                {
                    MaKh =
                        maKh.Value,

                    MaCombo =
                        maCombo,

                    MaHd =
                        maHd,

                    SoSao =
                        soSao,

                    NoiDung =
                        noiDung,

                    NgayDanhGia =
                        DateTime.Now
                };


            _context.DanhGiaCombos.Add(
                review
            );


            await _context.SaveChangesAsync();


            TempData["Toast"] =
                "Cảm ơn bạn đã đánh giá combo!";


            return RedirectToAction(
                "ComboDetails",
                "Promotion",
                new
                {
                    id = maCombo
                }
            );
        }

    }
}