using CoffeeShopManagement.Data;
using CoffeeShopManagement.Extensions;
using CoffeeShopManagement.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;


namespace CoffeeShopManagement.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly CoffeeShopDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<SharedResource> _localizer;


        public CheckoutController(
            CoffeeShopDbContext context,
            IConfiguration configuration,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _configuration = configuration;
            _localizer = localizer;
        }


        // =====================================================
        // SESSION KEYS
        // =====================================================

        private const string CARTKEY =
            "CART";


        private const string VOUCHER_CODE =
            "VoucherCode";


        private const string VOUCHER_DISCOUNT =
            "VoucherDiscount";


        private const string VOUCHER_MIN_ORDER =
            "VoucherMinOrder";


        // =====================================================
        // VNPAY
        // =====================================================

        private const string VNPAY_URL =
            "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";


        // =====================================================
        // WEBSITE LOCAL
        //
        // VNPay trả callback về NGROK.
        // Sau khi kiểm tra callback xong,
        // chuyển browser về localhost để lấy lại đúng
        // Session / Cookie đăng nhập khách hàng.
        // =====================================================

        private const string LOCAL_SITE_URL =
            "https://localhost:7231";


        // =====================================================
        // VNPAY CONFIG FROM USER SECRETS / CONFIG
        // =====================================================

        private string VnPayTmnCode =>
            _configuration[
                "VNPay:TmnCode"
            ]
            ?? throw new Exception(
                _localizer[
                    "VnPayTmnCodeMissing"
                ].Value
            );


        private string VnPayHashSecret =>
            _configuration[
                "VNPay:HashSecret"
            ]
            ?? throw new Exception(
                _localizer[
                    "VnPayHashSecretMissing"
                ].Value
            );


        private string VnPayReturnUrl =>
            _configuration[
                "VNPay:ReturnUrl"
            ]
            ?? throw new Exception(
                _localizer[
                    "VnPayReturnUrlMissing"
                ].Value
            );


        // =====================================================
        // CART
        // =====================================================

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
        }


        // =====================================================
        // VALID VOUCHERS
        // =====================================================

        private readonly Dictionary<
            string,
            (int Discount, int MinOrder)>
            _validVouchers =
                new()
                {
                    ["ROSALIE15"] =
                        (15000, 99000),

                    ["ROSALIE30"] =
                        (30000, 199000),

                    ["ROSALIE50"] =
                        (50000, 299000)
                };


        // =====================================================
        // CLEAR VOUCHER
        // =====================================================

        private void ClearVoucher()
        {
            HttpContext.Session.Remove(
                VOUCHER_CODE
            );


            HttpContext.Session.Remove(
                VOUCHER_DISCOUNT
            );


            HttpContext.Session.Remove(
                VOUCHER_MIN_ORDER
            );
        }


        // =====================================================
        // CALCULATE TOTAL
        // =====================================================

        private (
            decimal SubTotal,
            int Discount,
            decimal Shipping,
            decimal Total
        )
        CalculateTotals(
            string? districtName = null)
        {
            decimal subTotal =
                Cart.Sum(
                    x => x.ThanhTien
                );


            // =================================================
            // GIỎ HÀNG TRỐNG
            // =================================================

            if (subTotal <= 0)
            {
                ClearVoucher();


                return (
                    0,
                    0,
                    0,
                    0
                );
            }


            var maKh =
                HttpContext.Session
                    .GetInt32(
                        "MaKH"
                    );


            int discount =
                0;


            // =================================================
            // VOUCHER CHỈ DÙNG KHI LOGIN
            // =================================================

            if (maKh != null)
            {
                string? code =
                    HttpContext.Session
                        .GetString(
                            VOUCHER_CODE
                        );


                if (!string.IsNullOrWhiteSpace(code)
                    &&
                    _validVouchers.TryGetValue(
                        code,
                        out var voucher
                    ))
                {
                    if (subTotal >=
                        voucher.MinOrder)
                    {
                        discount =
                            voucher.Discount;
                    }
                    else
                    {
                        ClearVoucher();
                    }
                }
            }
            else
            {
                ClearVoucher();
            }


            // =================================================
            // KHÔNG GIẢM QUÁ TIỀN HÀNG
            // =================================================

            if (discount > subTotal)
            {
                discount =
                    Convert.ToInt32(
                        subTotal
                    );
            }


            decimal afterDiscount =
                subTotal
                - discount;


            // =================================================
            // SHIPPING
            // =================================================

            decimal shipping =
                CalculateShippingFee(
                    afterDiscount,
                    districtName
                );


            decimal total =
                afterDiscount
                + shipping;


            return (
                subTotal,
                discount,
                shipping,
                total
            );
        }


        // =====================================================
        // CALCULATE SHIPPING BY DISTRICT
        // =====================================================

        private decimal CalculateShippingFee(
            decimal afterDiscount,
            string? districtName)
        {
            // =================================================
            // SAU GIẢM >= 150.000đ
            // MIỄN PHÍ SHIP
            // =================================================

            if (afterDiscount >=
                150000)
            {
                return 0;
            }


            // =================================================
            // CHƯA CHỌN QUẬN/HUYỆN
            // =================================================

            if (string.IsNullOrWhiteSpace(
                    districtName))
            {
                return 0;
            }


            string district =
                RemoveVietnameseDiacritics(
                    districtName
                )
                .ToLowerInvariant()
                .Trim();


            // =================================================
            // CHUẨN HÓA TÊN QUẬN
            // =================================================

            district =
                district

                    .Replace(
                        "district ",
                        "quan "
                    )

                    .Replace(
                        "q. ",
                        "quan "
                    )

                    .Replace(
                        "q.",
                        "quan "
                    )

                    .Replace(
                        "tp. ",
                        ""
                    )

                    .Replace(
                        "tp.",
                        ""
                    );


            // =================================================
            // BÌNH THẠNH
            // =================================================

            if (
                district == "binh thanh"
                ||
                district ==
                "quan binh thanh"
            )
            {
                return 15000;
            }


            // =================================================
            // QUẬN 1
            // =================================================

            if (district ==
                "quan 1")
            {
                return 20000;
            }


            // =================================================
            // QUẬN 3
            // =================================================

            if (district ==
                "quan 3")
            {
                return 20000;
            }


            // =================================================
            // QUẬN 7
            // =================================================

            if (district ==
                "quan 7")
            {
                return 25000;
            }


            // =================================================
            // THỦ ĐỨC
            // =================================================

            if (
                district ==
                "thu duc"
                ||
                district ==
                "thanh pho thu duc"
                ||
                district ==
                "quan thu duc"
            )
            {
                return 30000;
            }


            // =================================================
            // KHU VỰC KHÁC
            // =================================================

            return 35000;
        }


        // =====================================================
        // REMOVE VIETNAMESE DIACRITICS
        // =====================================================

        private string RemoveVietnameseDiacritics(
            string text)
        {
            string normalized =
                text.Normalize(
                    NormalizationForm.FormD
                );


            var builder =
                new StringBuilder();


            foreach (
                char c in normalized)
            {
                UnicodeCategory category =
                    CharUnicodeInfo
                        .GetUnicodeCategory(c);


                if (category !=
                    UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c);
                }
            }


            return builder
                .ToString()

                .Normalize(
                    NormalizationForm.FormC
                )

                .Replace(
                    "đ",
                    "d"
                )

                .Replace(
                    "Đ",
                    "D"
                );
        }


        // =====================================================
        // PREPARE VIEWBAG
        // =====================================================

        private void PrepareCheckoutView(
            string? districtName = null)
        {
            var totals =
                CalculateTotals(
                    districtName
                );


            ViewBag.SubTotal =
                totals.SubTotal;


            ViewBag.Discount =
                totals.Discount;


            ViewBag.Shipping =
                totals.Shipping;


            ViewBag.Total =
                totals.Total;


            ViewBag.VoucherCode =
                HttpContext.Session
                    .GetString(
                        VOUCHER_CODE
                    );
        }


        // =====================================================
        // APPLY VOUCHER
        // =====================================================

        [HttpPost]
        public IActionResult ApplyVoucher(
            string code,
            int discount,
            int minOrder)
        {
            var maKh =
                HttpContext.Session
                    .GetInt32(
                        "MaKH"
                    );


            // =================================================
            // CHƯA LOGIN
            // =================================================

            if (maKh == null)
            {
                return Json(
                    new
                    {
                        success = false,

                        message =
                            _localizer[
                                "LoginRequiredForVoucher"
                            ].Value
                    }
                );
            }


            // =================================================
            // CODE KHÔNG TỒN TẠI
            // =================================================

            if (
                string.IsNullOrWhiteSpace(code)
                ||
                !_validVouchers.TryGetValue(
                    code,
                    out var voucher
                )
            )
            {
                ClearVoucher();


                return Json(
                    new
                    {
                        success = false,

                        message =
                            _localizer[
                                "VoucherInvalid"
                            ].Value
                    }
                );
            }


            // =================================================
            // KHÔNG TIN DỮ LIỆU DISCOUNT TỪ JS
            // =================================================

            if (
                voucher.Discount !=
                discount
                ||
                voucher.MinOrder !=
                minOrder
            )
            {
                ClearVoucher();


                return Json(
                    new
                    {
                        success = false,

                        message =
                            _localizer[
                                "VoucherInfoInvalid"
                            ].Value
                    }
                );
            }


            decimal subTotal =
                Cart.Sum(
                    x => x.ThanhTien
                );


            // =================================================
            // KHÔNG ĐỦ ĐƠN TỐI THIỂU
            // =================================================

            if (subTotal <
                voucher.MinOrder)
            {
                ClearVoucher();


                return Json(
                    new
                    {
                        success = false,

                        message =
                            _localizer[
                                "VoucherMinimumOrderServer",
                                code,
                                $"{voucher.MinOrder:N0}đ"
                            ].Value
                    }
                );
            }


            // =================================================
            // LƯU SESSION
            // =================================================

            HttpContext.Session.SetString(
                VOUCHER_CODE,
                code
            );


            HttpContext.Session.SetInt32(
                VOUCHER_DISCOUNT,
                voucher.Discount
            );


            HttpContext.Session.SetInt32(
                VOUCHER_MIN_ORDER,
                voucher.MinOrder
            );


            return Json(
                new
                {
                    success = true,

                    discount =
                        voucher.Discount
                }
            );
        }


        // =====================================================
        // REMOVE VOUCHER
        // =====================================================

        [HttpPost]
        public IActionResult RemoveVoucher()
        {
            ClearVoucher();


            return Json(
                new
                {
                    success = true
                }
            );
        }


        // =====================================================
        // GET CHECKOUT
        // =====================================================

        [HttpGet]
        public IActionResult Index()
        {
            // =================================================
            // GIỎ HÀNG TRỐNG
            // =================================================

            if (!Cart.Any())
            {
                return RedirectToAction(
                    "Index",
                    "Cart"
                );
            }


            // =================================================
            // PHẢI ĐĂNG NHẬP
            // =================================================

            var maKh =
                HttpContext.Session
                    .GetInt32(
                        "MaKH"
                    );


            if (maKh == null)
            {
                TempData["Toast"] =
                    _localizer[
                        "LoginRequiredForCheckout"
                    ].Value;


                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }


            // =================================================
            // KHÁCH HÀNG HIỆN TẠI
            // =================================================

            var khachHang =
                _context.KhachHangs
                    .FirstOrDefault(
                        x =>
                            x.MaKh ==
                            maKh.Value
                    );


            if (khachHang == null)
            {
                HttpContext.Session.Clear();


                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }


            // =================================================
            // AUTO FILL
            // =================================================

            var model =
                new CheckoutViewModel
                {
                    HoTen =
                        khachHang.HoTen,

                    DienThoai =
                        khachHang.DienThoai
                        ?? "",

                    DiaChi =
                        khachHang.DiaChi
                        ?? ""
                };


            PrepareCheckoutView();


            return View(
                model
            );
        }


        // =====================================================
        // POST CHECKOUT
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(
            CheckoutViewModel model)
        {
            // =================================================
            // CART
            // =================================================

            if (!Cart.Any())
            {
                return RedirectToAction(
                    "Index",
                    "Cart"
                );
            }


            // =================================================
            // LOGIN
            // =================================================

            var maKh =
                HttpContext.Session
                    .GetInt32(
                        "MaKH"
                    );


            if (maKh == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }


            // =================================================
            // DELIVERY AREA
            // =================================================

            string districtName =
                Request.Form[
                    "DistrictName"
                ]
                .ToString()
                .Trim();


            if (string.IsNullOrWhiteSpace(
                    districtName))
            {
                ModelState.AddModelError(
                    "",
                    "Vui lòng chọn Quận/Huyện để tính phí giao hàng."
                );
            }


            // =================================================
            // VALIDATION
            // =================================================

            if (!ModelState.IsValid)
            {
                PrepareCheckoutView(
                    districtName
                );


                return View(
                    model
                );
            }


            // =================================================
            // PAYMENT METHOD
            // =================================================

            string paymentMethod =
                Request.Form[
                    "PaymentMethod"
                ].ToString();


            if (string.IsNullOrWhiteSpace(
                    paymentMethod))
            {
                paymentMethod =
                    "COD";
            }


            if (
                paymentMethod != "COD"
                &&
                paymentMethod != "VNPAY"
            )
            {
                paymentMethod =
                    "COD";
            }


            // =================================================
            // USER
            // =================================================

            var khachHang =
                _context.KhachHangs
                    .FirstOrDefault(
                        x =>
                            x.MaKh ==
                            maKh.Value
                    );


            if (khachHang == null)
            {
                return BadRequest(
                    _localizer[
                        "CustomerNotFound"
                    ].Value
                );
            }


            // =================================================
            // UPDATE USER INFO
            // =================================================

            khachHang.HoTen =
                model.HoTen;


            khachHang.DienThoai =
                model.DienThoai;


            khachHang.DiaChi =
                model.DiaChi;


            _context.SaveChanges();


            // =================================================
            // TOTAL SERVER SIDE
            // =================================================

            var totals =
                CalculateTotals(
                    districtName
                );


            if (totals.Total <= 0)
            {
                return RedirectToAction(
                    "Index",
                    "Cart"
                );
            }


            // =================================================
            // CREATE ORDER
            // =================================================

            var hoaDon =
                new HoaDon
                {
                    MaKh =
                        khachHang.MaKh,


                    NgayDat =
                        DateTime.Now,


                    // =========================================
                    // TIỀN
                    // =========================================

                    TamTinh =
                        totals.SubTotal,


                    GiamGia =
                        totals.Discount,


                    PhiGiaoHang =
                        totals.Shipping,


                    TongTien =
                        totals.Total,


                    // =========================================
                    // SNAPSHOT THÔNG TIN NGƯỜI NHẬN
                    // =========================================

                    HoTenNguoiNhan =
                        model.HoTen?
                            .Trim(),


                    DienThoaiNguoiNhan =
                        model.DienThoai?
                            .Trim(),


                    DiaChiGiaoHang =
                        model.DiaChi?
                            .Trim(),


                    QuanHuyenGiaoHang =
                        districtName,


                    // =========================================
                    // THANH TOÁN
                    // =========================================

                    PhuongThucThanhToan =
                        paymentMethod,


                    // =========================================
                    // TRẠNG THÁI
                    // =========================================

                    TrangThai =
                        paymentMethod ==
                        "VNPAY"
                            ? "Chờ thanh toán"
                            : "Chờ xác nhận"
                };


            // =================================================
            // LƯU HÓA ĐƠN
            // SQL TẠO MaHd
            // =================================================

            _context.HoaDons.Add(
                hoaDon
            );


            _context.SaveChanges();


            // =================================================
            // ORDER DETAILS
            // =================================================

            foreach (var item in Cart)
            {
                // =============================================
                // COMBO
                // =============================================

                if (item.IsCombo)
                {
                    if (item.MaCombo == null)
                    {
                        continue;
                    }


                    var combo =
                        _context.Combos
                            .FirstOrDefault(
                                x =>
                                    x.MaCombo ==
                                    item.MaCombo.Value
                            );


                    if (combo == null)
                    {
                        continue;
                    }


                    _context
                        .ChiTietHoaDons
                        .Add(
                            new ChiTietHoaDon
                            {
                                MaHd =
                                    hoaDon.MaHd,


                                MaSp =
                                    null,


                                MaCombo =
                                    combo.MaCombo,


                                SoLuong =
                                    item.SoLuong,


                                DonGia =
                                    item.DonGia
                            }
                        );


                    continue;
                }


                // =============================================
                // SẢN PHẨM THƯỜNG
                // =============================================

                var sanPham =
                    _context.SanPhams
                        .FirstOrDefault(
                            sp =>
                                sp.MaSp ==
                                item.MaSP
                        );


                if (sanPham == null)
                {
                    continue;
                }


                decimal donGiaThucTe =
                    item.DonGia
                    + item.GiaSize
                    + item.GiaTopping;


                _context
                    .ChiTietHoaDons
                    .Add(
                        new ChiTietHoaDon
                        {
                            MaHd =
                                hoaDon.MaHd,


                            MaSp =
                                sanPham.MaSp,


                            MaCombo =
                                null,


                            SoLuong =
                                item.SoLuong,


                            DonGia =
                                donGiaThucTe
                        }
                    );
            }


            _context.SaveChanges();


            // =================================================
            // VNPAY
            // =================================================

            if (paymentMethod ==
                "VNPAY")
            {
                string paymentUrl =
                    CreateVnPayPaymentUrl(
                        hoaDon.MaHd,
                        hoaDon.TongTien
                        ?? 0
                    );


                // =============================================
                // KHÔNG XÓA CART Ở ĐÂY.
                // KHÁCH CHƯA THANH TOÁN XONG.
                // =============================================

                return Redirect(
                    paymentUrl
                );
            }


            // =================================================
            // COD
            // =================================================

            HttpContext.Session.Remove(
                CARTKEY
            );


            ClearVoucher();


            return RedirectToAction(
                nameof(Success),
                new
                {
                    id =
                        hoaDon.MaHd
                }
            );
        }


        // =====================================================
        // CREATE VNPAY PAYMENT URL
        // =====================================================

        private string CreateVnPayPaymentUrl(
            int hoaDonId,
            decimal amount)
        {
            if (amount <= 0)
            {
                throw new Exception(
                    _localizer[
                        "InvalidPaymentAmount"
                    ].Value
                );
            }


            // =================================================
            // TIME
            // =================================================

            var now =
                DateTime.Now;


            var createDate =
                now.ToString(
                    "yyyyMMddHHmmss"
                );


            // =================================================
            // HẾT HẠN SAU 15 PHÚT
            // =================================================

            var expireDate =
                now
                    .AddMinutes(15)
                    .ToString(
                        "yyyyMMddHHmmss"
                    );


            // =================================================
            // TRANSACTION REFERENCE
            // =================================================

            string txnRef =
                hoaDonId
                    .ToString();


            // =================================================
            // ORDER INFO
            // =================================================

            string orderInfo =
                _localizer[
                    "VnPayOrderInfo",
                    hoaDonId
                ].Value;


            // =================================================
            // CLIENT IP
            // =================================================

            string ipAddress =
                GetClientIpAddress();


            // =================================================
            // PARAMETERS
            // =================================================

            var vnpParams =
                new SortedDictionary<
                    string,
                    string>
                {
                    ["vnp_Amount"] =
                        ((long)(
                            amount * 100
                        ))
                        .ToString(),


                    ["vnp_Command"] =
                        "pay",


                    ["vnp_CreateDate"] =
                        createDate,


                    ["vnp_CurrCode"] =
                        "VND",


                    ["vnp_ExpireDate"] =
                        expireDate,


                    ["vnp_IpAddr"] =
                        ipAddress,


                    ["vnp_Locale"] =
                        CultureInfo
                            .CurrentUICulture
                            .TwoLetterISOLanguageName
                        == "en"
                            ? "en"
                            : "vn",


                    ["vnp_OrderInfo"] =
                        orderInfo,


                    ["vnp_OrderType"] =
                        "other",


                    // =========================================
                    // RETURN URL PHẢI LÀ NGROK
                    // =========================================

                    ["vnp_ReturnUrl"] =
                        VnPayReturnUrl,


                    ["vnp_TmnCode"] =
                        VnPayTmnCode,


                    ["vnp_TxnRef"] =
                        txnRef,


                    ["vnp_Version"] =
                        "2.1.0"
                };


            // =================================================
            // BUILD HASH DATA
            // =================================================

            string hashData =
                BuildVnPayQuery(
                    vnpParams
                );


            // =================================================
            // SIGN HMAC SHA512
            // =================================================

            string secureHash =
                HmacSha512(
                    VnPayHashSecret,
                    hashData
                );


            // =================================================
            // FINAL PAYMENT URL
            // =================================================

            string paymentUrl =
                VNPAY_URL
                + "?"
                + hashData
                + "&vnp_SecureHash="
                + secureHash;


            return paymentUrl;
        }


        // =====================================================
        // BUILD VNPAY QUERY / HASH DATA
        // =====================================================

        private string BuildVnPayQuery(
            SortedDictionary<
                string,
                string> parameters)
        {
            var query =
                new StringBuilder();


            foreach (
                var item in parameters)
            {
                if (string.IsNullOrWhiteSpace(
                        item.Value))
                {
                    continue;
                }


                if (query.Length > 0)
                {
                    query.Append(
                        "&"
                    );
                }


                query.Append(
                    WebUtility.UrlEncode(
                        item.Key
                    )
                );


                query.Append(
                    "="
                );


                query.Append(
                    WebUtility.UrlEncode(
                        item.Value
                    )
                );
            }


            return query.ToString();
        }


        // =====================================================
        // CLIENT IP
        // =====================================================

        private string GetClientIpAddress()
        {
            string ip =
                HttpContext.Connection
                    .RemoteIpAddress?
                    .ToString()
                ?? "127.0.0.1";


            // =================================================
            // LOCALHOST IPV6
            // =================================================

            if (ip == "::1")
            {
                return "127.0.0.1";
            }


            // =================================================
            // IPV4 MAPPED IPV6
            // =================================================

            if (ip.StartsWith(
                    "::ffff:",
                    StringComparison.OrdinalIgnoreCase))
            {
                ip =
                    ip.Replace(
                        "::ffff:",
                        "",
                        StringComparison.OrdinalIgnoreCase
                    );
            }


            return ip;
        }


        // =====================================================
        // HMAC SHA512
        // =====================================================

        private string HmacSha512(
            string key,
            string data)
        {
            byte[] keyBytes =
                Encoding.UTF8
                    .GetBytes(
                        key
                    );


            byte[] dataBytes =
                Encoding.UTF8
                    .GetBytes(
                        data
                    );


            using var hmac =
                new HMACSHA512(
                    keyBytes
                );


            byte[] hash =
                hmac.ComputeHash(
                    dataBytes
                );


            return Convert
                .ToHexString(
                    hash
                )
                .ToLowerInvariant();
        }


        // =====================================================
        // VNPAY RETURN
        //
        // REQUEST NÀY ĐẾN TỪ DOMAIN NGROK
        // =====================================================

        [HttpGet]
        public IActionResult VnPayReturn()
        {
            // =================================================
            // 1. GET ALL VNP PARAMETERS
            // =================================================

            var inputData =
                new SortedDictionary<
                    string,
                    string>();


            foreach (
                var item in Request.Query)
            {
                if (item.Key.StartsWith(
                        "vnp_",
                        StringComparison.OrdinalIgnoreCase))
                {
                    inputData[
                        item.Key
                    ] =
                        item.Value
                            .ToString();
                }
            }


            // =================================================
            // 2. RECEIVED HASH
            // =================================================

            if (!inputData.TryGetValue(
                    "vnp_SecureHash",
                    out string? receivedHash))
            {
                return BadRequest(
                    _localizer[
                        "VnPaySignatureNotFound"
                    ].Value
                );
            }


            // =================================================
            // KHÔNG DÙNG KHI VERIFY
            // =================================================

            inputData.Remove(
                "vnp_SecureHash"
            );


            inputData.Remove(
                "vnp_SecureHashType"
            );


            // =================================================
            // 3. REBUILD HASH DATA
            // =================================================

            string hashData =
                BuildVnPayQuery(
                    inputData
                );


            string calculatedHash =
                HmacSha512(
                    VnPayHashSecret,
                    hashData
                );


            // =================================================
            // 4. VERIFY SIGNATURE
            // =================================================

            if (!string.Equals(
                    calculatedHash,
                    receivedHash,
                    StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(
                    _localizer[
                        "VnPayInvalidSignature"
                    ].Value
                );
            }


            // =================================================
            // 5. GET ORDER ID
            // =================================================

            if (!inputData.TryGetValue(
                    "vnp_TxnRef",
                    out string? txnRef))
            {
                return BadRequest(
                    _localizer[
                        "OrderCodeNotFound"
                    ].Value
                );
            }


            if (!int.TryParse(
                    txnRef,
                    out int hoaDonId))
            {
                return BadRequest(
                    _localizer[
                        "InvalidOrderCode"
                    ].Value
                );
            }


            // =================================================
            // 6. FIND ORDER
            // =================================================

            var hoaDon =
                _context.HoaDons
                    .FirstOrDefault(
                        x =>
                            x.MaHd ==
                            hoaDonId
                    );


            if (hoaDon == null)
            {
                return NotFound(
                    _localizer[
                        "InvoiceNotFound"
                    ].Value
                );
            }


            // =================================================
            // 7. RESPONSE CODE
            // =================================================

            string responseCode =
                inputData.TryGetValue(
                    "vnp_ResponseCode",
                    out var response
                )
                    ? response
                    : "";


            string transactionStatus =
                inputData.TryGetValue(
                    "vnp_TransactionStatus",
                    out var status
                )
                    ? status
                    : "";


            // =================================================
            // 8. PAYMENT SUCCESS
            // =================================================

            if (
                responseCode == "00"
                &&
                transactionStatus == "00"
            )
            {
                // =============================================
                // CẬP NHẬT ĐƠN HÀNG
                // =============================================

                hoaDon.TrangThai =
                    "Đã thanh toán";


                _context.SaveChanges();


                // =============================================
                // QUAN TRỌNG:
                //
                // KHÔNG XÓA CART / VOUCHER Ở REQUEST NÀY.
                //
                // REQUEST HIỆN TẠI ĐANG Ở DOMAIN NGROK:
                //
                // radio-tamer-charging.ngrok-free.dev
                //
                // SESSION CỦA KHÁCH HÀNG NẰM Ở:
                //
                // localhost:7231
                //
                // NÊN PHẢI REDIRECT VỀ LOCALHOST TRƯỚC.
                // =============================================


                string successUrl =
                    $"{LOCAL_SITE_URL}" +
                    $"/Checkout/Success/{hoaDonId}" +
                    "?fromVnPay=1";


                return Redirect(
                    successUrl
                );
            }


            // =================================================
            // 9. PAYMENT FAILED / CANCEL
            // =================================================

            hoaDon.TrangThai =
                "Thanh toán thất bại";


            _context.SaveChanges();


            // =================================================
            // KHÔNG XÓA CART KHI THẤT BẠI
            // ĐỂ KHÁCH CÓ THỂ THANH TOÁN LẠI
            // =================================================

            string failedUrl =
                $"{LOCAL_SITE_URL}" +
                $"/Checkout/PaymentFailed/{hoaDonId}";


            return Redirect(
                failedUrl
            );
        }


        // =====================================================
        // PAYMENT FAILED
        // =====================================================

        public IActionResult PaymentFailed(
            int id)
        {
            var hoaDon =
                _context.HoaDons
                    .FirstOrDefault(
                        x =>
                            x.MaHd ==
                            id
                    );


            if (hoaDon == null)
            {
                return RedirectToAction(
                    "Index",
                    "Home"
                );
            }


            return View(
                hoaDon
            );
        }


        // =====================================================
        // SUCCESS
        // =====================================================

        public IActionResult Success(
            int id)
        {
            // =================================================
            // VNPAY ĐÃ THANH TOÁN THÀNH CÔNG
            //
            // LÚC NÀY URL ĐÃ TRỞ VỀ:
            //
            // https://localhost:7231
            //
            // NÊN SESSION KHÁCH HÀNG ĐÃ TRỞ LẠI.
            // =================================================

            string fromVnPay =
                Request.Query[
                    "fromVnPay"
                ]
                .ToString();


            if (fromVnPay ==
                "1")
            {
                // =============================================
                // XÓA GIỎ ĐÚNG SESSION LOCALHOST
                // =============================================

                HttpContext.Session.Remove(
                    CARTKEY
                );


                // =============================================
                // XÓA VOUCHER
                // =============================================

                ClearVoucher();
            }


            // =================================================
            // HÓA ĐƠN
            // =================================================

            var hoaDon =
                _context.HoaDons
                    .FirstOrDefault(
                        x =>
                            x.MaHd ==
                            id
                    );


            if (hoaDon == null)
            {
                return RedirectToAction(
                    "Index",
                    "Home"
                );
            }


            // =================================================
            // KHÁCH HÀNG
            // =================================================

            var khachHang =
                _context.KhachHangs
                    .FirstOrDefault(
                        x =>
                            x.MaKh ==
                            hoaDon.MaKh
                    );


            if (khachHang == null)
            {
                return RedirectToAction(
                    "Index",
                    "Home"
                );
            }


            // =================================================
            // CHI TIẾT HÓA ĐƠN
            // =================================================

            var chiTietHoaDons =
                _context.ChiTietHoaDons

                    .Where(
                        x =>
                            x.MaHd ==
                            id
                    )

                    .ToList();


            var dsSanPham =
                new List<SuccessItem>();


            foreach (
                var ct in chiTietHoaDons)
            {
                // =============================================
                // COMBO
                // =============================================

                if (ct.MaCombo != null)
                {
                    var combo =
                        _context.Combos
                            .FirstOrDefault(
                                x =>
                                    x.MaCombo ==
                                    ct.MaCombo.Value
                            );


                    if (combo != null)
                    {
                        dsSanPham.Add(
                            new SuccessItem
                            {
                                MaSP =
                                    0,


                                TenSP =
                                    combo.TenCombo,


                                HinhAnh =
                                    combo.HinhAnh,


                                DonGia =
                                    ct.DonGia,


                                SoLuong =
                                    ct.SoLuong
                            }
                        );
                    }


                    continue;
                }


                // =============================================
                // SẢN PHẨM THƯỜNG
                // =============================================

                if (ct.MaSp != null)
                {
                    var sanPham =
                        _context.SanPhams
                            .FirstOrDefault(
                                x =>
                                    x.MaSp ==
                                    ct.MaSp.Value
                            );


                    if (sanPham != null)
                    {
                        dsSanPham.Add(
                            new SuccessItem
                            {
                                MaSP =
                                    sanPham.MaSp,


                                TenSP =
                                    sanPham.TenSp,


                                HinhAnh =
                                    sanPham.HinhAnh,


                                DonGia =
                                    ct.DonGia,


                                SoLuong =
                                    ct.SoLuong
                            }
                        );
                    }
                }
            }


            // =================================================
            // VIEW MODEL
            // =================================================

            var model =
                new SuccessViewModel
                {
                    HoaDon =
                        hoaDon,


                    KhachHang =
                        khachHang,


                    SanPham =
                        dsSanPham
                };


            return View(
                model
            );
        }
    }
}