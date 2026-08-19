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
        // SEARCH + FILTER + SORT + PAGINATION
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            string? keyword,
            int? roleId,
            string? status,
            string? sort,
            int page = 1,
            int pageSize = 10)
        {
            // =================================================
            // CHUẨN HÓA
            // =================================================

            page = page < 1
                ? 1
                : page;

            var allowedPageSizes =
                new[]
                {
                    5,
                    10,
                    20,
                    50
                };

            if (!allowedPageSizes.Contains(pageSize))
            {
                pageSize = 10;
            }


            keyword =
                string.IsNullOrWhiteSpace(keyword)
                    ? null
                    : keyword.Trim();


            status =
                string.IsNullOrWhiteSpace(status)
                    ? null
                    : status.Trim().ToLowerInvariant();


            sort =
                string.IsNullOrWhiteSpace(sort)
                    ? "newest"
                    : sort.Trim().ToLowerInvariant();


            // =================================================
            // QUERY
            // =================================================

            var query =
                _context.TaiKhoans
                    .AsNoTracking()
                    .Include(
                        x => x.MaVaiTroNavigation
                    )
                    .AsQueryable();


            // =================================================
            // TÌM KIẾM
            // =================================================

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query =
                    query.Where(
                        x =>
                            x.TenDangNhap.Contains(keyword)

                            ||

                            x.HoTen.Contains(keyword)

                            ||

                            (
                                x.Email != null &&
                                x.Email.Contains(keyword)
                            )

                            ||

                            (
                                x.DienThoai != null &&
                                x.DienThoai.Contains(keyword)
                            )

                            ||

                            (
                                x.MaVaiTroNavigation != null &&
                                x.MaVaiTroNavigation.TenVaiTro
                                    .Contains(keyword)
                            )
                    );
            }


            // =================================================
            // LỌC VAI TRÒ
            // =================================================

            if (roleId.HasValue)
            {
                query =
                    query.Where(
                        x => x.MaVaiTro == roleId.Value
                    );
            }


            // =================================================
            // LỌC TRẠNG THÁI
            // =================================================

            if (status == "active")
            {
                query =
                    query.Where(
                        x => x.IsActive
                    );
            }
            else if (status == "locked")
            {
                query =
                    query.Where(
                        x => !x.IsActive
                    );
            }


            // =================================================
            // SẮP XẾP
            // =================================================

            query =
                sort switch
                {
                    "oldest" =>
                        query
                            .OrderBy(
                                x => x.NgayTao
                            )
                            .ThenBy(
                                x => x.MaTaiKhoan
                            ),

                    "username_asc" =>
                        query
                            .OrderBy(
                                x => x.TenDangNhap
                            ),

                    "username_desc" =>
                        query
                            .OrderByDescending(
                                x => x.TenDangNhap
                            ),

                    "role" =>
                        query
                            .OrderBy(
                                x => x.MaVaiTroNavigation!
                                      .TenVaiTro
                            )
                            .ThenBy(
                                x => x.TenDangNhap
                            ),

                    _ =>
                        query
                            .OrderByDescending(
                                x => x.NgayTao
                            )
                            .ThenByDescending(
                                x => x.MaTaiKhoan
                            )
                };


            // =================================================
            // ĐẾM KẾT QUẢ SAU FILTER
            // =================================================

            var totalFiltered =
                await query.CountAsync();


            var totalPages =
                totalFiltered == 0
                    ? 1
                    : (int)Math.Ceiling(
                        totalFiltered /
                        (double)pageSize
                    );


            if (page > totalPages)
            {
                page = totalPages;
            }


            // =================================================
            // PHÂN TRANG
            // =================================================

            var accounts =
                await query
                    .Skip(
                        (page - 1) * pageSize
                    )
                    .Take(
                        pageSize
                    )
                    .ToListAsync();


            // =================================================
            // THỐNG KÊ TOÀN HỆ THỐNG
            // =================================================

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


            ViewBag.AdminAccounts =
                await _context.TaiKhoans
                    .CountAsync(
                        x =>
                            x.MaVaiTroNavigation != null
                            &&
                            x.MaVaiTroNavigation.TenVaiTro ==
                            "Admin"
                    );


            // =================================================
            // DỮ LIỆU FILTER
            // =================================================

            ViewBag.Keyword =
                keyword;

            ViewBag.RoleId =
                roleId;

            ViewBag.Status =
                status;

            ViewBag.Sort =
                sort;

            ViewBag.PageSize =
                pageSize;

            ViewBag.CurrentPage =
                page;

            ViewBag.TotalPages =
                totalPages;

            ViewBag.TotalFiltered =
                totalFiltered;


            ViewBag.Roles =
                await _context.VaiTros
                    .AsNoTracking()
                    .Where(
                        x => x.IsActive
                    )
                    .OrderBy(
                        x => x.MaVaiTro
                    )
                    .ToListAsync();


            return View(
                accounts
            );
        }


        // =====================================================
        // CHI TIẾT TÀI KHOẢN
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Details(
            int id)
        {
            var account =
                await _context.TaiKhoans
                    .AsNoTracking()
                    .Include(
                        x => x.MaVaiTroNavigation
                    )
                    .FirstOrDefaultAsync(
                        x => x.MaTaiKhoan == id
                    );


            if (account == null)
            {
                TempData["ErrorMessage"] =
                    "Không tìm thấy tài khoản.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            return View(
                account
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
                    "Vui lòng nhập đầy đủ tên đăng nhập, mật khẩu và họ tên.";

                await LoadRolesAsync(
                    maVaiTro
                );

                return View();
            }


            // =================================================
            // CHUẨN HÓA
            // =================================================

            tenDangNhap =
                tenDangNhap.Trim();

            hoTen =
                hoTen.Trim();

            email =
                NormalizeNullable(
                    email
                );

            dienThoai =
                NormalizeNullable(
                    dienThoai
                );


            // =================================================
            // MẬT KHẨU TỐI THIỂU
            // =================================================

            if (matKhau.Length < 6)
            {
                TempData["ErrorMessage"] =
                    "Mật khẩu phải có ít nhất 6 ký tự.";

                await LoadRolesAsync(
                    maVaiTro
                );

                return View();
            }


            // =================================================
            // USERNAME TRÙNG
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
            // EMAIL TRÙNG
            // =================================================

            if (!string.IsNullOrWhiteSpace(email))
            {
                var emailExists =
                    await _context.TaiKhoans
                        .AnyAsync(
                            x =>
                                x.Email != null
                                &&
                                x.Email == email
                        );


                if (emailExists)
                {
                    TempData["ErrorMessage"] =
                        "Email này đã được sử dụng bởi tài khoản khác.";

                    await LoadRolesAsync(
                        maVaiTro
                    );

                    return View();
                }
            }


            // =================================================
            // KIỂM TRA ROLE
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
            // NHÂN VIÊN CÓ ACCOUNT_MANAGE
            // KHÔNG ĐƯỢC TẠO ADMIN
            // =================================================

            if (!IsCurrentUserAdmin() &&
                IsAdminRole(role.TenVaiTro))
            {
                TempData["ErrorMessage"] =
                    "Chỉ quản trị viên mới có thể tạo tài khoản Admin.";

                await LoadRolesAsync(
                    maVaiTro
                );

                return View();
            }


            // =================================================
            // UPLOAD AVATAR
            // =================================================

            string? avatarFileName =
                null;


            if (hinhAnh != null &&
                hinhAnh.Length > 0)
            {
                var uploadResult =
                    await SaveAvatarAsync(
                        hinhAnh
                    );


                if (!uploadResult.Success)
                {
                    TempData["ErrorMessage"] =
                        uploadResult.ErrorMessage;

                    await LoadRolesAsync(
                        maVaiTro
                    );

                    return View();
                }


                avatarFileName =
                    uploadResult.FileName;
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
            // HASH PASSWORD
            // =================================================

            var passwordHasher =
                new PasswordHasher<TaiKhoan>();


            account.MatKhau =
                passwordHasher.HashPassword(
                    account,
                    matKhau
                );


            // =================================================
            // SAVE
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
        // FORM CHỈNH SỬA TÀI KHOẢN
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Edit(
            int id)
        {
            var account =
                await _context.TaiKhoans
                    .Include(
                        x => x.MaVaiTroNavigation
                    )
                    .FirstOrDefaultAsync(
                        x => x.MaTaiKhoan == id
                    );


            if (account == null)
            {
                TempData["ErrorMessage"] =
                    "Không tìm thấy tài khoản.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            // Nhân viên có ACCOUNT_MANAGE
            // không được chỉnh tài khoản Admin.

            if (!IsCurrentUserAdmin() &&
                account.MaVaiTroNavigation != null &&
                IsAdminRole(
                    account.MaVaiTroNavigation.TenVaiTro
                ))
            {
                TempData["ErrorMessage"] =
                    "Bạn không có quyền chỉnh sửa tài khoản Admin.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            await LoadRolesAsync(
                account.MaVaiTro
            );


            return View(
                account
            );
        }


        // =====================================================
        // CHỈNH SỬA TÀI KHOẢN
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            string hoTen,
            string? email,
            string? dienThoai,
            int maVaiTro,
            IFormFile? hinhAnh)
        {
            var account =
                await _context.TaiKhoans
                    .Include(
                        x => x.MaVaiTroNavigation
                    )
                    .FirstOrDefaultAsync(
                        x => x.MaTaiKhoan == id
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
            // BẢO VỆ ADMIN
            // =================================================

            if (!IsCurrentUserAdmin() &&
                account.MaVaiTroNavigation != null &&
                IsAdminRole(
                    account.MaVaiTroNavigation.TenVaiTro
                ))
            {
                TempData["ErrorMessage"] =
                    "Bạn không có quyền chỉnh sửa tài khoản Admin.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            // =================================================
            // HỌ TÊN
            // =================================================

            if (string.IsNullOrWhiteSpace(hoTen))
            {
                TempData["ErrorMessage"] =
                    "Họ và tên không được để trống.";

                await LoadRolesAsync(
                    maVaiTro
                );

                return View(
                    account
                );
            }


            hoTen =
                hoTen.Trim();

            email =
                NormalizeNullable(
                    email
                );

            dienThoai =
                NormalizeNullable(
                    dienThoai
                );


            // =================================================
            // EMAIL TRÙNG
            // =================================================

            if (!string.IsNullOrWhiteSpace(email))
            {
                var emailExists =
                    await _context.TaiKhoans
                        .AnyAsync(
                            x =>
                                x.MaTaiKhoan !=
                                account.MaTaiKhoan

                                &&

                                x.Email != null

                                &&

                                x.Email == email
                        );


                if (emailExists)
                {
                    TempData["ErrorMessage"] =
                        "Email này đã được sử dụng bởi tài khoản khác.";

                    await LoadRolesAsync(
                        maVaiTro
                    );

                    return View(
                        account
                    );
                }
            }


            // =================================================
            // ROLE
            // =================================================

            var newRole =
                await _context.VaiTros
                    .FirstOrDefaultAsync(
                        x =>
                            x.MaVaiTro == maVaiTro
                            &&
                            x.IsActive
                    );


            if (newRole == null)
            {
                TempData["ErrorMessage"] =
                    "Vai trò không hợp lệ hoặc đã ngừng hoạt động.";

                await LoadRolesAsync(
                    maVaiTro
                );

                return View(
                    account
                );
            }


            // =================================================
            // KHÔNG CHO NHÂN VIÊN GÁN ROLE ADMIN
            // =================================================

            if (!IsCurrentUserAdmin() &&
                IsAdminRole(
                    newRole.TenVaiTro
                ))
            {
                TempData["ErrorMessage"] =
                    "Chỉ quản trị viên mới có thể gán vai trò Admin.";

                await LoadRolesAsync(
                    account.MaVaiTro
                );

                return View(
                    account
                );
            }


            // =================================================
            // KHÔNG CHO TỰ ĐỔI ROLE CỦA CHÍNH MÌNH
            // =================================================

            var currentAccountId =
                HttpContext.Session.GetInt32(
                    "AdminAccountId"
                );


            if (currentAccountId.HasValue &&
                currentAccountId.Value ==
                account.MaTaiKhoan &&
                account.MaVaiTro != maVaiTro)
            {
                TempData["ErrorMessage"] =
                    "Bạn không thể thay đổi vai trò của chính tài khoản đang đăng nhập.";

                await LoadRolesAsync(
                    account.MaVaiTro
                );

                return View(
                    account
                );
            }


            // =================================================
            // AVATAR MỚI
            // =================================================

            string? oldAvatar =
                account.HinhAnh;


            if (hinhAnh != null &&
                hinhAnh.Length > 0)
            {
                var uploadResult =
                    await SaveAvatarAsync(
                        hinhAnh
                    );


                if (!uploadResult.Success)
                {
                    TempData["ErrorMessage"] =
                        uploadResult.ErrorMessage;

                    await LoadRolesAsync(
                        maVaiTro
                    );

                    return View(
                        account
                    );
                }


                account.HinhAnh =
                    uploadResult.FileName;
            }


            // =================================================
            // UPDATE
            // =================================================

            account.HoTen =
                hoTen;

            account.Email =
                email;

            account.DienThoai =
                dienThoai;

            account.MaVaiTro =
                maVaiTro;


            await _context.SaveChangesAsync();


            // =================================================
            // XÓA AVATAR CŨ NẾU CÓ ẢNH MỚI
            // =================================================

            if (hinhAnh != null &&
                hinhAnh.Length > 0 &&
                !string.IsNullOrWhiteSpace(oldAvatar) &&
                oldAvatar != account.HinhAnh)
            {
                DeleteAvatarFile(
                    oldAvatar
                );
            }


            // =================================================
            // NẾU SỬA CHÍNH MÌNH
            // ĐỒNG BỘ SESSION
            // =================================================

            if (currentAccountId.HasValue &&
                currentAccountId.Value ==
                account.MaTaiKhoan)
            {
                HttpContext.Session.SetString(
                    "AdminFullName",
                    account.HoTen
                );

                HttpContext.Session.SetString(
                    "AdminAvatar",
                    account.HinhAnh ?? ""
                );
            }


            TempData["SuccessMessage"] =
                $"Đã cập nhật tài khoản \"{account.TenDangNhap}\".";


            return RedirectToAction(
                nameof(Index)
            );
        }


        // =====================================================
        // FORM ĐẶT LẠI MẬT KHẨU
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> ResetPassword(
            int id)
        {
            var account =
                await _context.TaiKhoans
                    .AsNoTracking()
                    .Include(
                        x => x.MaVaiTroNavigation
                    )
                    .FirstOrDefaultAsync(
                        x => x.MaTaiKhoan == id
                    );


            if (account == null)
            {
                TempData["ErrorMessage"] =
                    "Không tìm thấy tài khoản.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            if (!IsCurrentUserAdmin() &&
                account.MaVaiTroNavigation != null &&
                IsAdminRole(
                    account.MaVaiTroNavigation.TenVaiTro
                ))
            {
                TempData["ErrorMessage"] =
                    "Bạn không có quyền đặt lại mật khẩu tài khoản Admin.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            return View(
                account
            );
        }


        // =====================================================
        // ĐẶT LẠI MẬT KHẨU
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            int id,
            string newPassword,
            string confirmPassword)
        {
            var account =
                await _context.TaiKhoans
                    .Include(
                        x => x.MaVaiTroNavigation
                    )
                    .FirstOrDefaultAsync(
                        x => x.MaTaiKhoan == id
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
            // BẢO VỆ ADMIN
            // =================================================

            if (!IsCurrentUserAdmin() &&
                account.MaVaiTroNavigation != null &&
                IsAdminRole(
                    account.MaVaiTroNavigation.TenVaiTro
                ))
            {
                TempData["ErrorMessage"] =
                    "Bạn không có quyền đặt lại mật khẩu tài khoản Admin.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            // =================================================
            // VALIDATE
            // =================================================

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                TempData["ErrorMessage"] =
                    "Vui lòng nhập mật khẩu mới.";

                return View(
                    account
                );
            }


            if (newPassword.Length < 6)
            {
                TempData["ErrorMessage"] =
                    "Mật khẩu mới phải có ít nhất 6 ký tự.";

                return View(
                    account
                );
            }


            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] =
                    "Xác nhận mật khẩu không khớp.";

                return View(
                    account
                );
            }


            // =================================================
            // HASH PASSWORD MỚI
            // =================================================

            var passwordHasher =
                new PasswordHasher<TaiKhoan>();


            account.MatKhau =
                passwordHasher.HashPassword(
                    account,
                    newPassword
                );


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Đã đặt lại mật khẩu cho tài khoản \"{account.TenDangNhap}\".";


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
                        x => x.MaTaiKhoan == id
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


            // =================================================
            // NHÂN VIÊN KHÔNG ĐƯỢC KHÓA ADMIN
            // =================================================

            if (!IsCurrentUserAdmin() &&
                account.MaVaiTroNavigation != null &&
                IsAdminRole(
                    account.MaVaiTroNavigation.TenVaiTro
                ))
            {
                TempData["ErrorMessage"] =
                    "Bạn không có quyền khóa hoặc mở khóa tài khoản Admin.";

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
                    .AsNoTracking()
                    .Where(
                        x => x.IsActive
                    )
                    .OrderBy(
                        x => x.MaVaiTro
                    )
                    .ToListAsync();


            // Nếu người hiện tại không phải Admin
            // thì không đưa role Admin vào dropdown.

            if (!IsCurrentUserAdmin())
            {
                roles =
                    roles
                        .Where(
                            x => !IsAdminRole(
                                x.TenVaiTro
                            )
                        )
                        .ToList();
            }


            ViewBag.Roles =
                roles;


            ViewBag.SelectedRole =
                selectedRole;
        }


        // =====================================================
        // KIỂM TRA NGƯỜI HIỆN TẠI CÓ PHẢI ADMIN
        // =====================================================

        private bool IsCurrentUserAdmin()
        {
            var roleName =
                HttpContext.Session.GetString(
                    "AdminRole"
                );


            return string.Equals(
                roleName,
                "Admin",
                StringComparison.OrdinalIgnoreCase
            );
        }


        // =====================================================
        // KIỂM TRA ROLE ADMIN
        // =====================================================

        private static bool IsAdminRole(
            string? roleName)
        {
            return string.Equals(
                roleName,
                "Admin",
                StringComparison.OrdinalIgnoreCase
            );
        }


        // =====================================================
        // CHUẨN HÓA CHUỖI NULLABLE
        // =====================================================

        private static string? NormalizeNullable(
            string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }


        // =====================================================
        // SAVE AVATAR
        // =====================================================

        private async Task<(
            bool Success,
            string? FileName,
            string? ErrorMessage
        )> SaveAvatarAsync(
            IFormFile image)
        {
            // =================================================
            // FILE RỖNG
            // =================================================

            if (image.Length <= 0)
            {
                return (
                    false,
                    null,
                    "File ảnh không hợp lệ."
                );
            }


            // =================================================
            // MAX 5MB
            // =================================================

            const long maxFileSize =
                5 * 1024 * 1024;


            if (image.Length > maxFileSize)
            {
                return (
                    false,
                    null,
                    "Ảnh đại diện không được vượt quá 5MB."
                );
            }


            // =================================================
            // EXTENSION
            // =================================================

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
                    image.FileName
                )
                .ToLowerInvariant();


            if (string.IsNullOrWhiteSpace(extension) ||
                !allowedExtensions.Contains(extension))
            {
                return (
                    false,
                    null,
                    "Ảnh đại diện chỉ hỗ trợ JPG, JPEG, PNG hoặc WEBP."
                );
            }


            // =================================================
            // CONTENT TYPE
            // =================================================

            var allowedContentTypes =
                new[]
                {
                    "image/jpeg",
                    "image/png",
                    "image/webp"
                };


            var contentType =
                image.ContentType?
                    .ToLowerInvariant()
                ?? "";


            if (!allowedContentTypes.Contains(contentType))
            {
                return (
                    false,
                    null,
                    "File được chọn không phải định dạng ảnh hợp lệ."
                );
            }


            // =================================================
            // UNIQUE FILE NAME
            // =================================================

            var fileName =
                $"{Guid.NewGuid():N}{extension}";


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


            var filePath =
                Path.Combine(
                    uploadFolder,
                    fileName
                );


            await using (
                var stream =
                    new FileStream(
                        filePath,
                        FileMode.Create
                    )
            )
            {
                await image.CopyToAsync(
                    stream
                );
            }


            return (
                true,
                fileName,
                null
            );
        }


        // =====================================================
        // DELETE AVATAR CŨ
        // =====================================================

        private void DeleteAvatarFile(
            string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }


            var safeFileName =
                Path.GetFileName(
                    fileName
                );


            if (string.IsNullOrWhiteSpace(safeFileName))
            {
                return;
            }


            var filePath =
                Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "avatars",
                    safeFileName
                );


            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(
                    filePath
                );
            }
        }
    }
}