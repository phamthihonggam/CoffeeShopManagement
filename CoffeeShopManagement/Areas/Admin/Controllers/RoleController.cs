using CoffeeShopManagement.Data;
using CoffeeShopManagement.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize("ROLE_MANAGE")]
    public class RoleController : Controller
    {
        private readonly CoffeeShopDbContext _context;

        public RoleController(
            CoffeeShopDbContext context)
        {
            _context = context;
        }


        // =====================================================
        // DANH SÁCH VAI TRÒ
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var roles =
                await _context.VaiTros

                    .Include(x => x.MaQuyens)

                    .OrderBy(x => x.MaVaiTro)

                    .ToListAsync();


            ViewBag.TotalRoles =
                roles.Count;


            ViewBag.ActiveRoles =
                roles.Count(
                    x => x.IsActive
                );


            ViewBag.TotalPermissions =
                await _context.Quyens

                    .CountAsync(
                        x => x.IsActive
                    );


            return View(
                roles
            );
        }


        // =====================================================
        // TRANG PHÂN QUYỀN
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Permissions(
            int id)
        {
            var role =
                await _context.VaiTros

                    .Include(
                        x => x.MaQuyens
                    )

                    .FirstOrDefaultAsync(
                        x => x.MaVaiTro == id
                    );


            if (role == null)
            {
                return NotFound();
            }


            var permissions =
                await _context.Quyens

                    .Where(
                        x => x.IsActive
                    )

                    .OrderBy(
                        x => x.MaQuyen
                    )

                    .ToListAsync();


            var selectedPermissionIds =
                role.MaQuyens

                    .Select(
                        x => x.MaQuyen
                    )

                    .ToHashSet();


            ViewBag.Role =
                role;


            ViewBag.Permissions =
                permissions;


            ViewBag.SelectedPermissionIds =
                selectedPermissionIds;


            return View();
        }


        // =====================================================
        // LƯU PHÂN QUYỀN
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Permissions(
            int id,
            List<int>? selectedPermissions)
        {
            var role =
                await _context.VaiTros

                    .Include(
                        x => x.MaQuyens
                    )

                    .FirstOrDefaultAsync(
                        x => x.MaVaiTro == id
                    );


            if (role == null)
            {
                return NotFound();
            }


            selectedPermissions ??=
                new List<int>();


            var validPermissions =
                await _context.Quyens

                    .Where(
                        x =>
                            x.IsActive
                            &&
                            selectedPermissions.Contains(
                                x.MaQuyen
                            )
                    )

                    .ToListAsync();


            role.MaQuyens.Clear();


            foreach (var permission in validPermissions)
            {
                role.MaQuyens.Add(
                    permission
                );
            }


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                $"Đã cập nhật quyền cho vai trò \"{role.TenVaiTro}\".";


            return RedirectToAction(
                nameof(Index)
            );
        }
    }
}