using CoffeeShopManagement.Data;
using CoffeeShopManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Controllers
{
    public class MenuController : Controller
    {
        private readonly CoffeeShopDbContext _context;


        public MenuController(
            CoffeeShopDbContext context)
        {
            _context = context;
        }


        // =====================================================
        // MENU LANDING
        // =====================================================

        public IActionResult Index()
        {
            var categories =
                _context.LoaiSanPhams
                    .OrderBy(
                        x => x.MaLoai
                    )
                    .ToList();


            var model =
                categories
                    .Select(
                        x => new MenuLandingVM
                        {
                            Category = x,


                            Image =
                                x.MaLoai switch
                                {
                                    1 => "/images/menu/coffee.png",

                                    2 => "/images/menu/tea.png",

                                    3 => "/images/menu/matcha.png",

                                    4 => "/images/menu/soda.png",

                                    5 => "/images/menu/cake.png",

                                    6 => "/images/menu/juice.png",

                                    7 => "/images/menu/yogurt.png",

                                    8 => "/images/menu/blended.png",

                                    _ => "/images/menu/default.png"
                                },


                            Background =
                                x.MaLoai % 2 == 0
                                    ? "#F5EBDD"
                                    : "#F9F3EB",


                            Description =
                                x.TenLoai switch
                                {
                                    "Cà phê" =>
                                        "Đậm đà từ những hạt cà phê tuyển chọn, đánh thức mọi giác quan.",

                                    "Trà" =>
                                        "Thanh mát và tinh tế với hương vị thiên nhiên.",

                                    "Matcha" =>
                                        "Matcha Nhật Bản thơm béo, chuẩn vị truyền thống.",

                                    "Soda" =>
                                        "Sảng khoái cùng những ly soda đầy màu sắc.",

                                    "Bánh ngọt" =>
                                        "Bánh tươi mỗi ngày, kết hợp hoàn hảo cùng đồ uống.",

                                    "Nước ép" =>
                                        "Trái cây tươi nguyên chất, giàu vitamin.",

                                    "Sữa chua" =>
                                        "Chua ngọt hài hòa, tốt cho sức khỏe.",

                                    "Đá xay" =>
                                        "Mát lạnh, béo thơm, tiếp thêm năng lượng.",

                                    _ => ""
                                }
                        }
                    )
                    .ToList();


            return View(model);
        }


        // =====================================================
        // PRODUCTS PAGE
        // =====================================================

        public IActionResult Products(
            int category = 0,
            string keyword = "")
        {
            const int pageSize = 12;


            var query =
                _context.SanPhams
                    .Include(
                        x => x.MaLoaiNavigation
                    )
                    .AsQueryable();


            // =================================================
            // CATEGORY
            // =================================================

            if (category > 0)
            {
                query =
                    query.Where(
                        x => x.MaLoai == category
                    );
            }


            // =================================================
            // SEARCH KEYWORD
            // =================================================

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword =
                    keyword.Trim();


                query =
                    query.Where(
                        x =>
                            x.TenSp.Contains(keyword)
                            ||
                            (
                                x.MoTa != null
                                &&
                                x.MoTa.Contains(keyword)
                            )
                    );
            }


            // =================================================
            // LOAD FIRST PAGE
            // =================================================

            var products =
                query
                    .OrderBy(
                        x => x.MaSp
                    )
                    .Take(
                        pageSize
                    )
                    .ToList();


            // =================================================
            // SEND FILTER TO VIEW
            // =================================================

            ViewBag.Keyword =
                keyword;

            ViewBag.Category =
                category;


            return View(products);
        }


        // =====================================================
        // AJAX FILTER PRODUCTS
        // =====================================================

        [HttpGet]
        public IActionResult GetProducts(
            int page = 1,
            int category = 0,
            string keyword = "",
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string sort = "",
            int pageSize = 12)
        {
            if (page < 1)
            {
                page = 1;
            }


            if (pageSize <= 0)
            {
                pageSize = 12;
            }


            var query =
                _context.SanPhams
                    .Include(
                        x => x.MaLoaiNavigation
                    )
                    .AsQueryable();


            // =================================================
            // CATEGORY
            // =================================================

            if (category > 0)
            {
                query =
                    query.Where(
                        x => x.MaLoai == category
                    );
            }


            // =================================================
            // KEYWORD
            // =================================================

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword =
                    keyword.Trim();


                query =
                    query.Where(
                        x =>
                            x.TenSp.Contains(
                                keyword
                            )
                            ||
                            (
                                x.MoTa != null
                                &&
                                x.MoTa.Contains(
                                    keyword
                                )
                            )
                    );
            }


            // =================================================
            // MIN PRICE
            // =================================================

            if (minPrice.HasValue)
            {
                query =
                    query.Where(
                        x =>
                            x.DonGia >= minPrice.Value
                    );
            }


            // =================================================
            // MAX PRICE
            // =================================================

            if (maxPrice.HasValue)
            {
                query =
                    query.Where(
                        x =>
                            x.DonGia <= maxPrice.Value
                    );
            }


            // =================================================
            // SORT
            // =================================================

            query =
                sort switch
                {
                    "price-asc" =>
                        query.OrderBy(
                            x => x.DonGia
                        ),

                    "price-desc" =>
                        query.OrderByDescending(
                            x => x.DonGia
                        ),

                    "name-asc" =>
                        query.OrderBy(
                            x => x.TenSp
                        ),

                    "name-desc" =>
                        query.OrderByDescending(
                            x => x.TenSp
                        ),

                    _ =>
                        query.OrderBy(
                            x => x.MaSp
                        )
                };


            // =================================================
            // PAGINATION
            // =================================================

            var products =
                query
                    .Skip(
                        (page - 1) * pageSize
                    )
                    .Take(
                        pageSize
                    )
                    .ToList();


            return PartialView(
                "_MenuList",
                products
            );
        }


        // =====================================================
        // TOTAL PAGES
        // =====================================================

        [HttpGet]
        public IActionResult GetTotalPages(
            int category = 0,
            string keyword = "",
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int pageSize = 12)
        {
            if (pageSize <= 0)
            {
                pageSize = 12;
            }


            var query =
                _context.SanPhams
                    .AsQueryable();


            // =================================================
            // CATEGORY
            // =================================================

            if (category > 0)
            {
                query =
                    query.Where(
                        x => x.MaLoai == category
                    );
            }


            // =================================================
            // SEARCH
            // =================================================

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword =
                    keyword.Trim();


                query =
                    query.Where(
                        x =>
                            x.TenSp.Contains(
                                keyword
                            )
                            ||
                            (
                                x.MoTa != null
                                &&
                                x.MoTa.Contains(
                                    keyword
                                )
                            )
                    );
            }


            // =================================================
            // MIN PRICE
            // =================================================

            if (minPrice.HasValue)
            {
                query =
                    query.Where(
                        x =>
                            x.DonGia >= minPrice.Value
                    );
            }


            // =================================================
            // MAX PRICE
            // =================================================

            if (maxPrice.HasValue)
            {
                query =
                    query.Where(
                        x =>
                            x.DonGia <= maxPrice.Value
                    );
            }


            var total =
                query.Count();


            var totalPages =
                (int)Math.Ceiling(
                    (double)total
                    /
                    pageSize
                );


            return Json(
                totalPages
            );
        }


        // =====================================================
        // CUSTOMIZE MODAL
        // =====================================================

        public IActionResult GetCustomizeModal(
            int id)
        {
            var product =
                _context.SanPhams
                    .Include(
                        x => x.Toppings
                    )
                    .FirstOrDefault(
                        x => x.MaSp == id
                    );


            if (product == null)
            {
                return NotFound();
            }


            var vm =
                new ProductCustomizeViewModel
                {
                    Product =
                        product,


                    Sizes =
                        _context.ProductSizes
                            .Where(
                                x => x.MaSp == id
                            )
                            .OrderBy(
                                x => x.ThuTu
                            )
                            .ToList(),


                    IceLevels =
                        _context.IceLevels
                            .OrderBy(
                                x => x.Id
                            )
                            .ToList(),


                    SugarLevels =
                        _context.SugarLevels
                            .OrderBy(
                                x => x.Id
                            )
                            .ToList(),


                    Toppings =
                        product.Toppings
                            .ToList()
                };


            return PartialView(
                "~/Views/Shared/_ProductModal.cshtml",
                vm
            );
        }


        // =====================================================
        // CUSTOMIZE OPTIONS JSON
        // =====================================================

        [HttpGet]
        public async Task<IActionResult>
            GetCustomizeOptions(
                int id)
        {
            var product =
                await _context.SanPhams
                    .Include(
                        x => x.Toppings
                    )
                    .FirstOrDefaultAsync(
                        x => x.MaSp == id
                    );


            if (product == null)
            {
                return Json(
                    new
                    {
                        success = false
                    }
                );
            }


            var sizes =
                await _context.ProductSizes
                    .Where(
                        x => x.MaSp == id
                    )
                    .OrderBy(
                        x => x.ThuTu
                    )
                    .Select(
                        x => new
                        {
                            name = x.TenSize,
                            price = x.GiaThem
                        }
                    )
                    .ToListAsync();


            var sugarLevels =
                await _context.SugarLevels
                    .OrderBy(
                        x => x.Id
                    )
                    .Select(
                        x => new
                        {
                            name = x.TenDuong
                        }
                    )
                    .ToListAsync();


            var iceLevels =
                await _context.IceLevels
                    .OrderBy(
                        x => x.Id
                    )
                    .Select(
                        x => new
                        {
                            name = x.TenDa
                        }
                    )
                    .ToListAsync();


            var toppings =
                product.Toppings
                    .Where(
                        x => x.IsActive != false
                    )
                    .OrderBy(
                        x => x.Id
                    )
                    .Select(
                        x => new
                        {
                            name = x.TenTopping,
                            price = x.GiaThem
                        }
                    )
                    .ToList();


            return Json(
                new
                {
                    success = true,


                    productType =
                        product.MaLoai,


                    sizes,


                    sugarLevels,


                    iceLevels,


                    toppings
                }
            );
        }


        // =====================================================
        // DETAILS
        //
        // MENU:
        //     luôn dùng DonGia
        //
        // PROMOTION:
        //     source=promotion
        //     dùng GiaKhuyenMai nếu sản phẩm đang khuyến mãi
        // =====================================================

        public async Task<IActionResult> Details(
            int id,
            string? source = null)
        {
            var product =
                await _context.SanPhams
                    .Include(
                        x => x.MaLoaiNavigation
                    )
                    .Include(
                        x => x.Toppings
                    )
                    .FirstOrDefaultAsync(
                        x => x.MaSp == id
                    );


            if (product == null)
            {
                return NotFound();
            }


            // =================================================
            // NGUỒN TRUY CẬP
            // =================================================

            var fromPromotion =
                string.Equals(
                    source,
                    "promotion",
                    StringComparison.OrdinalIgnoreCase
                );


            // =================================================
            // GIÁ HIỂN THỊ
            //
            // Menu      => DonGia
            // Promotion => GiaKhuyenMai nếu hợp lệ
            // =================================================

            var showPromotionPrice =
                fromPromotion
                &&
                product.DangKhuyenMai
                &&
                product.GiaKhuyenMai.HasValue;


            var sellingPrice =
                showPromotionPrice
                    ? product.GiaKhuyenMai!.Value
                    : product.DonGia;


            // =================================================
            // VIEW MODEL
            // =================================================

            var vm =
                new ProductCustomizeViewModel
                {
                    Product =
                        product,


                    Sizes =
                        await _context.ProductSizes
                            .Where(
                                x => x.MaSp == id
                            )
                            .OrderBy(
                                x => x.ThuTu
                            )
                            .ToListAsync(),


                    IceLevels =
                        await _context.IceLevels
                            .OrderBy(
                                x => x.Id
                            )
                            .ToListAsync(),


                    SugarLevels =
                        await _context.SugarLevels
                            .OrderBy(
                                x => x.Id
                            )
                            .ToListAsync(),


                    Toppings =
                        product.Toppings
                            .Where(
                                x => x.IsActive != false
                            )
                            .ToList(),


                    Quantity = 1,


                    Note = ""
                };


            // =================================================
            // DATA CHO DETAILS VIEW
            // =================================================

            ViewBag.SellingPrice =
                sellingPrice;


            ViewBag.OriginalPrice =
                product.DonGia;


            ViewBag.IsPromotion =
                showPromotionPrice;


            ViewBag.Source =
                fromPromotion
                    ? "promotion"
                    : "menu";


            ViewBag.DiscountPercent =
                showPromotionPrice
                    ? product.PhanTramGiam
                    : null;


            // =================================================
            // ĐÁNH GIÁ SẢN PHẨM
            // =================================================

            var reviews =
                await _context.DanhGiaSanPhams
                    .Include(
                        x => x.MaKhNavigation
                    )
                    .Where(
                        x => x.MaSp == id
                    )
                    .OrderByDescending(
                        x => x.NgayDanhGia
                    )
                    .ToListAsync();


            ViewBag.Reviews =
                reviews;


            ViewBag.ReviewCount =
                reviews.Count;


            ViewBag.AverageRating =
                reviews.Any()
                    ? reviews.Average(
                        x => x.SoSao
                    )
                    : 0;


            return View(
                vm
            );
        }
    }
}