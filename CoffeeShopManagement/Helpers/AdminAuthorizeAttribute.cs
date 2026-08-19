using CoffeeShopManagement.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeShopManagement.Helpers
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string? _permission;


        // =====================================================
        // KHÔNG YÊU CẦU QUYỀN CỤ THỂ
        // =====================================================

        public AdminAuthorizeAttribute()
        {
        }


        // =====================================================
        // YÊU CẦU QUYỀN CỤ THỂ
        // VD: [AdminAuthorize("ROLE_MANAGE")]
        // =====================================================

        public AdminAuthorizeAttribute(string permission)
        {
            _permission = permission;
        }


        // =====================================================
        // KIỂM TRA ĐĂNG NHẬP + PHÂN QUYỀN
        // =====================================================

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var session =
                context.HttpContext.Session;


            // =================================================
            // 1. CHƯA ĐĂNG NHẬP KHU VỰC QUẢN TRỊ
            // =================================================

            if (session.GetString("AdminLoggedIn") != "true")
            {
                context.Result =
                    new RedirectToActionResult(
                        "Index",
                        "AdminLogin",
                        new
                        {
                            area = "Admin"
                        }
                    );

                return;
            }


            // =================================================
            // 2. KIỂM TRA ID TÀI KHOẢN TRONG SESSION
            // =================================================

            var accountId =
                session.GetInt32("AdminAccountId");


            if (!accountId.HasValue)
            {
                ClearAdminSession(session);

                context.Result =
                    new RedirectToActionResult(
                        "Index",
                        "AdminLogin",
                        new
                        {
                            area = "Admin"
                        }
                    );

                return;
            }


            // =================================================
            // 3. LẤY DB CONTEXT
            // =================================================

            var db =
                context.HttpContext
                    .RequestServices
                    .GetRequiredService<CoffeeShopDbContext>();


            // =================================================
            // 4. ĐỌC LẠI TÀI KHOẢN + ROLE + QUYỀN TỪ DATABASE
            // =================================================

            var account =
                await db.TaiKhoans

                    .AsNoTracking()

                    .Include(
                        x => x.MaVaiTroNavigation
                    )

                    .ThenInclude(
                        x => x.MaQuyens
                    )

                    .FirstOrDefaultAsync(
                        x => x.MaTaiKhoan ==
                             accountId.Value
                    );


            // =================================================
            // 5. TÀI KHOẢN KHÔNG CÒN TỒN TẠI / BỊ KHÓA
            // =================================================

            if (account == null ||
                !account.IsActive)
            {
                ClearAdminSession(session);

                context.Result =
                    new RedirectToActionResult(
                        "Index",
                        "AdminLogin",
                        new
                        {
                            area = "Admin"
                        }
                    );

                return;
            }


            // =================================================
            // 6. VAI TRÒ KHÔNG TỒN TẠI / BỊ KHÓA
            // =================================================

            if (account.MaVaiTroNavigation == null ||
                !account.MaVaiTroNavigation.IsActive)
            {
                ClearAdminSession(session);

                context.Result =
                    new RedirectToActionResult(
                        "Index",
                        "AdminLogin",
                        new
                        {
                            area = "Admin"
                        }
                    );

                return;
            }


            // =================================================
            // 7. KIỂM TRA ROLE
            // CHỈ ADMIN / NHÂN VIÊN ĐƯỢC Ở KHU QUẢN TRỊ
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
                ClearAdminSession(session);

                context.Result =
                    new RedirectToActionResult(
                        "Index",
                        "AdminLogin",
                        new
                        {
                            area = "Admin"
                        }
                    );

                return;
            }


            // =================================================
            // 8. LẤY DANH SÁCH QUYỀN HIỆN TẠI
            // =================================================

            var permissions =
                account.MaVaiTroNavigation
                    .MaQuyens

                    .Where(
                        x => x.IsActive
                    )

                    .Select(
                        x => x.TenQuyen
                    )

                    .Where(
                        x => !string.IsNullOrWhiteSpace(x)
                    )

                    .Distinct(
                        StringComparer.OrdinalIgnoreCase
                    )

                    .ToList();


            // =================================================
            // 9. ĐỒNG BỘ SESSION
            // =================================================

            session.SetString(
                "AdminRole",
                roleName
            );


            session.SetString(
                "AdminUsername",
                account.TenDangNhap
            );


            session.SetString(
                "AdminFullName",
                account.HoTen ??
                account.TenDangNhap
            );


            session.SetString(
                "AdminAvatar",
                account.HinhAnh ?? ""
            );


            session.SetString(
                "AdminPermissions",
                string.Join(
                    "|",
                    permissions
                )
            );


            // =================================================
            // 10. CONTROLLER KHÔNG YÊU CẦU QUYỀN CỤ THỂ
            // =================================================

            if (string.IsNullOrWhiteSpace(
                    _permission
                ))
            {
                await next();

                return;
            }


            // =================================================
            // 11. KIỂM TRA QUYỀN
            // =================================================

            var hasPermission =
                permissions.Any(
                    x => string.Equals(
                        x,
                        _permission,
                        StringComparison.OrdinalIgnoreCase
                    )
                );


            // =================================================
            // 12. KHÔNG CÓ QUYỀN
            // =================================================

            if (!hasPermission)
            {
                context.Result =
                    new RedirectToActionResult(
                        "AccessDenied",
                        "AdminLogin",
                        new
                        {
                            area = "Admin"
                        }
                    );

                return;
            }


            // =================================================
            // 13. CÓ QUYỀN -> CHO PHÉP THỰC HIỆN ACTION
            // =================================================

            await next();
        }


        // =====================================================
        // XÓA SESSION RIÊNG CỦA ADMIN
        // KHÔNG CLEAR TOÀN BỘ SESSION
        // ĐỂ KHÔNG ẢNH HƯỞNG GIỎ HÀNG / CUSTOMER
        // =====================================================

        private static void ClearAdminSession(
            ISession session)
        {
            session.Remove(
                "AdminLoggedIn"
            );

            session.Remove(
                "AdminAccountId"
            );

            session.Remove(
                "AdminUsername"
            );

            session.Remove(
                "AdminFullName"
            );

            session.Remove(
                "AdminRole"
            );

            session.Remove(
                "AdminAvatar"
            );

            session.Remove(
                "AdminPermissions"
            );
        }
    }
}