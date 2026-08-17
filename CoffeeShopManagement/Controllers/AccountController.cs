using CoffeeShopManagement.Data;
using CoffeeShopManagement.Helpers;
using CoffeeShopManagement.Models;
using CoffeeShopManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;
using CoffeeShopManagement.Services;
using Microsoft.Extensions.Localization;

namespace CoffeeShopManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly CoffeeShopDbContext _context;
        private readonly EmailService _emailService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AccountController(
            CoffeeShopDbContext context,
            EmailService emailService,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _emailService = emailService;
            _localizer = localizer;
        }

        // =========================================================
        // REGISTER - GET
        // =========================================================

        [HttpGet]
        public IActionResult Register()
        {
            // Nếu đã đăng nhập thì không cho đăng ký lại
            if (HttpContext.Session.GetInt32("MaKH") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // =========================================================
        // REGISTER - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(DangKyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Chuẩn hóa dữ liệu
            string email = model.Email.Trim().ToLower();
            string hoTen = model.HoTen.Trim();
            string dienThoai = model.DienThoai.Trim();

            string? diaChi = string.IsNullOrWhiteSpace(model.DiaChi)
                ? null
                : model.DiaChi.Trim();

            // Kiểm tra Email đã tồn tại
            bool emailExists = await _context.KhachHangs
                .AnyAsync(x => x.Email.ToLower() == email);

            if (emailExists)
            {
                ModelState.AddModelError(
                    "Email",
                    _localizer["EmailAlreadyUsed"].Value
                );

                return View(model);
            }

            // Tạo khách hàng mới
            var khachHang = new KhachHang
            {
                HoTen = hoTen,
                Email = email,
                DienThoai = dienThoai,
                DiaChi = diaChi,
                MatKhau = PasswordHelper.Hash(model.MatKhau),
                NgayTao = DateTime.Now
            };

            _context.KhachHangs.Add(khachHang);

            await _context.SaveChangesAsync();

            // =====================================================
            // AUTO LOGIN SAU KHI ĐĂNG KÝ
            // =====================================================

            HttpContext.Session.SetInt32(
                "MaKH",
                khachHang.MaKh
            );

            HttpContext.Session.SetString(
                "HoTen",
                khachHang.HoTen
            );

            HttpContext.Session.SetString(
                "Email",
                khachHang.Email
            );

            HttpContext.Session.SetString(
                "VaiTro",
                "KhachHang"
            );

            // Lưu email để sử dụng cho lần đăng nhập sau
            Response.Cookies.Append(
                "LastEmail",
                khachHang.Email,
                new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(30),
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax
                }
            );

            TempData["Success"] =
                _localizer[
                    "RegisterWelcomeSuccess",
                    khachHang.HoTen
                ].Value;

            return RedirectToAction("Index", "Home");
        }

        // =========================================================
        // LOGIN - GET
        // ADMIN + NHÂN VIÊN + GIAO HÀNG + KHÁCH HÀNG
        // =========================================================

        [HttpGet]
        public IActionResult Login()
        {
            // Admin / Nhân viên đã đăng nhập bằng Cookie
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction(
                        "Index",
                        "Dashboard",
                        new { area = "Admin" }
                    );
                }

                if (User.IsInRole("NhanVien"))
                {
                    return RedirectToAction(
                        "Index",
                        "Dashboard",
                        new { area = "Staff" }
                    );
                }

                if (User.IsInRole("GiaoHang"))
                {
                    return RedirectToAction(
                        "Index",
                        "Dashboard",
                        new { area = "Delivery" }
                    );
                }
            }

            // Khách hàng đã đăng nhập bằng Session
            if (HttpContext.Session.GetInt32("MaKH") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new DangNhapViewModel();

            // Lấy Email / tên đăng nhập đã lưu
            if (Request.Cookies.TryGetValue(
                "LastEmail",
                out string? lastEmail))
            {
                model.Email = lastEmail;
            }

            return View(model);
        }

        // =========================================================
        // LOGIN - POST
        // ADMIN + NHÂN VIÊN + GIAO HÀNG + KHÁCH HÀNG
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(DangNhapViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string login = model.Email.Trim().ToLower();

            // =====================================================
            // 1. KIỂM TRA TÀI KHOẢN ADMIN / NHÂN VIÊN / GIAO HÀNG
            // =====================================================

            var taiKhoan = await _context.TaiKhoans
                .Include(x => x.MaVaiTroNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.IsActive &&
                    (
                        x.TenDangNhap.ToLower() == login ||
                        (
                            x.Email != null &&
                            x.Email.ToLower() == login
                        )
                    )
                );

            if (taiKhoan != null)
            {
                bool passwordCorrect =
                PasswordHelper.VerifyAccount(
                    taiKhoan,
                    model.MatKhau
                );

                if (!passwordCorrect)
                {
                    ModelState.AddModelError(
                        "",
                        _localizer["InvalidEmailOrPassword"].Value
                    );

                    return View(model);
                }

                string databaseRole =
                    taiKhoan.MaVaiTroNavigation
                        .TenVaiTro
                        .Trim();

                string role;

                if (databaseRole.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase))
                {
                    role = "Admin";
                }
                else if (
                    databaseRole.Equals(
                        "NhanVien",
                        StringComparison.OrdinalIgnoreCase) ||
                    databaseRole.Equals(
                        "Nhân viên",
                        StringComparison.OrdinalIgnoreCase) ||
                    databaseRole.Equals(
                        "Nhan Vien",
                        StringComparison.OrdinalIgnoreCase) ||
                    databaseRole.Equals(
                        "Staff",
                        StringComparison.OrdinalIgnoreCase))
                {
                    role = "NhanVien";
                }
                else if (
                    databaseRole.Equals(
                        "GiaoHang",
                        StringComparison.OrdinalIgnoreCase) ||
                    databaseRole.Equals(
                        "Giao hàng",
                        StringComparison.OrdinalIgnoreCase) ||
                    databaseRole.Equals(
                        "Giao Hang",
                        StringComparison.OrdinalIgnoreCase) ||
                    databaseRole.Equals(
                        "Shipper",
                        StringComparison.OrdinalIgnoreCase))
                {
                    role = "GiaoHang";
                }
                else
                {
                    ModelState.AddModelError(
                        "",
                        "Tài khoản chưa được cấp vai trò đăng nhập hợp lệ."
                    );

                    return View(model);
                }

                // =================================================
                // CLAIMS
                // =================================================

                var claims = new List<Claim>
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        taiKhoan.MaTaiKhoan.ToString()
                    ),

                    new Claim(
                        ClaimTypes.Name,
                        taiKhoan.HoTen
                    ),

                    new Claim(
                        ClaimTypes.Role,
                        role
                    ),

                    new Claim(
                        "TenDangNhap",
                        taiKhoan.TenDangNhap
                    )
                };

                if (!string.IsNullOrWhiteSpace(taiKhoan.Email))
                {
                    claims.Add(
                        new Claim(
                            ClaimTypes.Email,
                            taiKhoan.Email
                        )
                    );
                }

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                var principal = new ClaimsPrincipal(identity);

                var authenticationProperties =
                    new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,

                        ExpiresUtc = model.RememberMe
                            ? DateTimeOffset.UtcNow.AddDays(7)
                            : DateTimeOffset.UtcNow.AddHours(2)
                    };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authenticationProperties
                );

                // =================================================
                // SESSION
                // =================================================

                HttpContext.Session.SetInt32(
                    "MaTaiKhoan",
                    taiKhoan.MaTaiKhoan
                );

                HttpContext.Session.SetString(
                    "HoTen",
                    taiKhoan.HoTen
                );

                HttpContext.Session.SetString(
                    "VaiTro",
                    role
                );

                HttpContext.Session.SetString(
                    "TenDangNhap",
                    taiKhoan.TenDangNhap
                );

                if (!string.IsNullOrWhiteSpace(taiKhoan.Email))
                {
                    HttpContext.Session.SetString(
                        "Email",
                        taiKhoan.Email
                    );
                }

                // =================================================
                // GHI NHỚ TÀI KHOẢN
                // =================================================

                if (model.RememberMe)
                {
                    Response.Cookies.Append(
                        "LastEmail",
                        !string.IsNullOrWhiteSpace(taiKhoan.Email)
                            ? taiKhoan.Email
                            : taiKhoan.TenDangNhap,
                        new CookieOptions
                        {
                            Expires =
                                DateTimeOffset.Now.AddDays(30),

                            HttpOnly = true,
                            IsEssential = true,
                            SameSite = SameSiteMode.Lax
                        }
                    );
                }
                else
                {
                    Response.Cookies.Delete("LastEmail");
                }

                // =================================================
                // ĐIỀU HƯỚNG THEO VAI TRÒ
                // =================================================

                if (role == "Admin")
                {
                    TempData["Success"] =
                        $"Xin chào Admin {taiKhoan.HoTen}!";

                    return RedirectToAction(
                        "Index",
                        "Dashboard",
                        new { area = "Admin" }
                    );
                }

                if (role == "NhanVien")
                {
                    TempData["Success"] =
                        $"Xin chào nhân viên {taiKhoan.HoTen}!";

                    return RedirectToAction(
                        "Index",
                        "Dashboard",
                        new { area = "Staff" }
                    );
                }

                if (role == "GiaoHang")
                {
                    TempData["Success"] =
                        $"Xin chào nhân viên giao hàng {taiKhoan.HoTen}!";

                    return RedirectToAction(
                        "Index",
                        "Dashboard",
                        new { area = "Delivery" }
                    );
                }

                return RedirectToAction(
                    "Index",
                    "Home"
                );
            }

            // =====================================================
            // 2. KIỂM TRA KHÁCH HÀNG
            // =====================================================

            var khachHang = await _context.KhachHangs
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Email.ToLower() == login
                );

            if (khachHang == null)
            {
                ModelState.AddModelError(
                    "",
                    _localizer["InvalidEmailOrPassword"].Value
                );

                return View(model);
            }

            bool customerPasswordCorrect =
                PasswordHelper.Verify(
                    model.MatKhau,
                    khachHang.MatKhau
                );

            if (!customerPasswordCorrect)
            {
                ModelState.AddModelError(
                    "",
                    _localizer["InvalidEmailOrPassword"].Value
                );

                return View(model);
            }

            // =====================================================
            // SESSION KHÁCH HÀNG
            // =====================================================

            HttpContext.Session.SetInt32(
                "MaKH",
                khachHang.MaKh
            );

            HttpContext.Session.SetString(
                "HoTen",
                khachHang.HoTen
            );

            HttpContext.Session.SetString(
                "Email",
                khachHang.Email
            );

            HttpContext.Session.SetString(
                "VaiTro",
                "KhachHang"
            );

            // =====================================================
            // REMEMBER EMAIL
            // =====================================================

            if (model.RememberMe)
            {
                Response.Cookies.Append(
                    "LastEmail",
                    khachHang.Email,
                    new CookieOptions
                    {
                        Expires =
                            DateTimeOffset.Now.AddDays(30),

                        HttpOnly = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.Lax
                    }
                );
            }
            else
            {
                Response.Cookies.Delete("LastEmail");
            }

            TempData["Success"] =
                _localizer[
                    "LoginWelcomeSuccess",
                    khachHang.HoTen
                ].Value;

            return RedirectToAction(
                "Index",
                "Home"
            );
        }

        // =========================================================
        // GOOGLE LOGIN
        // =========================================================

        [HttpGet]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action(
                nameof(GoogleCallback),
                "Account"
            );

            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            return Challenge(
                properties,
                GoogleDefaults.AuthenticationScheme
            );
        }

        // =========================================================
        // GOOGLE CALLBACK
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync();

            if (!result.Succeeded || result.Principal == null)
            {
                TempData["Toast"] =
                    _localizer["GoogleLoginFailed"].Value;

                return RedirectToAction(nameof(Login));
            }

            string? email = result.Principal
                .FindFirstValue(ClaimTypes.Email);

            string? hoTen = result.Principal
                .FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Toast"] =
                    _localizer["GoogleEmailUnavailable"].Value;

                return RedirectToAction(nameof(Login));
            }

            email = email.Trim().ToLower();

            // =====================================================
            // KIỂM TRA EMAIL ĐÃ CÓ TRONG DATABASE CHƯA
            // =====================================================

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(
                    x => x.Email.ToLower() == email
                );

            // =====================================================
            // CHƯA CÓ -> TỰ TẠO TÀI KHOẢN
            // =====================================================

            if (khachHang == null)
            {
                string randomPassword =
                    Convert.ToBase64String(
                        RandomNumberGenerator.GetBytes(32)
                    );

                khachHang = new KhachHang
                {
                    HoTen = string.IsNullOrWhiteSpace(hoTen)
                        ? email.Split('@')[0]
                        : hoTen,

                    Email = email,

                    DienThoai = "",

                    DiaChi = null,

                    MatKhau = PasswordHelper.Hash(
                        randomPassword
                    ),

                    NgayTao = DateTime.Now
                };

                _context.KhachHangs.Add(khachHang);

                await _context.SaveChangesAsync();
            }

            // =====================================================
            // LƯU SESSION GIỐNG LOGIN THƯỜNG
            // =====================================================

            HttpContext.Session.SetInt32(
                "MaKH",
                khachHang.MaKh
            );

            HttpContext.Session.SetString(
                "HoTen",
                khachHang.HoTen
            );

            HttpContext.Session.SetString(
                "Email",
                khachHang.Email
            );

            HttpContext.Session.SetString(
                "VaiTro",
                "KhachHang"
            );

            // =====================================================
            // LƯU EMAIL
            // =====================================================

            Response.Cookies.Append(
                "LastEmail",
                khachHang.Email,
                new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(30),
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax
                }
            );

            TempData["Success"] =
                _localizer[
                    "GoogleWelcomeSuccess",
                    khachHang.HoTen
                ].Value;

            return RedirectToAction(
                "Index",
                "Home"
            );
        }

        // =========================================================
        // FORGOT PASSWORD - GET
        // =========================================================

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }


        // =========================================================
        // FORGOT PASSWORD - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email = model.Email
                .Trim()
                .ToLower();

            var khachHang = await _context.KhachHangs
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Email != null &&
                         x.Email.ToLower() == email
                );

            if (khachHang == null)
            {
                ModelState.AddModelError(
                    "Email",
                    _localizer["AccountEmailNotFound"].Value
                );

                return View(model);
            }

            // Tạo OTP 6 số
            string otp = Random.Shared
                .Next(100000, 999999)
                .ToString();

            // Lưu OTP vào Session
            HttpContext.Session.SetString(
                "ResetEmail",
                email
            );

            HttpContext.Session.SetString(
                "ResetOtp",
                otp
            );

            HttpContext.Session.SetString(
                "ResetOtpExpire",
                DateTime.Now
                    .AddMinutes(5)
                    .ToString("O")
            );

            try
            {
                await _emailService.SendOtpAsync(
                    email,
                    otp
                );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    _localizer[
                        "EmailSendError",
                        ex.Message
                    ].Value
                );

                return View(model);
            }

            TempData["Success"] =
                _localizer["OtpSentSuccess"].Value;

            return RedirectToAction(
                nameof(VerifyOtp)
            );
        }


        // =========================================================
        // VERIFY OTP - GET
        // =========================================================

        [HttpGet]
        public IActionResult VerifyOtp()
        {
            if (HttpContext.Session.GetString("ResetEmail") == null)
            {
                return RedirectToAction(
                    nameof(ForgotPassword)
                );
            }

            return View(new VerifyOtpViewModel());
        }


        // =========================================================
        // VERIFY OTP - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyOtp(
            VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? savedOtp =
                HttpContext.Session.GetString(
                    "ResetOtp"
                );

            string? expireString =
                HttpContext.Session.GetString(
                    "ResetOtpExpire"
                );

            if (savedOtp == null ||
                expireString == null)
            {
                ModelState.AddModelError(
                    "",
                    _localizer["OtpNotExist"].Value
                );

                return View(model);
            }

            if (!DateTime.TryParse(
                    expireString,
                    out DateTime expireTime))
            {
                ModelState.AddModelError(
                    "",
                    _localizer["OtpInvalid"].Value
                );

                return View(model);
            }

            if (DateTime.Now > expireTime)
            {
                ModelState.AddModelError(
                    "",
                    _localizer["OtpExpired"].Value
                );

                return View(model);
            }

            if (model.Otp.Trim() != savedOtp)
            {
                ModelState.AddModelError(
                    "Otp",
                    _localizer["OtpIncorrect"].Value
                );

                return View(model);
            }

            HttpContext.Session.SetString(
                "ResetOtpVerified",
                "true"
            );

            return RedirectToAction(
                nameof(ResetPassword)
            );
        }


        // =========================================================
        // RESET PASSWORD - GET
        // =========================================================

        [HttpGet]
        public IActionResult ResetPassword()
        {
            string? verified =
                HttpContext.Session.GetString(
                    "ResetOtpVerified"
                );

            if (verified != "true")
            {
                return RedirectToAction(
                    nameof(ForgotPassword)
                );
            }

            return View(new ResetPasswordViewModel());
        }


        // =========================================================
        // RESET PASSWORD - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? email =
                HttpContext.Session.GetString(
                    "ResetEmail"
                );

            string? verified =
                HttpContext.Session.GetString(
                    "ResetOtpVerified"
                );

            if (email == null ||
                verified != "true")
            {
                return RedirectToAction(
                    nameof(ForgotPassword)
                );
            }

            var khachHang =
                await _context.KhachHangs
                    .FirstOrDefaultAsync(
                        x => x.Email != null &&
                             x.Email.ToLower() ==
                             email.ToLower()
                    );

            if (khachHang == null)
            {
                return RedirectToAction(
                    nameof(ForgotPassword)
                );
            }

            khachHang.MatKhau =
                PasswordHelper.Hash(
                    model.MatKhauMoi
                );

            await _context.SaveChangesAsync();

            // Xóa dữ liệu reset
            HttpContext.Session.Remove("ResetEmail");
            HttpContext.Session.Remove("ResetOtp");
            HttpContext.Session.Remove("ResetOtpExpire");
            HttpContext.Session.Remove("ResetOtpVerified");

            TempData["Success"] =
                _localizer["PasswordChangedSuccess"].Value;

            return RedirectToAction(
                nameof(Login)
            );
        }

        // =========================================================
        // PROFILE - GET
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var maKh =
                HttpContext.Session.GetInt32("MaKH");

            if (maKh == null)
            {
                return RedirectToAction(
                    nameof(Login)
                );
            }

            var khachHang =
                await _context.KhachHangs
                    .FirstOrDefaultAsync(
                        x => x.MaKh == maKh.Value
                    );

            if (khachHang == null)
            {
                HttpContext.Session.Clear();

                return RedirectToAction(
                    nameof(Login)
                );
            }

            return View(khachHang);
        }


        // =========================================================
        // PROFILE - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(
            KhachHang model)
        {
            var maKh =
                HttpContext.Session.GetInt32("MaKH");

            if (maKh == null)
            {
                return RedirectToAction(
                    nameof(Login)
                );
            }

            var khachHang =
                await _context.KhachHangs
                    .FirstOrDefaultAsync(
                        x => x.MaKh == maKh.Value
                    );

            if (khachHang == null)
            {
                return RedirectToAction(
                    nameof(Login)
                );
            }


            // =============================================
            // VALIDATE
            // =============================================

            if (string.IsNullOrWhiteSpace(model.HoTen))
            {
                ModelState.AddModelError(
                    "HoTen",
                    _localizer["FullNameRequired"].Value
                );
            }

            if (string.IsNullOrWhiteSpace(model.DienThoai))
            {
                ModelState.AddModelError(
                    "DienThoai",
                    _localizer["PhoneRequired"].Value
                );
            }

            if (!ModelState.IsValid)
            {
                // Giữ những thông tin không cho sửa
                model.MaKh =
                    khachHang.MaKh;

                model.Email =
                    khachHang.Email;

                model.NgayTao =
                    khachHang.NgayTao;

                model.MatKhau =
                    khachHang.MatKhau;

                return View(model);
            }


            // =============================================
            // UPDATE
            // =============================================

            khachHang.HoTen =
                model.HoTen.Trim();

            khachHang.DienThoai =
                model.DienThoai?.Trim();

            khachHang.DiaChi =
                string.IsNullOrWhiteSpace(model.DiaChi)
                    ? null
                    : model.DiaChi.Trim();


            await _context.SaveChangesAsync();


            // Cập nhật tên trên Header ngay lập tức
            HttpContext.Session.SetString(
                "HoTen",
                khachHang.HoTen
            );


            TempData["Success"] =
                _localizer["ProfileUpdateSuccess"].Value;


            return RedirectToAction(
                nameof(Profile)
            );
        }

        // =========================================================
        // ORDER HISTORY
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> OrderHistory()
        {
            // =====================================================
            // LOGIN
            // =====================================================

            var maKh =
                HttpContext.Session.GetInt32("MaKH");

            if (maKh == null)
            {
                return RedirectToAction(
                    nameof(Login)
                );
            }


            // =====================================================
            // LOAD ĐƠN HÀNG + CHI TIẾT + SẢN PHẨM + COMBO
            // =====================================================

            var orders =
                await _context.HoaDons
                    .Where(x => x.MaKh == maKh.Value)

                    .Include(x => x.ChiTietHoaDons)
                        .ThenInclude(x => x.MaSpNavigation)

                    .Include(x => x.ChiTietHoaDons)
                        .ThenInclude(x => x.MaComboNavigation)

                    .OrderByDescending(x => x.NgayDat)

                    .ToListAsync();


            // =====================================================
            // DANH SÁCH SẢN PHẨM ĐÃ ĐÁNH GIÁ
            // =====================================================

            var reviewedItems =
                await _context.DanhGiaSanPhams
                    .Where(x => x.MaKh == maKh.Value)

                    .Select(x => new
                    {
                        x.MaHd,
                        x.MaSp
                    })

                    .ToListAsync();


            var reviewedKeys =
                reviewedItems
                    .Select(x =>
                        $"{x.MaHd}_{x.MaSp}"
                    )
                    .ToList();


            ViewBag.ReviewedKeys =
                reviewedKeys;


            // =====================================================
            // DANH SÁCH COMBO ĐÃ ĐÁNH GIÁ
            // =====================================================

            var reviewedComboItems =
                await _context.DanhGiaCombos
                    .Where(
                        x => x.MaKh == maKh.Value
                    )

                    .Select(
                        x => new
                        {
                            x.MaHd,
                            x.MaCombo
                        }
                    )

                    .ToListAsync();


            var reviewedComboKeys =
                reviewedComboItems
                    .Select(
                        x =>
                            $"{x.MaHd}_{x.MaCombo}"
                    )
                    .ToList();


            ViewBag.ReviewedComboKeys =
                reviewedComboKeys;


            return View(
                orders
            );
        }


        // =========================================================
        // HỦY ĐƠN / YÊU CẦU HỦY ĐƠN
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(
            int maHd,
            string lyDoHuy,
            string? ghiChuHuy)
        {
            // =====================================================
            // KIỂM TRA ĐĂNG NHẬP
            // =====================================================

            var maKh =
                HttpContext.Session.GetInt32("MaKH");


            if (maKh == null)
            {
                return RedirectToAction(
                    nameof(Login)
                );
            }


            // =====================================================
            // KIỂM TRA LÝ DO HỦY
            // =====================================================

            lyDoHuy =
                lyDoHuy?.Trim()
                ?? string.Empty;


            ghiChuHuy =
                ghiChuHuy?.Trim();


            if (string.IsNullOrWhiteSpace(lyDoHuy))
            {
                TempData["Toast"] =
                    "Vui lòng chọn lý do hủy đơn.";

                return RedirectToAction(
                    nameof(OrderHistory)
                );
            }


            if (
                lyDoHuy.Equals(
                    "Khác",
                    StringComparison.OrdinalIgnoreCase
                )
                &&
                string.IsNullOrWhiteSpace(ghiChuHuy)
            )
            {
                TempData["Toast"] =
                    "Vui lòng nhập lý do hủy đơn.";

                return RedirectToAction(
                    nameof(OrderHistory)
                );
            }


            if (lyDoHuy.Length > 300)
            {
                TempData["Toast"] =
                    "Lý do hủy đơn quá dài.";

                return RedirectToAction(
                    nameof(OrderHistory)
                );
            }


            if (
                ghiChuHuy != null
                &&
                ghiChuHuy.Length > 1000
            )
            {
                TempData["Toast"] =
                    "Ghi chú hủy đơn tối đa 1000 ký tự.";

                return RedirectToAction(
                    nameof(OrderHistory)
                );
            }


            // =====================================================
            // LẤY ĐƠN HÀNG
            // CHỈ CHO KHÁCH HỦY ĐƠN CỦA CHÍNH MÌNH
            // =====================================================

            var order =
                await _context.HoaDons
                    .FirstOrDefaultAsync(
                        x =>
                            x.MaHd == maHd
                            &&
                            x.MaKh == maKh.Value
                    );


            if (order == null)
            {
                TempData["Toast"] =
                    "Không tìm thấy đơn hàng.";

                return RedirectToAction(
                    nameof(OrderHistory)
                );
            }


            string currentStatus =
                order.TrangThai?.Trim()
                ?? string.Empty;


            // =====================================================
            // KHÔNG CHO HỦY LẠI / HỦY ĐƠN ĐÃ KẾT THÚC
            // =====================================================

            var blockedStatuses =
                new[]
                {
                    "Hoàn thành",
                    "Đã hủy",
                    "Đã hủy - Chờ hoàn tiền",
                    "Đã hủy - Đã hoàn tiền",
                    "Yêu cầu hủy",
                    "Yêu cầu hủy khi đang giao"
                };


            if (
                blockedStatuses.Any(
                    x => x.Equals(
                        currentStatus,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
            )
            {
                TempData["Toast"] =
                    "Đơn hàng này hiện không thể gửi thêm yêu cầu hủy.";

                return RedirectToAction(
                    nameof(OrderHistory)
                );
            }


            // =====================================================
            // LƯU THÔNG TIN HỦY
            // =====================================================

            order.LyDoHuy =
                lyDoHuy;

            order.GhiChuHuy =
                ghiChuHuy;

            order.NgayYeuCauHuy =
                DateTime.Now;


            // =====================================================
            // 1. CHỜ XÁC NHẬN
            // COD / CHƯA THANH TOÁN
            // => HỦY NGAY
            // =====================================================

            if (
                currentStatus.Equals(
                    "Chờ xác nhận",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                order.TrangThai =
                    "Đã hủy";

                order.NgayXuLyHuy =
                    DateTime.Now;

                order.TrangThaiHoanTien =
                    "Không cần hoàn tiền";

                order.SoTienHoan =
                    0;

                order.NgayHoanTien =
                    null;


                await _context.SaveChangesAsync();


                TempData["Toast"] =
                    $"Đã hủy đơn #HD{order.MaHd:D5} thành công.";

                return RedirectToAction(
                    nameof(OrderHistory)
                );
            }


            // =====================================================
            // 2. ĐÃ THANH TOÁN
            // => GỬI YÊU CẦU HỦY
            // => ĐÁNH DẤU CÓ KHẢ NĂNG PHẢI HOÀN TIỀN
            // =====================================================

            if (
                currentStatus.Equals(
                    "Đã thanh toán",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                order.TrangThai =
                    "Yêu cầu hủy";

                order.TrangThaiHoanTien =
                    "Chờ hoàn tiền";

                order.SoTienHoan =
                    order.TongTien ?? 0;

                order.NgayXuLyHuy =
                    null;

                order.NgayHoanTien =
                    null;


                await _context.SaveChangesAsync();


                TempData["Toast"] =
                    "Đã gửi yêu cầu hủy. Đơn đã thanh toán sẽ được xử lý hoàn tiền sau khi yêu cầu được duyệt.";

                return RedirectToAction(
                    nameof(OrderHistory)
                );
            }


            // =====================================================
            // 3. ĐANG XỬ LÝ
            // => KHÔNG HỦY NGAY
            // => CHỜ ADMIN / NHÂN VIÊN DUYỆT
            // =====================================================

            if (
                currentStatus.Equals(
                    "Đang xử lý",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                order.TrangThai =
                    "Yêu cầu hủy";

                order.NgayXuLyHuy =
                    null;


                await _context.SaveChangesAsync();


                TempData["Toast"] =
                    "Đã gửi yêu cầu hủy đơn. Cửa hàng sẽ kiểm tra và phản hồi.";

                return RedirectToAction(
                    nameof(OrderHistory)
                );
            }


            // =====================================================
            // 4. ĐANG GIAO HÀNG
            // => YÊU CẦU HỦY KHI ĐANG GIAO
            // => CẦN ADMIN / NHÂN VIÊN + SHIPPER XỬ LÝ
            // =====================================================

            if (
                currentStatus.Equals(
                    "Đang giao hàng",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                order.TrangThai =
                    "Yêu cầu hủy khi đang giao";

                order.NgayXuLyHuy =
                    null;


                await _context.SaveChangesAsync();


                TempData["Toast"] =
                    "Đã gửi yêu cầu hủy khi đơn đang giao. Cửa hàng sẽ kiểm tra với nhân viên giao hàng trước khi xác nhận.";

                return RedirectToAction(
                    nameof(OrderHistory)
                );
            }


            // =====================================================
            // TRẠNG THÁI KHÁC
            // =====================================================

            TempData["Toast"] =
                $"Đơn hàng ở trạng thái \"{currentStatus}\" hiện chưa hỗ trợ hủy.";


            return RedirectToAction(
                nameof(OrderHistory)
            );
        }


        // =========================================================
        // ACCESS DENIED
        // =========================================================

        [HttpGet]
        public IActionResult AccessDenied()
        {
            TempData["Toast"] =
                "Bạn không có quyền truy cập chức năng này.";

            return RedirectToAction(
                "Index",
                "Home"
            );
        }

        // =========================================================
        // LOGOUT
        // ADMIN + NHÂN VIÊN + GIAO HÀNG + KHÁCH HÀNG
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            HttpContext.Session.Clear();

            TempData["Toast"] =
                _localizer["LogoutSuccess"].Value;

            return RedirectToAction(
                "Index",
                "Home"
            );
        }
    }
}