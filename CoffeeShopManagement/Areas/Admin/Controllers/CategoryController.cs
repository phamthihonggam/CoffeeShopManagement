using CoffeeShopManagement.Data;
using CoffeeShopManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly CoffeeShopDbContext _context;

        public CategoryController(CoffeeShopDbContext context)
        {
            _context = context;
        }


        // =========================================
        // DANH SÁCH DANH MỤC
        // =========================================

        public async Task<IActionResult> Index()
        {
            var categories = await _context.LoaiSanPhams
                .Include(x => x.SanPhams)
                .OrderBy(x => x.MaLoai)
                .ToListAsync();

            return View(categories);
        }


        // =========================================
        // THÊM DANH MỤC - GET
        // =========================================

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        // =========================================
        // THÊM DANH MỤC - POST
        // =========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            LoaiSanPham category)
        {
            if (string.IsNullOrWhiteSpace(category.TenLoai))
            {
                ModelState.AddModelError(
                    "TenLoai",
                    "Vui lòng nhập tên danh mục."
                );
            }


            // Không cho trùng tên
            var exists = await _context.LoaiSanPhams
                .AnyAsync(x =>
                    x.TenLoai.ToLower() ==
                    category.TenLoai.Trim().ToLower()
                );


            if (exists)
            {
                ModelState.AddModelError(
                    "TenLoai",
                    "Danh mục này đã tồn tại."
                );
            }


            if (!ModelState.IsValid)
            {
                return View(category);
            }


            category.TenLoai =
                category.TenLoai.Trim();


            _context.LoaiSanPhams.Add(category);

            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                "Thêm danh mục thành công!";


            return RedirectToAction(
                nameof(Index)
            );
        }


        // =========================================
        // SỬA DANH MỤC - GET
        // =========================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category =
                await _context.LoaiSanPhams
                    .FindAsync(id);


            if (category == null)
            {
                return NotFound();
            }


            return View(category);
        }


        // =========================================
        // SỬA DANH MỤC - POST
        // =========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            LoaiSanPham category)
        {
            if (id != category.MaLoai)
            {
                return NotFound();
            }


            if (string.IsNullOrWhiteSpace(category.TenLoai))
            {
                ModelState.AddModelError(
                    "TenLoai",
                    "Vui lòng nhập tên danh mục."
                );
            }


            var exists = await _context.LoaiSanPhams
                .AnyAsync(x =>
                    x.MaLoai != id &&
                    x.TenLoai.ToLower() ==
                    category.TenLoai.Trim().ToLower()
                );


            if (exists)
            {
                ModelState.AddModelError(
                    "TenLoai",
                    "Tên danh mục này đã tồn tại."
                );
            }


            if (!ModelState.IsValid)
            {
                return View(category);
            }


            var existingCategory =
                await _context.LoaiSanPhams
                    .FindAsync(id);


            if (existingCategory == null)
            {
                return NotFound();
            }


            existingCategory.TenLoai =
                category.TenLoai.Trim();


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                "Cập nhật danh mục thành công!";


            return RedirectToAction(
                nameof(Index)
            );
        }


        // =========================================
        // XÓA DANH MỤC
        // =========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category =
                await _context.LoaiSanPhams
                    .Include(x => x.SanPhams)
                    .FirstOrDefaultAsync(
                        x => x.MaLoai == id
                    );


            if (category == null)
            {
                return NotFound();
            }


            // Không cho xóa nếu còn sản phẩm
            if (category.SanPhams.Any())
            {
                TempData["ErrorMessage"] =
                    $"Không thể xóa danh mục \"{category.TenLoai}\" vì vẫn còn {category.SanPhams.Count} sản phẩm.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            try
            {
                _context.LoaiSanPhams.Remove(
                    category
                );

                await _context.SaveChangesAsync();


                TempData["SuccessMessage"] =
                    "Xóa danh mục thành công!";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] =
                    "Không thể xóa danh mục này vì đang được sử dụng trong dữ liệu khác.";
            }


            return RedirectToAction(
                nameof(Index)
            );
        }
    }
}