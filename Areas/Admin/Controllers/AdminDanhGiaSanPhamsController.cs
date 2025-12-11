using KIDORA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KIDORA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminDanhGiaSanPhamsController : Controller
    {
        private readonly KidoraDbContext _context;

        public AdminDanhGiaSanPhamsController(KidoraDbContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminDanhGia
        public async Task<IActionResult> Index(string searchString, int? rating, string status)
        {
            var query = _context.DanhGiaSanPhams
                                .Include(d => d.MaKhNavigation)
                                    .ThenInclude(kh => kh.MaKhNavigation)// KHACH_HANG
                                .Include(d => d.MaSpNavigation)   // SAN_PHAM
                                .AsQueryable();

            // Tìm theo MÃ KH, TÊN KH hoặc TÊN SP
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var key = searchString.ToLower();
                query = query.Where(d =>
                    d.MaKh.ToLower().Contains(key) ||
                    d.MaKhNavigation.MaKhNavigation.HoTen.ToLower().Contains(key) ||
                    d.MaSpNavigation.TenSp.ToLower().Contains(key));
            }

            // Lọc theo điểm đánh giá (1–5 sao)
            if (rating.HasValue)
            {
                query = query.Where(d => d.DiemDanhGia == rating.Value);
            }

            // Lọc theo trạng thái: Pending/Approved/Rejected
            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                query = query.Where(d => d.TrangThai == status);
            }

            var list = await query
                .OrderByDescending(d => d.NgayDanhGia)
                .ToListAsync();

            return View(list);
        }
    }
}
