using CoffeeShopManagement.Data;
using CoffeeShopManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CoffeeShopManagement.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "NhanVien")]
    public class ReviewController : Controller
    {
        private readonly CoffeeShopDbContext _context;


        public ReviewController(
            CoffeeShopDbContext context)
        {
            _context = context;
        }


        // =========================================================
        // LẤY MÃ TÀI KHOẢN NHÂN VIÊN HIỆN TẠI
        // =========================================================

        private int? GetCurrentAccountId()
        {
            string? claimId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );


            if (
                !string.IsNullOrWhiteSpace(claimId)
                &&
                int.TryParse(
                    claimId,
                    out int id
                )
            )
            {
                return id;
            }


            int? sessionId =
                HttpContext.Session.GetInt32(
                    "MaTaiKhoan"
                );


            if (sessionId.HasValue)
            {
                return sessionId;
            }


            return HttpContext.Session.GetInt32(
                "AdminAccountId"
            );
        }


        // =========================================================
        // DANH SÁCH ĐÁNH GIÁ
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            string? keyword,
            string? type,
            int? star,
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


            if (
                pageSize != 10
                &&
                pageSize != 20
                &&
                pageSize != 50
            )
            {
                pageSize = 10;
            }


            // =====================================================
            // TYPE
            // =====================================================

            type =
                string.IsNullOrWhiteSpace(type)
                    ? "all"
                    : type.Trim().ToLower();


            if (
                type != "all"
                &&
                type != "product"
                &&
                type != "combo"
            )
            {
                type = "all";
            }


            // =====================================================
            // STAR
            // =====================================================

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


            // =====================================================
            // SEARCH
            // =====================================================

            keyword =
                keyword?.Trim();


            int? numberKeyword =
                null;


            if (
                !string.IsNullOrWhiteSpace(keyword)
                &&
                int.TryParse(
                    keyword,
                    out int parsedNumber
                )
            )
            {
                numberKeyword =
                    parsedNumber;
            }


            // =====================================================
            // DANH SÁCH
            // =====================================================

            var reviews =
                new List<StaffReviewItemViewModel>();


            // =====================================================
            // PRODUCT
            // =====================================================

            if (
                type == "all"
                ||
                type == "product"
            )
            {
                IQueryable<DanhGiaSanPham> query =
                    _context
                        .DanhGiaSanPhams
                        .AsNoTracking();


                // =================================================
                // STAR
                // =================================================

                if (star.HasValue)
                {
                    query =
                        query.Where(
                            x =>
                                x.SoSao
                                ==
                                star.Value
                        );
                }


                // =================================================
                // SEARCH
                // =================================================

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    string search =
                        keyword;


                    query =
                        query.Where(
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
                                    x.NoiDung.Contains(search)
                                )

                                ||

                                (
                                    x.PhanHoi != null
                                    &&
                                    x.PhanHoi.Contains(search)
                                )

                                ||

                                (
                                    numberKeyword.HasValue
                                    &&
                                    (
                                        x.MaHd
                                            ==
                                            numberKeyword.Value

                                        ||

                                        x.MaDanhGia
                                            ==
                                            numberKeyword.Value
                                    )
                                )
                        );
                }


                var productReviews =
                    await query

                        .Select(
                            x =>
                                new StaffReviewItemViewModel
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
                                        x.NgayDanhGia,

                                    HasReply =
                                        x.PhanHoi != null
                                        &&
                                        x.PhanHoi != ""
                                }
                        )

                        .ToListAsync();


                reviews.AddRange(
                    productReviews
                );
            }


            // =====================================================
            // COMBO
            // =====================================================

            if (
                type == "all"
                ||
                type == "combo"
            )
            {
                IQueryable<DanhGiaCombo> query =
                    _context
                        .DanhGiaCombos
                        .AsNoTracking();


                // =================================================
                // STAR
                // =================================================

                if (star.HasValue)
                {
                    query =
                        query.Where(
                            x =>
                                x.SoSao
                                ==
                                star.Value
                        );
                }


                // =================================================
                // SEARCH
                // =================================================

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    string search =
                        keyword;


                    query =
                        query.Where(
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
                                    x.NoiDung.Contains(search)
                                )

                                ||

                                (
                                    x.PhanHoi != null
                                    &&
                                    x.PhanHoi.Contains(search)
                                )

                                ||

                                (
                                    numberKeyword.HasValue
                                    &&
                                    (
                                        x.MaHd
                                            ==
                                            numberKeyword.Value

                                        ||

                                        x.MaDanhGia
                                            ==
                                            numberKeyword.Value
                                    )
                                )
                        );
                }


                var comboReviews =
                    await query

                        .Select(
                            x =>
                                new StaffReviewItemViewModel
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
                                        x.NgayDanhGia,

                                    HasReply =
                                        x.PhanHoi != null
                                        &&
                                        x.PhanHoi != ""
                                }
                        )

                        .ToListAsync();


                reviews.AddRange(
                    comboReviews
                );
            }


            // =====================================================
            // SORT
            // =====================================================

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


            // =====================================================
            // PAGINATION
            // =====================================================

            int totalItems =
                reviews.Count;


            int totalPages =
                Math.Max(
                    1,
                    (int)Math.Ceiling(
                        totalItems
                        /
                        (double)pageSize
                    )
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


            // =====================================================
            // THỐNG KÊ
            // =====================================================

            int productCount =
                await _context
                    .DanhGiaSanPhams
                    .CountAsync();


            int comboCount =
                await _context
                    .DanhGiaCombos
                    .CountAsync();


            int totalReviews =
                productCount
                +
                comboCount;


            int productStarSum =
                await _context
                    .DanhGiaSanPhams
                    .SumAsync(
                        x =>
                            (int?)x.SoSao
                    )
                ?? 0;


            int comboStarSum =
                await _context
                    .DanhGiaCombos
                    .SumAsync(
                        x =>
                            (int?)x.SoSao
                    )
                ?? 0;


            double averageRating =
                totalReviews > 0

                    ? (
                        productStarSum
                        +
                        comboStarSum
                    )
                    /
                    (double)totalReviews

                    : 0;


            int fiveStarReviews =
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


            int lowRatingReviews =
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


            // =====================================================
            // MODEL
            // =====================================================

            var model =
                new StaffReviewIndexViewModel
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
                        productCount,

                    ComboReviews =
                        comboCount,

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


        // =========================================================
        // CHI TIẾT ĐÁNH GIÁ
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Details(
            string type,
            int id)
        {
            type =
                type?.Trim().ToLower()
                ??
                "";


            StaffReviewDetailsViewModel? model =
                null;


            // =====================================================
            // PRODUCT
            // =====================================================

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
                                new StaffReviewDetailsViewModel
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
                                        x.NgayDanhGia,

                                    Reply =
                                        x.PhanHoi,

                                    ReplyDate =
                                        x.NgayPhanHoi,

                                    ReplierName =
                                        x.MaTaiKhoanPhanHoiNavigation
                                            != null

                                            ? x.MaTaiKhoanPhanHoiNavigation
                                                .HoTen

                                            : null,

                                    ReplierRole =
                                        x.MaTaiKhoanPhanHoiNavigation
                                            != null

                                            ? x.MaTaiKhoanPhanHoiNavigation
                                                .MaVaiTroNavigation
                                                .TenVaiTro

                                            : null
                                }
                        )

                        .FirstOrDefaultAsync();
            }


            // =====================================================
            // COMBO
            // =====================================================

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
                                new StaffReviewDetailsViewModel
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
                                        x.NgayDanhGia,

                                    Reply =
                                        x.PhanHoi,

                                    ReplyDate =
                                        x.NgayPhanHoi,

                                    ReplierName =
                                        x.MaTaiKhoanPhanHoiNavigation
                                            != null

                                            ? x.MaTaiKhoanPhanHoiNavigation
                                                .HoTen

                                            : null,

                                    ReplierRole =
                                        x.MaTaiKhoanPhanHoiNavigation
                                            != null

                                            ? x.MaTaiKhoanPhanHoiNavigation
                                                .MaVaiTroNavigation
                                                .TenVaiTro

                                            : null
                                }
                        )

                        .FirstOrDefaultAsync();
            }


            if (model == null)
            {
                TempData["ReviewError"] =
                    "Không tìm thấy đánh giá.";


                return RedirectToAction(
                    nameof(Index)
                );
            }


            return View(
                model
            );
        }


        // =========================================================
        // NHÂN VIÊN PHẢN HỒI
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(
            string type,
            int id,
            string? reply)
        {
            type =
                type?.Trim().ToLower()
                ??
                "";


            reply =
                reply?.Trim();


            // =====================================================
            // VALIDATION
            // =====================================================

            if (string.IsNullOrWhiteSpace(reply))
            {
                TempData["ReviewError"] =
                    "Vui lòng nhập nội dung phản hồi.";


                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        type,
                        id
                    }
                );
            }


            if (reply.Length > 1000)
            {
                TempData["ReviewError"] =
                    "Nội dung phản hồi tối đa 1000 ký tự.";


                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        type,
                        id
                    }
                );
            }


            // =====================================================
            // TÀI KHOẢN NHÂN VIÊN
            // =====================================================

            int? accountId =
                GetCurrentAccountId();


            if (!accountId.HasValue)
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


            var account =
                await _context
                    .TaiKhoans
                    .Include(
                        x =>
                            x.MaVaiTroNavigation
                    )
                    .FirstOrDefaultAsync(
                        x =>
                            x.MaTaiKhoan
                            ==
                            accountId.Value

                            &&
                            x.IsActive
                    );


            if (account == null)
            {
                return Forbid();
            }


            // =====================================================
            // KIỂM TRA ROLE
            // =====================================================

            string role =
                account
                    .MaVaiTroNavigation
                    .TenVaiTro
                    .Trim();


            bool isStaff =
                role.Equals(
                    "NhanVien",
                    StringComparison.OrdinalIgnoreCase
                )

                ||

                role.Equals(
                    "Nhân viên",
                    StringComparison.OrdinalIgnoreCase
                )

                ||

                role.Equals(
                    "Nhan Vien",
                    StringComparison.OrdinalIgnoreCase
                )

                ||

                role.Equals(
                    "Staff",
                    StringComparison.OrdinalIgnoreCase
                );


            if (!isStaff)
            {
                return Forbid();
            }


            // =====================================================
            // PRODUCT
            // =====================================================

            if (type == "product")
            {
                var review =
                    await _context
                        .DanhGiaSanPhams
                        .FirstOrDefaultAsync(
                            x =>
                                x.MaDanhGia == id
                        );


                if (review == null)
                {
                    TempData["ReviewError"] =
                        "Không tìm thấy đánh giá sản phẩm.";


                    return RedirectToAction(
                        nameof(Index)
                    );
                }


                review.PhanHoi =
                    reply;


                review.NgayPhanHoi =
                    DateTime.Now;


                review.MaTaiKhoanPhanHoi =
                    account.MaTaiKhoan;
            }


            // =====================================================
            // COMBO
            // =====================================================

            else if (type == "combo")
            {
                var review =
                    await _context
                        .DanhGiaCombos
                        .FirstOrDefaultAsync(
                            x =>
                                x.MaDanhGia == id
                        );


                if (review == null)
                {
                    TempData["ReviewError"] =
                        "Không tìm thấy đánh giá combo.";


                    return RedirectToAction(
                        nameof(Index)
                    );
                }


                review.PhanHoi =
                    reply;


                review.NgayPhanHoi =
                    DateTime.Now;


                review.MaTaiKhoanPhanHoi =
                    account.MaTaiKhoan;
            }


            // =====================================================
            // INVALID TYPE
            // =====================================================

            else
            {
                TempData["ReviewError"] =
                    "Loại đánh giá không hợp lệ.";


                return RedirectToAction(
                    nameof(Index)
                );
            }


            // =====================================================
            // SAVE
            // =====================================================

            await _context
                .SaveChangesAsync();


            TempData["ReviewSuccess"] =
                "Đã lưu phản hồi khách hàng.";


            return RedirectToAction(
                nameof(Details),
                new
                {
                    type,
                    id
                }
            );
        }
    }


    // =============================================================
    // INDEX VIEW MODEL
    // =============================================================

    public sealed class StaffReviewIndexViewModel
    {
        public List<StaffReviewItemViewModel> Reviews
        {
            get;
            set;
        } = new();


        public string? Keyword
        {
            get;
            set;
        }


        public string Type
        {
            get;
            set;
        } = "all";


        public int? Star
        {
            get;
            set;
        }


        public int CurrentPage
        {
            get;
            set;
        } = 1;


        public int PageSize
        {
            get;
            set;
        } = 10;


        public int TotalItems
        {
            get;
            set;
        }


        public int TotalPages
        {
            get;
            set;
        } = 1;


        public int TotalReviews
        {
            get;
            set;
        }


        public int ProductReviews
        {
            get;
            set;
        }


        public int ComboReviews
        {
            get;
            set;
        }


        public double AverageRating
        {
            get;
            set;
        }


        public int FiveStarReviews
        {
            get;
            set;
        }


        public int LowRatingReviews
        {
            get;
            set;
        }
    }


    // =============================================================
    // ITEM VIEW MODEL
    // =============================================================

    public sealed class StaffReviewItemViewModel
    {
        public int ReviewId
        {
            get;
            set;
        }


        public string ReviewType
        {
            get;
            set;
        } = "";


        public string TypeName
        {
            get;
            set;
        } = "";


        public int ItemId
        {
            get;
            set;
        }


        public string ItemName
        {
            get;
            set;
        } = "";


        public int CustomerId
        {
            get;
            set;
        }


        public string CustomerName
        {
            get;
            set;
        } = "";


        public string? CustomerEmail
        {
            get;
            set;
        }


        public int OrderId
        {
            get;
            set;
        }


        public int Stars
        {
            get;
            set;
        }


        public string? Content
        {
            get;
            set;
        }


        public DateTime ReviewDate
        {
            get;
            set;
        }


        public bool HasReply
        {
            get;
            set;
        }
    }


    // =============================================================
    // DETAILS VIEW MODEL
    // =============================================================

    public sealed class StaffReviewDetailsViewModel
    {
        public int ReviewId
        {
            get;
            set;
        }


        public string ReviewType
        {
            get;
            set;
        } = "";


        public string TypeName
        {
            get;
            set;
        } = "";


        public int ItemId
        {
            get;
            set;
        }


        public string ItemName
        {
            get;
            set;
        } = "";


        public int CustomerId
        {
            get;
            set;
        }


        public string CustomerName
        {
            get;
            set;
        } = "";


        public string? CustomerEmail
        {
            get;
            set;
        }


        public string? CustomerPhone
        {
            get;
            set;
        }


        public int OrderId
        {
            get;
            set;
        }


        public int Stars
        {
            get;
            set;
        }


        public string? Content
        {
            get;
            set;
        }


        public DateTime ReviewDate
        {
            get;
            set;
        }


        public string? Reply
        {
            get;
            set;
        }


        public DateTime? ReplyDate
        {
            get;
            set;
        }


        public string? ReplierName
        {
            get;
            set;
        }


        public string? ReplierRole
        {
            get;
            set;
        }
    }
}