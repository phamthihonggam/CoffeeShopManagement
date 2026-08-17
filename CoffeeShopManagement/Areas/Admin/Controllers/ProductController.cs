using CoffeeShopManagement.Data;
using CoffeeShopManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly CoffeeShopDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductController(
            CoffeeShopDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        // =====================================================
        // DANH SÁCH SẢN PHẨM
        // =====================================================

        public async Task<IActionResult> Index(string? keyword)
        {
            var query = _context.SanPhams
                .Include(x => x.MaLoaiNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.TenSp.Contains(keyword) ||
                    (x.MoTa != null &&
                     x.MoTa.Contains(keyword))
                );
            }

            var products = await query
                .OrderBy(x => x.MaSp)
                .ToListAsync();

            ViewBag.Keyword = keyword;

            return View(products);
        }


        // =====================================================
        // THÊM SẢN PHẨM - GET
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategories();

            return View();
        }


        // =====================================================
        // THÊM SẢN PHẨM - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            SanPham product,
            IFormFile? imageFile)
        {
            // Navigation không nhập từ form
            ModelState.Remove("MaLoaiNavigation");

            if (!ModelState.IsValid)
            {
                await LoadCategories(product.MaLoai);

                return View(product);
            }


            // =============================
            // UPLOAD ẢNH
            // =============================

            if (imageFile != null &&
                imageFile.Length > 0)
            {
                product.HinhAnh =
                    await SaveImage(imageFile);
            }


            _context.SanPhams.Add(product);

            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                "Thêm sản phẩm thành công!";


            return RedirectToAction(
                nameof(Index)
            );
        }


        // =====================================================
        // SỬA SẢN PHẨM - GET
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product =
                await _context.SanPhams
                    .FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }


            await LoadCategories(
                product.MaLoai
            );


            return View(product);
        }


        // =====================================================
        // SỬA SẢN PHẨM - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            SanPham product,
            IFormFile? imageFile)
        {
            if (id != product.MaSp)
            {
                return NotFound();
            }


            ModelState.Remove(
                "MaLoaiNavigation"
            );


            if (!ModelState.IsValid)
            {
                await LoadCategories(
                    product.MaLoai
                );

                return View(product);
            }


            var existingProduct =
                await _context.SanPhams
                    .FindAsync(id);


            if (existingProduct == null)
            {
                return NotFound();
            }


            // =============================
            // UPDATE THÔNG TIN
            // =============================

            existingProduct.TenSp =
                product.TenSp;

            existingProduct.DonGia =
                product.DonGia;

            existingProduct.MoTa =
                product.MoTa;

            existingProduct.MaLoai =
                product.MaLoai;

            existingProduct.GiaGoc =
                product.GiaGoc;

            existingProduct.GiaKhuyenMai =
                product.GiaKhuyenMai;

            existingProduct.PhanTramGiam =
                product.PhanTramGiam;

            existingProduct.DangKhuyenMai =
                product.DangKhuyenMai;

            existingProduct.NgayBatDau =
                product.NgayBatDau;

            existingProduct.NgayKetThuc =
                product.NgayKetThuc;


            // =============================
            // ẢNH MỚI
            // =============================

            if (imageFile != null &&
                imageFile.Length > 0)
            {
                existingProduct.HinhAnh =
                    await SaveImage(imageFile);
            }


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                "Cập nhật sản phẩm thành công!";


            return RedirectToAction(
                nameof(Index)
            );
        }


        // =====================================================
        // XÓA SẢN PHẨM
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product =
                await _context.SanPhams
                    .FindAsync(id);


            if (product == null)
            {
                return NotFound();
            }


            try
            {
                _context.SanPhams.Remove(
                    product
                );

                await _context
                    .SaveChangesAsync();


                TempData["SuccessMessage"] =
                    "Xóa sản phẩm thành công!";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] =
                    "Không thể xóa sản phẩm này vì sản phẩm đang được sử dụng trong dữ liệu khác.";
            }


            return RedirectToAction(
                nameof(Index)
            );
        }


        // =====================================================
        // LOAD DANH MỤC
        // =====================================================

        private async Task LoadCategories(
            int? selectedId = null)
        {
            var categories =
                await _context.LoaiSanPhams
                    .OrderBy(x => x.TenLoai)
                    .ToListAsync();


            ViewBag.Categories =
                new SelectList(
                    categories,
                    "MaLoai",
                    "TenLoai",
                    selectedId
                );
        }


        // =====================================================
        // LƯU ẢNH
        // =====================================================

        private async Task<string> SaveImage(
            IFormFile imageFile)
        {
            var extension =
                Path.GetExtension(
                    imageFile.FileName
                );


            var fileName =
                $"{Guid.NewGuid()}{extension}";


            var folder =
                Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "menu"
                );


            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(
                    folder
                );
            }


            var filePath =
                Path.Combine(
                    folder,
                    fileName
                );


            using var stream =
                new FileStream(
                    filePath,
                    FileMode.Create
                );


            await imageFile.CopyToAsync(
                stream
            );


            return fileName;
        }
    }
}