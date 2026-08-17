using CoffeeShopManagement.Data;
using CoffeeShopManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Controllers
{
    public class PromotionController : Controller
    {
        private readonly CoffeeShopDbContext _context;

        public PromotionController(
            CoffeeShopDbContext context)
        {
            _context = context;
        }


        // ==========================================
        // TRANG KHUYẾN MÃI
        // ==========================================

        public IActionResult Index()
        {
            var vm =
                new PromotionViewModel
                {
                    // ==================================
                    // SẢN PHẨM KHUYẾN MÃI
                    // ==================================

                    SanPhams = _context.SanPhams
                        .Where(x =>
                            x.DangKhuyenMai == true &&
                            x.GiaKhuyenMai.HasValue
                        )
                        .OrderBy(x => x.MaSp)
                        .ToList(),


                    // ==================================
                    // COMBO
                    // ==================================

                    Combos = _context.Combos
                        .OrderBy(x => x.MaCombo)
                        .ToList()
                };


            return View(vm);
        }


        // ==========================================
        // TRANG CHI TIẾT COMBO
        // ==========================================

        [HttpGet]
        public IActionResult ComboDetails(
            int id)
        {
            var combo =
                _context.Combos
                    .FirstOrDefault(
                        x => x.MaCombo == id
                    );


            if (combo == null)
            {
                return NotFound();
            }


            var products =
                _context.ChiTietCombos

                    .Where(
                        x => x.MaCombo == id
                    )

                    .Include(
                        x => x.SanPham
                    )

                    .Select(
                        x => new ComboProductItemViewModel
                        {
                            TenSanPham =
                                x.SanPham.TenSp,

                            SoLuong =
                                x.SoLuong
                        }
                    )

                    .ToList();


            var vm =
                new ComboDetailViewModel
                {
                    MaCombo =
                        combo.MaCombo,

                    TenCombo =
                        combo.TenCombo,

                    MoTa =
                        combo.MoTa,

                    HinhAnh =
                        combo.HinhAnh,

                    GiaGoc =
                        combo.GiaGoc,

                    GiaBan =
                        combo.GiaBan,

                    PhanTramGiam =
                        combo.PhanTramGiam,

                    Products =
                        products
                };


            // ==========================================
            // ĐÁNH GIÁ COMBO
            // ==========================================

            var reviews =
                _context.DanhGiaCombos

                    .Include(
                        x => x.MaKhNavigation
                    )

                    .Where(
                        x => x.MaCombo == id
                    )

                    .OrderByDescending(
                        x => x.NgayDanhGia
                    )

                    .ToList();


            ViewBag.ComboReviews =
                reviews;


            ViewBag.ComboReviewCount =
                reviews.Count;


            ViewBag.ComboAverageRating =
                reviews.Any()
                    ? reviews.Average(
                        x => x.SoSao
                    )
                    : 0;


            return View(vm);
        }


        // ==========================================
        // ADD COMBO TO CART
        // ==========================================

        [HttpPost]
        public IActionResult AddComboToCart(
            int id)
        {
            var combo =
                _context.Combos
                    .FirstOrDefault(
                        x => x.MaCombo == id
                    );


            if (combo == null)
            {
                return NotFound();
            }


            var comboDetails =
                _context.ChiTietCombos
                    .Where(
                        x => x.MaCombo == id
                    )
                    .ToList();


            TempData["Success"] =
                $"Đã thêm combo \"{combo.TenCombo}\" vào giỏ hàng.";


            return RedirectToAction(
                nameof(Index)
            );
        }


        // ==========================================
        // GET COMBO DETAIL
        // ==========================================

        [HttpGet]
        public IActionResult GetComboDetail(
            int id)
        {
            var combo =
                _context.Combos
                    .FirstOrDefault(
                        x => x.MaCombo == id
                    );


            if (combo == null)
            {
                return NotFound();
            }


            var products =
                _context.ChiTietCombos

                    .Where(
                        x => x.MaCombo == id
                    )

                    .Select(x => new
                    {
                        Ten =
                            x.SanPham.TenSp,

                        SoLuong =
                            x.SoLuong
                    })

                    .ToList();


            return Json(
                new
                {
                    combo.MaCombo,

                    combo.TenCombo,

                    combo.MoTa,

                    combo.GiaBan,

                    combo.GiaGoc,

                    combo.PhanTramGiam,

                    combo.HinhAnh,

                    Products =
                        products
                }
            );
        }
    }
}