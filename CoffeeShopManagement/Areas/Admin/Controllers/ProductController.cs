using CoffeeShopManagement.Data;
using CoffeeShopManagement.Helpers;
using CoffeeShopManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
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
        // SEARCH + FILTER + PAGINATION
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            string? keyword,
            int? categoryId,
            string? promotion,
            int page = 1,
            int pageSize = 10)
        {
            if (page < 1)
            {
                page = 1;
            }

            var allowedPageSizes = new[] { 10, 20, 50 };

            if (!allowedPageSizes.Contains(pageSize))
            {
                pageSize = 10;
            }


            var query =
                _context.SanPhams
                    .Include(x => x.MaLoaiNavigation)
                    .AsNoTracking()
                    .AsQueryable();


            // =================================================
            // TÌM KIẾM
            // =================================================

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.TenSp.Contains(keyword) ||
                    (x.MoTa != null &&
                     x.MoTa.Contains(keyword))
                );
            }


            // =================================================
            // LỌC DANH MỤC
            // =================================================

            if (categoryId.HasValue &&
                categoryId.Value > 0)
            {
                query = query.Where(
                    x => x.MaLoai == categoryId.Value
                );
            }


            // =================================================
            // LỌC KHUYẾN MÃI
            // =================================================

            if (!string.IsNullOrWhiteSpace(promotion))
            {
                if (promotion == "sale")
                {
                    query = query.Where(
                        x => x.DangKhuyenMai
                    );
                }
                else if (promotion == "normal")
                {
                    query = query.Where(
                        x => !x.DangKhuyenMai
                    );
                }
            }


            // =================================================
            // TỔNG KẾT QUẢ SAU KHI LỌC
            // =================================================

            var totalItems =
                await query.CountAsync();


            var totalPages =
                (int)Math.Ceiling(
                    totalItems / (double)pageSize
                );


            if (totalPages > 0 &&
                page > totalPages)
            {
                page = totalPages;
            }


            // =================================================
            // PHÂN TRANG
            // =================================================

            var products =
                await query
                    .OrderBy(x => x.MaSp)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();


            // =================================================
            // LOAD DANH MỤC CHO FILTER
            // =================================================

            ViewBag.FilterCategories =
                await _context.LoaiSanPhams
                    .AsNoTracking()
                    .OrderBy(x => x.TenLoai)
                    .ToListAsync();


            // =================================================
            // VIEWBAG
            // =================================================

            ViewBag.Keyword = keyword;
            ViewBag.CategoryId = categoryId;
            ViewBag.Promotion = promotion;

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;

            ViewBag.StartItem =
                totalItems == 0
                    ? 0
                    : ((page - 1) * pageSize) + 1;

            ViewBag.EndItem =
                Math.Min(
                    page * pageSize,
                    totalItems
                );


            return View(products);
        }


        // =====================================================
        // XEM CHI TIẾT SẢN PHẨM
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product =
                await _context.SanPhams
                    .Include(x => x.MaLoaiNavigation)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.MaSp == id
                    );


            if (product == null)
            {
                return NotFound();
            }


            return View(product);
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


            // =================================================
            // UPLOAD ẢNH
            // =================================================

            if (imageFile != null &&
                imageFile.Length > 0)
            {
                product.HinhAnh =
                    await SaveImage(imageFile);
            }


            _context.SanPhams.Add(
                product
            );


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


            // =================================================
            // UPDATE THÔNG TIN
            // =================================================

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


            // =================================================
            // ẢNH MỚI
            // =================================================

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
                TempData["ErrorMessage"] =
                    "Không tìm thấy sản phẩm.";

                return RedirectToAction(
                    nameof(Index)
                );
            }


            try
            {
                _context.SanPhams.Remove(
                    product
                );


                await _context.SaveChangesAsync();


                TempData["SuccessMessage"] =
                    $"Đã xóa sản phẩm \"{product.TenSp}\" thành công!";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] =
                    "Không thể xóa sản phẩm này vì sản phẩm đang được sử dụng trong đơn hàng hoặc dữ liệu khác.";
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
            var allowedExtensions =
                new[]
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".webp"
                };


            var extension =
                Path.GetExtension(
                    imageFile.FileName
                )
                .ToLowerInvariant();


            if (!allowedExtensions.Contains(
                    extension
                ))
            {
                throw new InvalidOperationException(
                    "Định dạng ảnh không hợp lệ."
                );
            }


            const long maxFileSize =
                5 * 1024 * 1024;


            if (imageFile.Length >
                maxFileSize)
            {
                throw new InvalidOperationException(
                    "Ảnh sản phẩm không được vượt quá 5MB."
                );
            }


            var fileName =
                $"{Guid.NewGuid():N}{extension}";


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


            await using var stream =
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