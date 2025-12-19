using KIDORA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KIDORA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminSanPhamsController : Controller
    {
        private readonly KidoraDbContext _context;

        public AdminSanPhamsController(KidoraDbContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminSanPhams
        public async Task<IActionResult> Index(string? keyword, int page = 1, int pageSize = 10)
        {
            var query = _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaNccNavigation)
                .AsQueryable();

            // Tìm kiếm theo keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(s =>
                    s.TenSp.Contains(keyword) ||
                    s.Sku.Contains(keyword) ||
                    s.MaSp.Contains(keyword) ||
                    (s.MaDanhMucNavigation != null && s.MaDanhMucNavigation.TenDanhMuc.Contains(keyword)) ||
                    (s.MaNccNavigation != null && s.MaNccNavigation.TenNcc.Contains(keyword))
                );
            }

            // Tính toán phân trang
            int tongSanPham = await query.CountAsync();
            int soTrang = (int)Math.Ceiling(tongSanPham / (double)pageSize);

            if (page < 1) page = 1;
            if (page > soTrang && soTrang > 0) page = soTrang;

            // Lấy dữ liệu theo trang
            var sanPhams = await query
                .OrderBy(s => s.MaSp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Truyền dữ liệu phân trang vào ViewBag
            ViewBag.Keyword = keyword;
            ViewBag.TrangHienTai = page;
            ViewBag.SoTrang = soTrang;
            ViewBag.TongSanPham = tongSanPham;
            ViewBag.PageSize = pageSize;

            return View(sanPhams);
        }

        // GET: Admin/AdminSanPhams/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaNccNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: Admin/AdminSanPhams/Create
        public IActionResult Create()
        {
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc");
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "TenNcc");
            return View();
        }

        // POST: Admin/AdminSanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSp,Sku,TenSp,MoTaNgan,MoTaChiTiet,MaDanhMuc,MaNcc,ThuongHieu,DoTuoiToiThieu,DoTuoiToiDa,DonGiaBan,GiaNiemYet,KhoiLuong,AnhChinh,DangBan")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "TenNcc", sanPham.MaNcc);
            return View(sanPham);
        }

        // GET: Admin/AdminSanPhams/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "TenNcc", sanPham.MaNcc);
            return View(sanPham);
        }

        // POST: Admin/AdminSanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaSp,Sku,TenSp,MoTaNgan,MoTaChiTiet,MaDanhMuc,MaNcc,ThuongHieu,DoTuoiToiThieu,DoTuoiToiDa,DonGiaBan,GiaNiemYet,KhoiLuong,AnhChinh,DangBan")] SanPham sanPham)
        {
            if (id != sanPham.MaSp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.MaSp))
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
            ViewData["MaDanhMuc"] = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "TenNcc", sanPham.MaNcc);
            return View(sanPham);
        }

        // GET: Admin/AdminSanPhams/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .Include(s => s.MaNccNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: Admin/AdminSanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                _context.SanPhams.Remove(sanPham);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleVisibility(string id, bool disable = true)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FirstOrDefaultAsync(s => s.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            sanPham.DangBan = !disable;
            await _context.SaveChangesAsync();

            TempData["Success"] = disable ? "Đã vô hiệu hóa sản phẩm." : "Đã kích hoạt sản phẩm.";
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(string id)
        {
            return _context.SanPhams.Any(e => e.MaSp == id);
        }
    }
}
