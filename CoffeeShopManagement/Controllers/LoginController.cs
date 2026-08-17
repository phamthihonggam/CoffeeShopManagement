using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CoffeeShopManagement.Data;

namespace CoffeeShopManagement.Controllers
{
    public class LoginController : Controller
    {
        private readonly CoffeeShopDbContext _context;

        public LoginController(CoffeeShopDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            var user = _context.NhanViens
                .FirstOrDefault(x =>
                    x.TenDangNhap == username &&
                    x.MatKhau == password);

            if (user == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }

            HttpContext.Session.SetString("Role", user.ChucVu);
            HttpContext.Session.SetString("UserName", user.HoTen);

            if (user.ChucVu == "Admin")
            {
                return RedirectToAction("Admin");
            }

            return RedirectToAction("Staff");
        }

        public IActionResult Admin()
        {
            return View();
        }

        public IActionResult Staff()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}