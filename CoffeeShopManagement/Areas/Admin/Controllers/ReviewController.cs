using CoffeeShopManagement.Data;
using CoffeeShopManagement.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class ReviewController : Controller
    {
        private readonly CoffeeShopDbContext _context;


        public ReviewController(
            CoffeeShopDbContext context)
        {
            _context = context;
        }


        // =====================================================
        // DANH SÁCH ĐÁNH GIÁ
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            string? keyword,
            string? type,
            int? star,
            int page = 1,
            int pageSize = 10)
        {
            // =================================================
            // PAGE
            // =================================================

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


            // =================================================
            // TYPE
            // all | product | combo
            // =================================================

            type =
                string.IsNullOrWhiteSpace(type)
                    ? "all"
                    : type.Trim().ToLower();


            var allowedTypes =
                new[]
                {
                    "all",
                    "product",
                    "combo"
                };


            if (!allowedTypes.Contains(type))
            {
                type = "all";
            }


            // =================================================
            // STAR
            // =================================================

            if (
                star.HasValue
                &&
                (
                    star.Value < 1
                    ||
                    star.Value > 5
                )
            )
            {
                star = null;
            }


            // =================================================
            // KEYWORD
            // =================================================

            keyword =
                keyword?.Trim();


            int? numberKeyword =
                null;


            if (
                !string.IsNullOrWhiteSpace(keyword)
                &&
                int.TryParse(
                    keyword,
                    out var parsedNumber
                )
            )
            {
                numberKeyword =
                    parsedNumber;
            }


            // =================================================
            // RESULT LIST
            // =================================================

            var reviews =
                new List<AdminReviewItemViewModel>();


            // =================================================
            // PRODUCT REVIEWS
            // =================================================

            if (
                type == "all"
                ||
                type == "product"
            )
            {
                var productQuery =
                    _context.DanhGiaSanPhams
                        .AsNoTracking()
                        .AsQueryable();


                // STAR

                if (star.HasValue)
                {
                    productQuery =
                        productQuery.Where(
                            x =>
                                x.SoSao ==
                                star.Value
                        );
                }


                // SEARCH

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    var search =
                        keyword;


                    productQuery =
                        productQuery.Where(
                            x =>
                                x.MaKhNavigation.HoTen
                                    .Contains(search)
                                ||
                                (
                                    x.MaKhNavigation.Email != null
                                    &&
                                    x.MaKhNavigation.Email
                                        .Contains(search)
                                )
                                ||
                                x.MaSpNavigation.TenSp
                                    .Contains(search)
                                ||
                                (
                                    x.NoiDung != null
                                    &&
                                    x.NoiDung
                                        .Contains(search)
                                )
                                ||
                                (
                                    numberKeyword.HasValue
                                    &&
                                    (
                                        x.MaHd ==
                                        numberKeyword.Value
                                        ||
                                        x.MaDanhGia ==
                                        numberKeyword.Value
                                    )
                                )
                        );
                }


                var productReviews =
                    await productQuery

                        .Select(
                            x =>
                                new AdminReviewItemViewModel
                                {
                                    ReviewId =
                                        x.MaDanhGia,

                                    ReviewType =
                                        "product",

                                    TypeName =
                                        "Sản phẩm",

                                    ItemId =
                                        x.MaSp,

                                    ItemName =
                                        x.MaSpNavigation.TenSp,

                                    CustomerId =
                                        x.MaKh,

                                    CustomerName =
                                        x.MaKhNavigation.HoTen,

                                    CustomerEmail =
                                        x.MaKhNavigation.Email,

                                    OrderId =
                                        x.MaHd,

                                    Stars =
                                        x.SoSao,

                                    Content =
                                        x.NoiDung,

                                    ReviewDate =
                                        x.NgayDanhGia
                                }
                        )

                        .ToListAsync();


                reviews.AddRange(
                    productReviews
                );
            }


            // =================================================
            // COMBO REVIEWS
            // =================================================

            if (
                type == "all"
                ||
                type == "combo"
            )
            {
                var comboQuery =
                    _context.DanhGiaCombos
                        .AsNoTracking()
                        .AsQueryable();


                // STAR

                if (star.HasValue)
                {
                    comboQuery =
                        comboQuery.Where(
                            x =>
                                x.SoSao ==
                                star.Value
                        );
                }


                // SEARCH

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    var search =
                        keyword;


                    comboQuery =
                        comboQuery.Where(
                            x =>
                                x.MaKhNavigation.HoTen
                                    .Contains(search)
                                ||
                                (
                                    x.MaKhNavigation.Email != null
                                    &&
                                    x.MaKhNavigation.Email
                                        .Contains(search)
                                )
                                ||
                                x.MaComboNavigation.TenCombo
                                    .Contains(search)
                                ||
                                (
                                    x.NoiDung != null
                                    &&
                                    x.NoiDung
                                        .Contains(search)
                                )
                                ||
                                (
                                    numberKeyword.HasValue
                                    &&
                                    (
                                        x.MaHd ==
                                        numberKeyword.Value
                                        ||
                                        x.MaDanhGia ==
                                        numberKeyword.Value
                                    )
                                )
                        );
                }


                var comboReviews =
                    await comboQuery

                        .Select(
                            x =>
                                new AdminReviewItemViewModel
                                {
                                    ReviewId =
                                        x.MaDanhGia,

                                    ReviewType =
                                        "combo",

                                    TypeName =
                                        "Combo",

                                    ItemId =
                                        x.MaCombo,

                                    ItemName =
                                        x.MaComboNavigation.TenCombo,

                                    CustomerId =
                                        x.MaKh,

                                    CustomerName =
                                        x.MaKhNavigation.HoTen,

                                    CustomerEmail =
                                        x.MaKhNavigation.Email,

                                    OrderId =
                                        x.MaHd,

                                    Stars =
                                        x.SoSao,

                                    Content =
                                        x.NoiDung,

                                    ReviewDate =
                                        x.NgayDanhGia
                                }
                        )

                        .ToListAsync();


                reviews.AddRange(
                    comboReviews
                );
            }


            // =================================================
            // SORT
            // =================================================

            reviews =
                reviews

                    .OrderByDescending(
                        x =>
                            x.ReviewDate
                    )

                    .ThenByDescending(
                        x =>
                            x.ReviewId
                    )

                    .ToList();


            // =================================================
            // PAGINATION
            // =================================================

            var totalItems =
                reviews.Count;


            var totalPages =
                totalItems == 0
                    ? 1
                    : (int)Math.Ceiling(
                        totalItems /
                        (double)pageSize
                    );


            if (page > totalPages)
            {
                page =
                    totalPages;
            }


            var pagedReviews =
                reviews

                    .Skip(
                        (page - 1)
                        *
                        pageSize
                    )

                    .Take(
                        pageSize
                    )

                    .ToList();


            // =================================================
            // QUICK STATISTICS
            // =================================================

            var totalProductReviews =
                await _context
                    .DanhGiaSanPhams
                    .CountAsync();


            var totalComboReviews =
                await _context
                    .DanhGiaCombos
                    .CountAsync();


            var totalReviews =
                totalProductReviews
                +
                totalComboReviews;


            // =================================================
            // STAR SUM
            // =================================================

            var productStarSum =
                await _context
                    .DanhGiaSanPhams

                    .SumAsync(
                        x =>
                            (int?)x.SoSao
                    )
                ?? 0;


            var comboStarSum =
                await _context
                    .DanhGiaCombos

                    .SumAsync(
                        x =>
                            (int?)x.SoSao
                    )
                ?? 0;


            var averageRating =
                totalReviews > 0

                    ? (
                        productStarSum
                        +
                        comboStarSum
                    )
                    /
                    (double)totalReviews

                    : 0;


            // =================================================
            // 5 STAR
            // =================================================

            var fiveStarReviews =
                await _context
                    .DanhGiaSanPhams

                    .CountAsync(
                        x =>
                            x.SoSao == 5
                    )
                +
                await _context
                    .DanhGiaCombos

                    .CountAsync(
                        x =>
                            x.SoSao == 5
                    );


            // =================================================
            // LOW RATING
            // 1 - 2 STAR
            // =================================================

            var lowRatingReviews =
                await _context
                    .DanhGiaSanPhams

                    .CountAsync(
                        x =>
                            x.SoSao <= 2
                    )
                +
                await _context
                    .DanhGiaCombos

                    .CountAsync(
                        x =>
                            x.SoSao <= 2
                    );


            // =================================================
            // MODEL
            // =================================================

            var model =
                new AdminReviewIndexViewModel
                {
                    Reviews =
                        pagedReviews,

                    Keyword =
                        keyword,

                    Type =
                        type,

                    Star =
                        star,

                    CurrentPage =
                        page,

                    PageSize =
                        pageSize,

                    TotalItems =
                        totalItems,

                    TotalPages =
                        totalPages,

                    TotalReviews =
                        totalReviews,

                    ProductReviews =
                        totalProductReviews,

                    ComboReviews =
                        totalComboReviews,

                    AverageRating =
                        averageRating,

                    FiveStarReviews =
                        fiveStarReviews,

                    LowRatingReviews =
                        lowRatingReviews
                };


            return View(
                model
            );
        }


        // =====================================================
        // CHI TIẾT ĐÁNH GIÁ
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Details(
            string type,
            int id)
        {
            type =
                type?.Trim().ToLower()
                ??
                "";


            AdminReviewDetailsViewModel? model =
                null;


            // =================================================
            // PRODUCT
            // =================================================

            if (type == "product")
            {
                model =
                    await _context
                        .DanhGiaSanPhams

                        .AsNoTracking()

                        .Where(
                            x =>
                                x.MaDanhGia == id
                        )

                        .Select(
                            x =>
                                new AdminReviewDetailsViewModel
                                {
                                    ReviewId =
                                        x.MaDanhGia,

                                    ReviewType =
                                        "product",

                                    TypeName =
                                        "Sản phẩm",

                                    ItemId =
                                        x.MaSp,

                                    ItemName =
                                        x.MaSpNavigation.TenSp,

                                    CustomerId =
                                        x.MaKh,

                                    CustomerName =
                                        x.MaKhNavigation.HoTen,

                                    CustomerEmail =
                                        x.MaKhNavigation.Email,

                                    CustomerPhone =
                                        x.MaKhNavigation.DienThoai,

                                    OrderId =
                                        x.MaHd,

                                    Stars =
                                        x.SoSao,

                                    Content =
                                        x.NoiDung,

                                    ReviewDate =
                                        x.NgayDanhGia
                                }
                        )

                        .FirstOrDefaultAsync();
            }


            // =================================================
            // COMBO
            // =================================================

            else if (type == "combo")
            {
                model =
                    await _context
                        .DanhGiaCombos

                        .AsNoTracking()

                        .Where(
                            x =>
                                x.MaDanhGia == id
                        )

                        .Select(
                            x =>
                                new AdminReviewDetailsViewModel
                                {
                                    ReviewId =
                                        x.MaDanhGia,

                                    ReviewType =
                                        "combo",

                                    TypeName =
                                        "Combo",

                                    ItemId =
                                        x.MaCombo,

                                    ItemName =
                                        x.MaComboNavigation.TenCombo,

                                    CustomerId =
                                        x.MaKh,

                                    CustomerName =
                                        x.MaKhNavigation.HoTen,

                                    CustomerEmail =
                                        x.MaKhNavigation.Email,

                                    CustomerPhone =
                                        x.MaKhNavigation.DienThoai,

                                    OrderId =
                                        x.MaHd,

                                    Stars =
                                        x.SoSao,

                                    Content =
                                        x.NoiDung,

                                    ReviewDate =
                                        x.NgayDanhGia
                                }
                        )

                        .FirstOrDefaultAsync();
            }


            if (model == null)
            {
                return NotFound();
            }


            return View(
                model
            );
        }
    }


    // =========================================================
    // INDEX VIEW MODEL
    // =========================================================

    public sealed class AdminReviewIndexViewModel
    {
        public List<AdminReviewItemViewModel>
            Reviews
        { get; set; } = new();


        public string? Keyword
        { get; set; }


        public string Type
        { get; set; } = "all";


        public int? Star
        { get; set; }


        public int CurrentPage
        { get; set; }


        public int PageSize
        { get; set; }


        public int TotalItems
        { get; set; }


        public int TotalPages
        { get; set; }


        public int TotalReviews
        { get; set; }


        public int ProductReviews
        { get; set; }


        public int ComboReviews
        { get; set; }


        public double AverageRating
        { get; set; }


        public int FiveStarReviews
        { get; set; }


        public int LowRatingReviews
        { get; set; }
    }


    // =========================================================
    // LIST ITEM
    // =========================================================

    public sealed class AdminReviewItemViewModel
    {
        public int ReviewId
        { get; set; }


        public string ReviewType
        { get; set; } = "";


        public string TypeName
        { get; set; } = "";


        public int ItemId
        { get; set; }


        public string ItemName
        { get; set; } = "";


        public int CustomerId
        { get; set; }


        public string CustomerName
        { get; set; } = "";


        public string? CustomerEmail
        { get; set; }


        public int OrderId
        { get; set; }


        public int Stars
        { get; set; }


        public string? Content
        { get; set; }


        public DateTime ReviewDate
        { get; set; }
    }


    // =========================================================
    // DETAILS
    // =========================================================

    public sealed class AdminReviewDetailsViewModel
    {
        public int ReviewId
        { get; set; }


        public string ReviewType
        { get; set; } = "";


        public string TypeName
        { get; set; } = "";


        public int ItemId
        { get; set; }


        public string ItemName
        { get; set; } = "";


        public int CustomerId
        { get; set; }


        public string CustomerName
        { get; set; } = "";


        public string? CustomerEmail
        { get; set; }


        public string? CustomerPhone
        { get; set; }


        public int OrderId
        { get; set; }


        public int Stars
        { get; set; }


        public string? Content
        { get; set; }


        public DateTime ReviewDate
        { get; set; }
    }
}