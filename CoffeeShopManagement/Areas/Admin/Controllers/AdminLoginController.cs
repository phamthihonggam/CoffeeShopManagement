using CoffeeShopManagement.Data;
using CoffeeShopManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminLoginController : Controller
    {
        private readonly CoffeeShopDbContext _context;

        public AdminLoginController(
            CoffeeShopDbContext context)
        {
            _context = context;
        }


        // =====================================================
        // TRANG ĐĂNG NHẬP
        // =====================================================

        [HttpGet]
        public IActionResult Index()
        {
            // Nếu đã đăng nhập thì về Dashboard

            if (HttpContext.Session.GetString(
                    "AdminLoggedIn"
                ) == "true")
            {
                return RedirectToAction(
                    "Index",
                    "Dashboard",
                    new
                    {
                        area = "Admin"
                    }
                );
            }

            return View();
        }


        // =====================================================
        // XỬ LÝ ĐĂNG NHẬP
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            string username,
            string password)
        {
            // =================================================
            // 1. KIỂM TRA RỖNG
            // =================================================

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage =
                    "Vui lòng nhập đầy đủ tài khoản và mật khẩu.";

                return View();
            }


            username = username.Trim();


            // =================================================
            // 2. TÌM TÀI KHOẢN
            // =================================================

            var account =
                await _context.TaiKhoans
                    .Include(
                        x => x.MaVaiTroNavigation
                    )
                    .FirstOrDefaultAsync(
                        x => x.TenDangNhap == username
                    );


            // =================================================
            // 3. KHÔNG TÌM THẤY TÀI KHOẢN
            // =================================================

            if (account == null)
            {
                ViewBag.ErrorMessage =
                    "Tài khoản hoặc mật khẩu không chính xác.";

                return View();
            }


            // =================================================
            // 4. KIỂM TRA TÀI KHOẢN BỊ KHÓA
            // =================================================

            if (!account.IsActive)
            {
                ViewBag.ErrorMessage =
                    "Tài khoản này hiện đang bị khóa.";

                return View();
            }


            // =================================================
            // 5. KIỂM TRA VAI TRÒ
            // =================================================

            if (account.MaVaiTroNavigation == null ||
                !account.MaVaiTroNavigation.IsActive)
            {
                ViewBag.ErrorMessage =
                    "Vai trò của tài khoản hiện không hoạt động.";

                return View();
            }


            // =================================================
            // 6. KIỂM TRA MẬT KHẨU HASH
            // =================================================

            var passwordHasher =
                new PasswordHasher<TaiKhoan>();

            var verifyResult =
                passwordHasher.VerifyHashedPassword(
                    account,
                    account.MatKhau,
                    password
                );


            if (verifyResult ==
                PasswordVerificationResult.Failed)
            {
                ViewBag.ErrorMessage =
                    "Tài khoản hoặc mật khẩu không chính xác.";

                return View();
            }


            // =================================================
            // 7. KIỂM TRA ROLE
            // CHỈ ADMIN / NHÂN VIÊN ĐƯỢC VÀO KHU QUẢN TRỊ
            // =================================================

            var roleName =
                account.MaVaiTroNavigation.TenVaiTro;


            var isAdmin =
                string.Equals(
                    roleName,
                    "Admin",
                    StringComparison.OrdinalIgnoreCase
                );


            var isStaff =
                string.Equals(
                    roleName,
                    "Nhân viên",
                    StringComparison.OrdinalIgnoreCase
                );


            if (!isAdmin && !isStaff)
            {
                ViewBag.ErrorMessage =
                    "Tài khoản này không có quyền truy cập khu vực quản trị.";

                return View();
            }


            // =================================================
            // 8. LẤY QUYỀN CỦA VAI TRÒ TỪ DATABASE
            // =================================================

            var permissions =
                await _context.VaiTros
                    .Where(
                        x => x.MaVaiTro ==
                             account.MaVaiTro
                    )
                    .SelectMany(
                        x => x.MaQuyens
                    )
                    .Where(
                        x => x.IsActive
                    )
                    .Select(
                        x => x.TenQuyen
                    )
                    .Distinct()
                    .ToListAsync();


            // =================================================
            // 9. LƯU SESSION
            // =================================================


            // Đã đăng nhập

            HttpContext.Session.SetString(
                "AdminLoggedIn",
                "true"
            );


            // ID tài khoản

            HttpContext.Session.SetInt32(
                "AdminAccountId",
                account.MaTaiKhoan
            );


            // Tên đăng nhập

            HttpContext.Session.SetString(
                "AdminUsername",
                account.TenDangNhap
            );


            // Họ tên

            HttpContext.Session.SetString(
                "AdminFullName",
                account.HoTen ?? account.TenDangNhap
            );


            // Vai trò

            HttpContext.Session.SetString(
                "AdminRole",
                roleName
            );


            // Ảnh đại diện

            HttpContext.Session.SetString(
                "AdminAvatar",
                account.HinhAnh ?? ""
            );


            // Danh sách quyền

            HttpContext.Session.SetString(
                "AdminPermissions",
                string.Join(
                    "|",
                    permissions
                )
            );


            // =================================================
            // 10. ĐĂNG NHẬP THÀNH CÔNG
            // =================================================

            return RedirectToAction(
                "Index",
                "Dashboard",
                new
                {
                    area = "Admin"
                }
            );
        }


        // =====================================================
        // KHÔNG CÓ QUYỀN TRUY CẬP
        // =====================================================

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        // =====================================================
        // ĐĂNG XUẤT
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Chỉ xóa session của khu quản trị
            // Không Session.Clear() để tránh ảnh hưởng
            // các session khác như giỏ hàng khách hàng.

            HttpContext.Session.Remove(
                "AdminLoggedIn"
            );

            HttpContext.Session.Remove(
                "AdminAccountId"
            );

            HttpContext.Session.Remove(
                "AdminUsername"
            );

            HttpContext.Session.Remove(
                "AdminFullName"
            );

            HttpContext.Session.Remove(
                "AdminRole"
            );

            HttpContext.Session.Remove(
                "AdminAvatar"
            );

            HttpContext.Session.Remove(
                "AdminPermissions"
            );


            // =================================================
            // QUAY VỀ LOGIN
            // =================================================

            return RedirectToAction(
                "Index",
                "AdminLogin",
                new
                {
                    area = "Admin"
                }
            );
        }
    }
}