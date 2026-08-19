using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Globalization;

using CoffeeShopManagement.Data;
using CoffeeShopManagement.Services;

var builder = WebApplication.CreateBuilder(args);


// =====================================================
// DATABASE
// =====================================================

builder.Services.AddDbContext<CoffeeShopDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.CommandTimeout(120);

            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );
        }
    );
});


// =====================================================
// LOCALIZATION
// =====================================================

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});


// =====================================================
// MVC
// =====================================================

builder.Services
    .AddControllersWithViews()
    .AddViewLocalization();


// =====================================================
// SESSION
// =====================================================

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);

    options.Cookie.HttpOnly = true;

    options.Cookie.IsEssential = true;
});


// =====================================================
// AUTHENTICATION
// ADMIN + NHÂN VIÊN + KHÁCH HÀNG
// =====================================================

builder.Services
    .AddAuthentication(options =>
    {
        // Cookie là đăng nhập mặc định
        options.DefaultAuthenticateScheme =
            CookieAuthenticationDefaults.AuthenticationScheme;

        options.DefaultSignInScheme =
            CookieAuthenticationDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            CookieAuthenticationDefaults.AuthenticationScheme;
    })

    .AddCookie(options =>
    {
        // Chưa đăng nhập -> chuyển tới đây
        options.LoginPath = "/Account/Login";

        // Không đủ quyền
        options.AccessDeniedPath = "/Account/AccessDenied";

        // Cookie tồn tại 2 giờ
        options.ExpireTimeSpan = TimeSpan.FromHours(2);

        options.SlidingExpiration = true;
    })

    // Google Login
    .AddGoogle(options =>
    {
        options.ClientId =
            builder.Configuration[
                "Authentication:Google:ClientId"
            ]!;

        options.ClientSecret =
            builder.Configuration[
                "Authentication:Google:ClientSecret"
            ]!;
    });


// =====================================================
// AUTHORIZATION - PHÂN QUYỀN
// =====================================================

builder.Services.AddAuthorization(options =>
{
    // =================================================
    // CHỈ ADMIN
    // =================================================

    options.AddPolicy(
        "AdminOnly",
        policy =>
        {
            policy.RequireRole("Admin");
        }
    );


    // =================================================
    // ADMIN HOẶC NHÂN VIÊN
    // =================================================

    options.AddPolicy(
        "StaffOrAdmin",
        policy =>
        {
            policy.RequireRole(
                "Admin",
                "Nhân viên"
            );
        }
    );


    // =================================================
    // KHÁCH HÀNG
    // =================================================

    options.AddPolicy(
        "CustomerOnly",
        policy =>
        {
            policy.RequireRole("KhachHang");
        }
    );
});


// =====================================================
// EMAIL SERVICE
// =====================================================

builder.Services.AddScoped<EmailService>();


// =====================================================
// BUILD APP
// =====================================================

var app = builder.Build();


// =====================================================
// LOCALIZATION
// =====================================================

var supportedCultures = new[]
{
    new CultureInfo("vi"),
    new CultureInfo("en")
};

var localizationOptions =
    new RequestLocalizationOptions
    {
        DefaultRequestCulture =
            new RequestCulture("vi"),

        SupportedCultures =
            supportedCultures,

        SupportedUICultures =
            supportedCultures
    };

app.UseRequestLocalization(
    localizationOptions
);


// =====================================================
// HTTP PIPELINE
// =====================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Home/Error"
    );

    app.UseHsts();
}


// =====================================================
// HTTPS
// =====================================================

app.UseHttpsRedirection();


// =====================================================
// STATIC FILES
// =====================================================

app.MapStaticAssets();


// =====================================================
// ROUTING
// =====================================================

app.UseRouting();


// =====================================================
// SESSION
// PHẢI ĐẶT TRƯỚC AUTHENTICATION / AUTHORIZATION
// =====================================================

app.UseSession();


// =====================================================
// AUTHENTICATION + AUTHORIZATION
// =====================================================

app.UseAuthentication();

app.UseAuthorization();


// =====================================================
// AREA ROUTE
// ADMIN / STAFF
// PHẢI NẰM TRƯỚC DEFAULT ROUTE
// =====================================================

app.MapControllerRoute(
    name: "areas",
    pattern:
        "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
)
.WithStaticAssets();


// =====================================================
// DEFAULT ROUTE
// =====================================================

app.MapControllerRoute(
    name: "default",
    pattern:
        "{controller=Home}/{action=Index}/{id?}"
)
.WithStaticAssets();


// =====================================================
// RUN
// =====================================================

app.Run();