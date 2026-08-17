using CoffeeShopManagement.Data;
using CoffeeShopManagement.Extensions;
using CoffeeShopManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopManagement.Controllers
{
    public class CartController : Controller
    {
        private readonly CoffeeShopDbContext _context;

        public CartController(
            CoffeeShopDbContext context)
        {
            _context = context;
        }


        private const string CARTKEY =
            "CART";


        // =====================================
        // CART SESSION
        // =====================================

        private List<CartItem> Cart
        {
            get
            {
                return HttpContext.Session
                    .GetObjectFromJson<List<CartItem>>(
                        CARTKEY
                    )
                    ?? new List<CartItem>();
            }

            set
            {
                HttpContext.Session
                    .SetObjectAsJson(
                        CARTKEY,
                        value
                    );
            }
        }


        // =====================================
        // GIỎ HÀNG
        // =====================================

        public IActionResult Index()
        {
            return View(
                Cart
            );
        }


        // =====================================
        // THÊM SẢN PHẨM VÀO GIỎ
        //
        // source = menu
        //      => DonGia
        //
        // source = promotion
        //      => GiaKhuyenMai nếu hợp lệ
        // =====================================

        [HttpPost]
        public IActionResult AddToCart(
            int id,
            string source = "menu",
            string size = "S",
            string mucDa = "Đá vừa",
            string doNgot = "100%",
            string? ghiChu = "",
            int soLuong = 1,
            decimal giaSize = 0,
            decimal giaTopping = 0,
            List<string>? toppings = null)
        {
            toppings ??=
                new List<string>();


            // =================================
            // PRODUCT
            // =================================

            var product =
                _context.SanPhams
                    .FirstOrDefault(
                        x => x.MaSp == id
                    );


            if (product == null)
            {
                return Json(
                    new
                    {
                        success =
                            false,

                        message =
                            "Không tìm thấy sản phẩm."
                    }
                );
            }


            // =================================
            // SOURCE
            // =================================

            var fromPromotion =
                string.Equals(
                    source,
                    "promotion",
                    StringComparison.OrdinalIgnoreCase
                );


            // =================================
            // GIÁ BÁN THỰC TẾ
            //
            // MENU:
            //     DonGia
            //
            // PROMOTION:
            //     GiaKhuyenMai
            // =================================

            decimal sellingPrice =
                product.DonGia;


            if (
                fromPromotion
                &&
                product.DangKhuyenMai
                &&
                product.GiaKhuyenMai.HasValue
            )
            {
                sellingPrice =
                    product.GiaKhuyenMai.Value;
            }


            // =================================
            // QUANTITY VALIDATION
            // =================================

            if (soLuong < 1)
            {
                soLuong =
                    1;
            }


            // =================================
            // CART
            // =================================

            var cart =
                Cart;


            // =================================
            // TÌM ITEM GIỐNG NHAU
            //
            // Quan trọng:
            // phải so cả DonGia để tránh
            // Menu và Promotion gộp chung
            // thành một dòng.
            // =================================

            var item =
                cart.FirstOrDefault(
                    x =>
                        !x.IsCombo
                        &&
                        x.MaSP == id
                        &&
                        x.DonGia == sellingPrice
                        &&
                        x.Size == size
                        &&
                        x.MucDa == mucDa
                        &&
                        x.DoNgot == doNgot
                        &&
                        x.GhiChu == ghiChu
                        &&
                        x.GiaSize == giaSize
                        &&
                        x.GiaTopping == giaTopping
                        &&
                        x.Toppings.SequenceEqual(
                            toppings
                        )
                );


            // =================================
            // ADD NEW
            // =================================

            if (item == null)
            {
                cart.Add(
                    new CartItem
                    {
                        IsCombo =
                            false,


                        MaSP =
                            product.MaSp,


                        TenSP =
                            product.TenSp,


                        // =========================
                        // GIÁ ĐÚNG THEO SOURCE
                        // =========================

                        DonGia =
                            sellingPrice,


                        SoLuong =
                            soLuong,


                        HinhAnh =
                            product.HinhAnh,


                        Size =
                            size,


                        MucDa =
                            mucDa,


                        DoNgot =
                            doNgot,


                        GhiChu =
                            ghiChu,


                        GiaSize =
                            giaSize,


                        GiaTopping =
                            giaTopping,


                        Toppings =
                            toppings
                    }
                );
            }

            // =================================
            // UPDATE QUANTITY
            // =================================

            else
            {
                item.SoLuong +=
                    soLuong;
            }


            // =================================
            // SAVE CART
            // =================================

            Cart =
                cart;


            // =================================
            // RESPONSE
            // =================================

            return Json(
                new
                {
                    success =
                        true,


                    count =
                        cart.Sum(
                            x => x.SoLuong
                        ),


                    source =
                        fromPromotion
                            ? "promotion"
                            : "menu",


                    price =
                        sellingPrice
                }
            );
        }


        // =====================================
        // TĂNG SỐ LƯỢNG
        // =====================================

        public IActionResult Increase(
            Guid rowId)
        {
            var cart =
                Cart;


            var item =
                cart.FirstOrDefault(
                    x => x.RowId == rowId
                );


            if (item != null)
            {
                item.SoLuong++;
            }


            Cart =
                cart;


            return RedirectToAction(
                nameof(Index)
            );
        }


        // =====================================
        // GIẢM SỐ LƯỢNG
        // =====================================

        public IActionResult Decrease(
            Guid rowId)
        {
            var cart =
                Cart;


            var item =
                cart.FirstOrDefault(
                    x => x.RowId == rowId
                );


            if (item != null)
            {
                item.SoLuong--;


                if (item.SoLuong <= 0)
                {
                    cart.Remove(
                        item
                    );
                }
            }


            Cart =
                cart;


            return RedirectToAction(
                nameof(Index)
            );
        }


        // =====================================
        // XÓA 1 SẢN PHẨM
        // =====================================

        public IActionResult Remove(
            Guid rowId)
        {
            var cart =
                Cart;


            var item =
                cart.FirstOrDefault(
                    x => x.RowId == rowId
                );


            if (item != null)
            {
                cart.Remove(
                    item
                );
            }


            Cart =
                cart;


            return RedirectToAction(
                nameof(Index)
            );
        }


        // =====================================
        // XÓA TOÀN BỘ
        // =====================================

        public IActionResult Clear()
        {
            Cart =
                new List<CartItem>();


            return RedirectToAction(
                nameof(Index)
            );
        }


        // =====================================
        // LẤY SỐ LƯỢNG GIỎ HÀNG
        // =====================================

        [HttpGet]
        public IActionResult GetCartCount()
        {
            return Json(
                new
                {
                    count =
                        Cart.Sum(
                            x => x.SoLuong
                        )
                }
            );
        }


        // =====================================
        // THÊM COMBO VÀO GIỎ
        // =====================================

        [HttpPost]
        public IActionResult AddCombo(
            int comboId)
        {
            var combo =
                _context.Combos
                    .FirstOrDefault(
                        x => x.MaCombo == comboId
                    );


            if (combo == null)
            {
                return Json(
                    new
                    {
                        success =
                            false,

                        message =
                            "Không tìm thấy combo."
                    }
                );
            }


            var cart =
                Cart;


            var item =
                cart.FirstOrDefault(
                    x =>
                        x.IsCombo
                        &&
                        x.MaCombo == comboId
                );


            if (item == null)
            {
                cart.Add(
                    new CartItem
                    {
                        IsCombo =
                            true,


                        MaCombo =
                            combo.MaCombo,


                        TenSP =
                            combo.TenCombo,


                        DonGia =
                            combo.GiaBan,


                        SoLuong =
                            1,


                        HinhAnh =
                            combo.HinhAnh
                    }
                );
            }
            else
            {
                item.SoLuong++;
            }


            Cart =
                cart;


            return Json(
                new
                {
                    success =
                        true,


                    count =
                        cart.Sum(
                            x => x.SoLuong
                        )
                }
            );
        }
    }
}