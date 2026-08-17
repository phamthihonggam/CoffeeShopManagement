using CoffeeShopManagement.Data;
using CoffeeShopManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopManagement.Controllers
{
    public class NhanViensController : Controller
    {
        private readonly CoffeeShopDbContext _context;

        public NhanViensController(CoffeeShopDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var ds = _context.NhanViens.ToList();
            return View(ds);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(NhanVien nv)
        {
            if (ModelState.IsValid)
            {
                _context.NhanViens.Add(nv);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(nv);
        }
        public IActionResult Edit(int id)
        {
            var nv = _context.NhanViens.Find(id);

            if (nv == null)
                return NotFound();

            return View(nv);
        }

        [HttpPost]
        public IActionResult Edit(NhanVien nv)
        {
            if (ModelState.IsValid)
            {
                _context.NhanViens.Update(nv);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(nv);
        }

        public IActionResult Delete(int id)
        {
            var nv = _context.NhanViens.Find(id);

            if (nv != null)
            {
                _context.NhanViens.Remove(nv);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}