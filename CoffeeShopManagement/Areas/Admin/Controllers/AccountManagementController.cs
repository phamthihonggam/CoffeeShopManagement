using CoffeeShopManagement.Data;
using CoffeeShopManagement.Helpers;
using CoffeeShopManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize("ACCOUNT_MANAGE")]
    public class AccountManagementController : Controller
    {
        private readonly CoffeeShopDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AccountManagementController(
            CoffeeShopDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        // =====================================================
        // DANH SÁCH TÀI KHOẢN
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            string? keyword)
        {
            var query =
                _context.TaiKhoans

                    .Include(
                        x => x.MaVaiTroNavigation
                    )

                    .AsQueryable();


            // =================================================
            // TÌM KIẾM
            // =================================================

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword =
                    keyword.Trim();


                query =
                    query.Where(x =>

                        x.TenDangNhap.Contains(keyword)

                        ||

                        x.HoTen.Contains(keyword)

                        ||

                        (x.Email != null &&
                         x.Email.Contains(keyword))

                        ||

                        (x.DienThoai != null &&
                         x.DienThoai.Contains(keyword))

                        ||

                        x.MaVaiTroNavigation.TenVaiTro.Contains(keyword)
                    );
            }


            var accounts =
                await query

                    .OrderByDescending(
                        x => x.NgayTao
                    )

                    .ThenBy(
                        x => x.MaTaiKhoan
                    )

                    .ToListAsync();


            ViewBag.Keyword =
                keyword;


            ViewBag.TotalAccounts =
                await _context.TaiKhoans
                    .CountAsync();


            ViewBag.ActiveAccounts =
                await _context.TaiKhoans
                    .CountAsync(
                        x => x.IsActive
                    );


            ViewBag.LockedAccounts =
                await _context.TaiKhoans
                    .CountAsync(
                        x => !x.IsActive
                    );


            return View(
                accounts
            );
        }


        // =====================================================
        // FORM THÊM TÀI KHOẢN
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadRolesAsync();

            return View();
        }


        // =====================================================
        // THÊM TÀI KHOẢN
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string tenDangNhap,
            string matKhau,
            string hoTen,
            string? email,
            string? dienThoai,
            int maVaiTro,
            IFormFile? hinhAnh)
        {
            // =================================================
            // KIỂM TRA RỖNG
            // =================================================

            if (string.IsNullOrWhiteSpace(tenDangNhap) ||
                string.IsNullOrWhiteSpace(matKhau) ||
                string.IsNullOrWhiteSpace(hoTen))
            {
                TempData["ErrorMessage"] =
                    "Vui lòng nhập đầy đủ tài khoản, mật khẩu và họ tên.";

                await LoadRolesAsync(
                    maVaiTro
                );

                return View();
            }


            tenDangNhap =
                tenDangNhap.Trim();

            hoTen =
                hoTen.Trim();

            email =
                string.IsNullOrWhiteSpace(email)
                    ? null
                    : email.Trim();

            dienThoai =
                string.IsNullOrWhiteSpace(dienThoai)
                    ? null
                    : dienThoai.Trim();


            // =================================================
            // KIỂM TRA USERNAME TRÙNG
            // =================================================

            var usernameExists =
                await _context.TaiKhoans

                    .AnyAsync(
                        x =>
                            x.TenDangNhap ==
                            tenDangNhap
                    );


            if (usernameExists)
            {
                TempData["ErrorMessage"] =
                    "Tên đăng nhập đã tồn tại.";

                await LoadRolesAsync(
                    maVaiTro
                );

                return View();
            }


            // =================================================
            // KIỂM TRA VAI TRÒ
            // =================================================

            var role =
                await _context.VaiTros

                    .FirstOrDefaultAsync(
                        x =>
                            x.MaVaiTro == maVaiTro
                            &&
                            x.IsActive
                    );


            if (role == null)
            {
                TempData["ErrorMessage"] =
                    "Vai trò không hợp lệ hoặc đã ngừng hoạt động.";

                await LoadRolesAsync(
                    maVaiTro
                );

                return View();
            }


            // =================================================
            // XỬ LÝ AVATAR
            // =================================================

            string? avatarFileName =
                null;


            if (hinhAnh != null &&
                hinhAnh.Length > 0)
            {
                // =============================================
                // KIỂM TRA DUNG LƯỢNG
                // TỐI ĐA 5MB
                // =============================================

                const long maxFileSize =
                    5 * 1024 * 1024;


                if (hinhAnh.Length > maxFileSize)
                {
                    TempData["ErrorMessage"] =
                        "Ảnh đại diện không được vượt quá 5MB.";

                    await LoadRolesAsync(
                        maVaiTro
                    );

                    return View();
                }


                // =============================================
                // KIỂM TRA ĐUÔI FILE
                // =============================================

                var allowedExtensions =
                    new[]
                    {
                        ".jpg",
                        ".jpeg",
                        ".png",
                        ".webp"
                    };


                var extension =
                    Path.GetExtension(
                        hinhAnh.FileName
                    )
                    .ToLowerInvariant();


                if (string.IsNullOrWhiteSpace(extension) ||
                    !allowedExtensions.Contains(extension))
                {
                    TempData["ErrorMessage"] =
                        "Ảnh đại diện chỉ hỗ trợ JPG, JPEG, PNG hoặc WEBP.";

                    await LoadRolesAsync(
                        maVaiTro
                    );

                    return View();
                }


                // =============================================
                // KIỂM TRA CONTENT TYPE
                // =============================================

                var allowedContentTypes =
                    new[]
                    {
                        "image/jpeg",
                        "image/png",
                        "image/webp"
                    };


                if (!allowedContentTypes.Contains(
                        hinhAnh.ContentType.ToLowerInvariant()
                    ))
                {
                    TempData["ErrorMessage"] =
                        "File được chọn không phải định dạng ảnh hợp lệ.";

                    await LoadRolesAsync(
                        maVaiTro
                    );

                    return View();
                }


                // =============================================
                // TẠO TÊN FILE DUY NHẤT
                // =============================================

                avatarFileName =
                    $"{Guid.NewGuid():N}{extension}";


                // =============================================
                // THƯ MỤC LƯU AVATAR
                // =============================================

                var uploadFolder =
                    Path.Combine(
                        _environment.WebRootPath,
                        "images",
                        "avatars"
                    );


                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(
                        uploadFolder
                    );
                }


                // =============================================
                // FULL PATH
                // =============================================

                var filePath =
                    Path.Combine(
                        uploadFolder,
                        avatarFileName
                    );


                // =============================================
                // SAVE FILE
                // =============================================

                await using (
                    var stream =
                        new FileStream(
                            filePath,
                            FileMode.Create
                        )
                )
                {
                    await hinhAnh.CopyToAsync(
                        stream
                    );
                }
            }


            // =================================================
            // TẠO TÀI KHOẢN
            // =================================================

            var account =
                new TaiKhoan
                {
                    TenDangNhap =
                        tenDangNhap,

                    HoTen =
                        hoTen,

                    Email =
                        email,

                    DienThoai =
                        dienThoai,

                    MaVaiTro =
                        maVaiTro,

                    IsActive =
                        true,

                    NgayTao =
                        DateTime.Now,

                    HinhAnh =
                        avatarFileName,

                    MatKhau =
                        ""
                };


            // =================================================
            // HASH MẬT KHẨU
            // =================================================

            var passwordHasher =
                new PasswordHasher<TaiKhoan>();


            account.MatKhau =
                passwordHasher.HashPassword(
                    account,
                    matKhau
                );


            // =================================================
            // SAVE DATABASE
            // =================================================

            _context.TaiKhoans.Add(
                account
            );


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Đã thêm tài khoản \"{account.TenDangNhap}\" thành công.";


            return RedirectToAction(
                nameof(Index)
            );
        }


        // =====================================================
        // KHÓA / MỞ KHÓA
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(
            int id)
        {
            var account =
                await _context.TaiKhoans

                    .Include(
                        x => x.MaVaiTroNavigation
                    )

                    .FirstOrDefaultAsync(
                        x =>
                            x.MaTaiKhoan == id
                    );


            if (account == null)
            {
                TempData["ErrorMessage"] =
                    "Không tìm thấy tài khoản.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            // =================================================
            // KHÔNG CHO TỰ KHÓA CHÍNH MÌNH
            // =================================================

            var currentAccountId =
                HttpContext.Session.GetInt32(
                    "AdminAccountId"
                );


            if (currentAccountId.HasValue &&
                currentAccountId.Value ==
                account.MaTaiKhoan)
            {
                TempData["ErrorMessage"] =
                    "Bạn không thể tự khóa tài khoản đang đăng nhập.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            account.IsActive =
                !account.IsActive;


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                account.IsActive
                    ? $"Đã mở khóa tài khoản \"{account.TenDangNhap}\"."
                    : $"Đã khóa tài khoản \"{account.TenDangNhap}\".";


            return RedirectToAction(
                nameof(Index)
            );
        }


        // =====================================================
        // LOAD VAI TRÒ
        // =====================================================

        private async Task LoadRolesAsync(
            int? selectedRole = null)
        {
            var roles =
                await _context.VaiTros

                    .Where(
                        x => x.IsActive
                    )

                    .OrderBy(
                        x => x.MaVaiTro
                    )

                    .ToListAsync();


            ViewBag.Roles =
                roles;


            ViewBag.SelectedRole =
                selectedRole;
        }
    }
}