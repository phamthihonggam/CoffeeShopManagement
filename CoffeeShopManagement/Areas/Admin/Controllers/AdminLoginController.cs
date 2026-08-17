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
        public async Task<IActionResult> Index()
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


            // =================================================
            // TẠO TÀI KHOẢN MẶC ĐỊNH
            // CHỈ TẠO NẾU CHƯA TỒN TẠI
            // =================================================

            await CreateDefaultAccountsAsync();


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
            // KIỂM TRA RỖNG
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
            // TÌM TÀI KHOẢN
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
            // KHÔNG TÌM THẤY TÀI KHOẢN
            // =================================================

            if (account == null)
            {
                ViewBag.ErrorMessage =
                    "Tài khoản hoặc mật khẩu không chính xác.";

                return View();
            }


            // =================================================
            // TÀI KHOẢN BỊ KHÓA
            // =================================================

            if (!account.IsActive)
            {
                ViewBag.ErrorMessage =
                    "Tài khoản này hiện đang bị khóa.";

                return View();
            }


            // =================================================
            // VAI TRÒ KHÔNG HOẠT ĐỘNG
            // =================================================

            if (account.MaVaiTroNavigation == null ||
                !account.MaVaiTroNavigation.IsActive)
            {
                ViewBag.ErrorMessage =
                    "Vai trò của tài khoản hiện không hoạt động.";

                return View();
            }


            // =================================================
            // KIỂM TRA MẬT KHẨU HASH
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
            // CHỈ ADMIN / NHÂN VIÊN ĐƯỢC VÀO ADMIN
            // =================================================

            var roleName =
                account.MaVaiTroNavigation.TenVaiTro;


            if (roleName != "Admin" &&
                roleName != "Nhân viên")
            {
                ViewBag.ErrorMessage =
                    "Tài khoản này không có quyền truy cập khu vực quản trị.";

                return View();
            }


            // =================================================
            // LẤY QUYỀN CỦA VAI TRÒ
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

                    .ToListAsync();


            // =================================================
            // LƯU SESSION
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
                account.HoTen
            );


            // Vai trò

            HttpContext.Session.SetString(
                "AdminRole",
                roleName
            );


            // =================================================
            // ẢNH ĐẠI DIỆN
            // =================================================

            HttpContext.Session.SetString(
                "AdminAvatar",
                account.HinhAnh ?? ""
            );


            // =================================================
            // DANH SÁCH QUYỀN
            // =================================================

            HttpContext.Session.SetString(
                "AdminPermissions",
                string.Join(
                    "|",
                    permissions
                )
            );


            // =================================================
            // ĐĂNG NHẬP THÀNH CÔNG
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
            // =================================================
            // XÓA SESSION
            // =================================================

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


            // Ảnh đại diện

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


        // =====================================================
        // TẠO TÀI KHOẢN MẶC ĐỊNH
        // =====================================================

        private async Task CreateDefaultAccountsAsync()
        {
            var passwordHasher =
                new PasswordHasher<TaiKhoan>();


            // =================================================
            // 1. TẠO ADMIN
            // =================================================

            var adminRole =
                await _context.VaiTros

                    .FirstOrDefaultAsync(
                        x => x.TenVaiTro == "Admin"
                    );


            if (adminRole != null)
            {
                var adminExists =
                    await _context.TaiKhoans

                        .AnyAsync(
                            x => x.TenDangNhap == "admin"
                        );


                if (!adminExists)
                {
                    var admin =
                        new TaiKhoan
                        {
                            TenDangNhap =
                                "admin",

                            HoTen =
                                "Quản trị viên",

                            Email =
                                null,

                            DienThoai =
                                null,

                            HinhAnh =
                                null,

                            MaVaiTro =
                                adminRole.MaVaiTro,

                            IsActive =
                                true,

                            NgayTao =
                                DateTime.Now,

                            MatKhau =
                                ""
                        };


                    admin.MatKhau =
                        passwordHasher.HashPassword(
                            admin,
                            "123456"
                        );


                    _context.TaiKhoans.Add(
                        admin
                    );
                }
            }


            // =================================================
            // 2. TẠO NHÂN VIÊN
            // =================================================

            var employeeRole =
                await _context.VaiTros

                    .FirstOrDefaultAsync(
                        x => x.TenVaiTro == "Nhân viên"
                    );


            if (employeeRole != null)
            {
                var employeeExists =
                    await _context.TaiKhoans

                        .AnyAsync(
                            x => x.TenDangNhap == "nhanvien"
                        );


                if (!employeeExists)
                {
                    var employee =
                        new TaiKhoan
                        {
                            TenDangNhap =
                                "nhanvien",

                            HoTen =
                                "Nhân viên Rosalie",

                            Email =
                                null,

                            DienThoai =
                                null,

                            HinhAnh =
                                null,

                            MaVaiTro =
                                employeeRole.MaVaiTro,

                            IsActive =
                                true,

                            NgayTao =
                                DateTime.Now,

                            MatKhau =
                                ""
                        };


                    employee.MatKhau =
                        passwordHasher.HashPassword(
                            employee,
                            "123456"
                        );


                    _context.TaiKhoans.Add(
                        employee
                    );
                }
            }


            // =================================================
            // SAVE
            // =================================================

            await _context.SaveChangesAsync();
        }
    }
}