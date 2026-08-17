using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoffeeShopManagement.Helpers
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string? _permission;

        public AdminAuthorizeAttribute()
        {
        }

        public AdminAuthorizeAttribute(string permission)
        {
            _permission = permission;
        }

        public override void OnActionExecuting(
            ActionExecutingContext context)
        {
            var session =
                context.HttpContext.Session;

            // =========================================
            // 1. CHƯA ĐĂNG NHẬP ADMIN
            // =========================================

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


            // =========================================
            // 2. KHÔNG YÊU CẦU QUYỀN CỤ THỂ
            // =========================================

            if (string.IsNullOrWhiteSpace(_permission))
            {
                base.OnActionExecuting(context);
                return;
            }


            // =========================================
            // 3. LẤY QUYỀN TỪ SESSION
            // =========================================

            var permissionString =
                session.GetString(
                    "AdminPermissions"
                ) ?? "";


            var permissions =
                permissionString
                    .Split(
                        '|',
                        StringSplitOptions.RemoveEmptyEntries
                    );


            // =========================================
            // 4. KIỂM TRA QUYỀN
            // =========================================

            var hasPermission =
                permissions.Any(
                    x => string.Equals(
                        x,
                        _permission,
                        StringComparison.OrdinalIgnoreCase
                    )
                );


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


            base.OnActionExecuting(context);
        }
    }
}