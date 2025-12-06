using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KIDORA.Data;

namespace KIDORA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminGdKcoinsController : Controller
    {
        private readonly KidoraDbContext _context;

        public AdminGdKcoinsController(KidoraDbContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminGdKcoins
        public async Task<IActionResult> Index()
        {
            var kidoraDbContext = _context.GdKcoins.Include(g => g.MaDonHangNavigation).Include(g => g.MaKhNavigation);
            return View(await kidoraDbContext.ToListAsync());
        }

        // GET: Admin/AdminGdKcoins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gdKcoin = await _context.GdKcoins
                .Include(g => g.MaDonHangNavigation)
                .Include(g => g.MaKhNavigation)
                .FirstOrDefaultAsync(m => m.MaGdKcoin == id);
            if (gdKcoin == null)
            {
                return NotFound();
            }

            return View(gdKcoin);
        }

        // GET: Admin/AdminGdKcoins/Create
        public IActionResult Create()
        {
            ViewData["MaDonHang"] = new SelectList(_context.DonHangs, "MaDonHang", "MaDonHang");
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh");
            return View();
        }

        // POST: Admin/AdminGdKcoins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaGdKcoin,MaKh,MaDonHang,KieuGd,SoKcoin,SoDuSauGd,MoTa,ThoiGianGd")] GdKcoin gdKcoin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gdKcoin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaDonHang"] = new SelectList(_context.DonHangs, "MaDonHang", "MaDonHang", gdKcoin.MaDonHang);
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", gdKcoin.MaKh);
            return View(gdKcoin);
        }

        // GET: Admin/AdminGdKcoins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gdKcoin = await _context.GdKcoins.FindAsync(id);
            if (gdKcoin == null)
            {
                return NotFound();
            }
            ViewData["MaDonHang"] = new SelectList(_context.DonHangs, "MaDonHang", "MaDonHang", gdKcoin.MaDonHang);
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", gdKcoin.MaKh);
            return View(gdKcoin);
        }

        // POST: Admin/AdminGdKcoins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaGdKcoin,MaKh,MaDonHang,KieuGd,SoKcoin,SoDuSauGd,MoTa,ThoiGianGd")] GdKcoin gdKcoin)
        {
            if (id != gdKcoin.MaGdKcoin)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gdKcoin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GdKcoinExists(gdKcoin.MaGdKcoin))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaDonHang"] = new SelectList(_context.DonHangs, "MaDonHang", "MaDonHang", gdKcoin.MaDonHang);
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", gdKcoin.MaKh);
            return View(gdKcoin);
        }

        // GET: Admin/AdminGdKcoins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gdKcoin = await _context.GdKcoins
                .Include(g => g.MaDonHangNavigation)
                .Include(g => g.MaKhNavigation)
                .FirstOrDefaultAsync(m => m.MaGdKcoin == id);
            if (gdKcoin == null)
            {
                return NotFound();
            }

            return View(gdKcoin);
        }

        // POST: Admin/AdminGdKcoins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gdKcoin = await _context.GdKcoins.FindAsync(id);
            if (gdKcoin != null)
            {
                _context.GdKcoins.Remove(gdKcoin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GdKcoinExists(int id)
        {
            return _context.GdKcoins.Any(e => e.MaGdKcoin == id);
        }
    }
}
