using CoffeeShopManagement.Data;
using CoffeeShopManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "NhanVien")]
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


            var allowedPageSizes =
                new[]
                {
                    10,
                    20,
                    50
                };


            if (!allowedPageSizes.Contains(pageSize))
            {
                pageSize = 10;
            }


            // =================================================
            // QUERY
            // =================================================

            var query =
                _context.SanPhams
                    .Include(
                        x => x.MaLoaiNavigation
                    )
                    .AsNoTracking()
                    .AsQueryable();


            // =================================================
            // SEARCH
            // =================================================

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword =
                    keyword.Trim();


                query =
                    query.Where(
                        x =>
                            x.TenSp.Contains(keyword)
                            ||
                            (
                                x.MoTa != null
                                &&
                                x.MoTa.Contains(keyword)
                            )
                    );
            }


            // =================================================
            // FILTER CATEGORY
            // =================================================

            if (
                categoryId.HasValue
                &&
                categoryId.Value > 0
            )
            {
                query =
                    query.Where(
                        x =>
                            x.MaLoai == categoryId.Value
                    );
            }


            // =================================================
            // FILTER PROMOTION
            // =================================================

            if (!string.IsNullOrWhiteSpace(promotion))
            {
                promotion =
                    promotion.Trim();


                if (promotion == "sale")
                {
                    query =
                        query.Where(
                            x => x.DangKhuyenMai
                        );
                }

                else if (promotion == "normal")
                {
                    query =
                        query.Where(
                            x => !x.DangKhuyenMai
                        );
                }
            }


            // =================================================
            // TOTAL ITEMS
            // =================================================

            int totalItems =
                await query.CountAsync();


            int totalPages =
                (int)Math.Ceiling(
                    totalItems
                    /
                    (double)pageSize
                );


            if (
                totalPages > 0
                &&
                page > totalPages
            )
            {
                page =
                    totalPages;
            }


            // =================================================
            // PAGINATION
            // =================================================

            var products =
                await query

                    .OrderBy(
                        x => x.MaSp
                    )

                    .Skip(
                        (page - 1)
                        *
                        pageSize
                    )

                    .Take(
                        pageSize
                    )

                    .ToListAsync();


            // =================================================
            // CATEGORIES
            // =================================================

            ViewBag.FilterCategories =
                await _context
                    .LoaiSanPhams

                    .AsNoTracking()

                    .OrderBy(
                        x => x.TenLoai
                    )

                    .ToListAsync();


            // =================================================
            // VIEWBAG
            // =================================================

            ViewBag.Keyword =
                keyword;

            ViewBag.CategoryId =
                categoryId;

            ViewBag.Promotion =
                promotion;


            ViewBag.CurrentPage =
                page;

            ViewBag.PageSize =
                pageSize;

            ViewBag.TotalItems =
                totalItems;

            ViewBag.TotalPages =
                totalPages;


            ViewBag.StartItem =
                totalItems == 0
                    ? 0
                    : ((page - 1) * pageSize) + 1;


            ViewBag.EndItem =
                Math.Min(
                    page * pageSize,
                    totalItems
                );


            return View(
                products
            );
        }


        // =====================================================
        // DETAILS
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Details(
            int id)
        {
            var product =
                await _context
                    .SanPhams

                    .Include(
                        x => x.MaLoaiNavigation
                    )

                    .AsNoTracking()

                    .FirstOrDefaultAsync(
                        x =>
                            x.MaSp == id
                    );


            if (product == null)
            {
                return NotFound();
            }


            return View(
                product
            );
        }


        // =====================================================
        // CREATE - GET
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategories();


            return View();
        }


        // =====================================================
        // CREATE - POST
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


                return View(
                    product
                );
            }


            try
            {
                // =============================================
                // IMAGE
                // =============================================

                if (
                    imageFile != null
                    &&
                    imageFile.Length > 0
                )
                {
                    product.HinhAnh =
                        await SaveImage(
                            imageFile
                        );
                }


                _context
                    .SanPhams
                    .Add(
                        product
                    );


                await _context
                    .SaveChangesAsync();


                TempData["SuccessMessage"] =
                    "Thêm sản phẩm thành công!";


                return RedirectToAction(
                    nameof(Index)
                );
            }

            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    "imageFile",
                    ex.Message
                );


                await LoadCategories(
                    product.MaLoai
                );


                return View(
                    product
                );
            }
        }


        // =====================================================
        // EDIT - GET
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Edit(
            int id)
        {
            var product =
                await _context
                    .SanPhams
                    .FindAsync(
                        id
                    );


            if (product == null)
            {
                return NotFound();
            }


            await LoadCategories(
                product.MaLoai
            );


            return View(
                product
            );
        }


        // =====================================================
        // EDIT - POST
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


                return View(
                    product
                );
            }


            var existingProduct =
                await _context
                    .SanPhams
                    .FindAsync(
                        id
                    );


            if (existingProduct == null)
            {
                return NotFound();
            }


            try
            {
                // =============================================
                // UPDATE BASIC INFO
                // =============================================

                existingProduct.TenSp =
                    product.TenSp;

                existingProduct.DonGia =
                    product.DonGia;

                existingProduct.MoTa =
                    product.MoTa;

                existingProduct.MaLoai =
                    product.MaLoai;


                // =============================================
                // PROMOTION
                // =============================================

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


                // =============================================
                // NEW IMAGE
                // =============================================

                if (
                    imageFile != null
                    &&
                    imageFile.Length > 0
                )
                {
                    existingProduct.HinhAnh =
                        await SaveImage(
                            imageFile
                        );
                }


                await _context
                    .SaveChangesAsync();


                TempData["SuccessMessage"] =
                    "Cập nhật sản phẩm thành công!";


                return RedirectToAction(
                    nameof(Index)
                );
            }

            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    "imageFile",
                    ex.Message
                );


                await LoadCategories(
                    product.MaLoai
                );


                return View(
                    product
                );
            }
        }


        // =====================================================
        // DELETE
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            int id)
        {
            var product =
                await _context
                    .SanPhams
                    .FindAsync(
                        id
                    );


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
                _context
                    .SanPhams
                    .Remove(
                        product
                    );


                await _context
                    .SaveChangesAsync();


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
        // LOAD CATEGORIES
        // =====================================================

        private async Task LoadCategories(
            int? selectedId = null)
        {
            var categories =
                await _context
                    .LoaiSanPhams

                    .OrderBy(
                        x => x.TenLoai
                    )

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
        // SAVE IMAGE
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


            string extension =
                Path.GetExtension(
                    imageFile.FileName
                )
                .ToLowerInvariant();


            if (
                !allowedExtensions.Contains(
                    extension
                )
            )
            {
                throw new InvalidOperationException(
                    "Định dạng ảnh không hợp lệ. Chỉ chấp nhận JPG, JPEG, PNG hoặc WEBP."
                );
            }


            const long maxFileSize =
                5 * 1024 * 1024;


            if (
                imageFile.Length >
                maxFileSize
            )
            {
                throw new InvalidOperationException(
                    "Ảnh sản phẩm không được vượt quá 5MB."
                );
            }


            string fileName =
                $"{Guid.NewGuid():N}{extension}";


            string folder =
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


            string filePath =
                Path.Combine(
                    folder,
                    fileName
                );


            await using var stream =
                new FileStream(
                    filePath,
                    FileMode.Create
                );


            await imageFile
                .CopyToAsync(
                    stream
                );


            return fileName;
        }
    }
}