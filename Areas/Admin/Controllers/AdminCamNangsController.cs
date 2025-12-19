using KIDORA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KIDORA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminCamNangsController : Controller
    {
        private readonly KidoraDbContext _context;

        public AdminCamNangsController(KidoraDbContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminCamNangs
        public async Task<IActionResult> Index()
        {
            return View(await _context.CamNangs.ToListAsync());
        }

        // GET: Admin/AdminCamNangs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camNang = await _context.CamNangs
                .FirstOrDefaultAsync(m => m.MaBai == id);
            if (camNang == null)
            {
                return NotFound();
            }

            return View(camNang);
        }

        // GET: Admin/AdminCamNangs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminCamNangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaBai,TieuDe,Anh,NgayDang,TacGia,TomTat,NoiDung")] CamNang camNang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(camNang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(camNang);
        }

        // GET: Admin/AdminCamNangs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camNang = await _context.CamNangs.FindAsync(id);
            if (camNang == null)
            {
                return NotFound();
            }
            return View(camNang);
        }

        // POST: Admin/AdminCamNangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaBai,TieuDe,Anh,NgayDang,TacGia,TomTat,NoiDung")] CamNang camNang)
        {
            if (id != camNang.MaBai)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(camNang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CamNangExists(camNang.MaBai))
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
            return View(camNang);
        }

        // GET: Admin/AdminCamNangs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camNang = await _context.CamNangs
                .FirstOrDefaultAsync(m => m.MaBai == id);
            if (camNang == null)
            {
                return NotFound();
            }

            return View(camNang);
        }

        // POST: Admin/AdminCamNangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var camNang = await _context.CamNangs.FindAsync(id);
            if (camNang != null)
            {
                _context.CamNangs.Remove(camNang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CamNangExists(string id)
        {
            return _context.CamNangs.Any(e => e.MaBai == id);
        }
    }
}
