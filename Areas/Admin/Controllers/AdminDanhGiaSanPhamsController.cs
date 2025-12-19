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

        // GET: Admin/AdminDanhGiaSanPhams
        public async Task<IActionResult> Index(string? searchString, int? rating, string? status)
        {
            // DANH_GIA_SAN_PHAM -> KHACH_HANG -> NGUOI_DUNG
            var query = _context.DanhGiaSanPhams
                .Include(d => d.MaSpNavigation)
                .Include(d => d.MaKhNavigation)
                    .ThenInclude(kh => kh.MaKhNavigation)  // NguoiDung
                .AsQueryable();

            // Tìm theo MÃ KH, TÊN KH hoặc TÊN SP
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var key = searchString.Trim().ToLower();

                query = query.Where(d =>
                    d.MaKh.Trim().ToLower().Contains(key) ||
                    d.MaKhNavigation.MaKhNavigation.HoTen.ToLower().Contains(key) ||
                    d.MaSpNavigation.TenSp.ToLower().Contains(key));
            }

            // Lọc theo điểm đánh giá (1–5 sao)
            if (rating.HasValue)
            {
                query = query.Where(d => d.DiemDanhGia == rating.Value);
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                var st = status.Trim().ToLower();

                if (st == "đang chờ duyệt")
                {
                    // những đánh giá chưa duyệt / chưa từ chối
                    query = query.Where(d =>
                        d.TrangThai == null ||
                        d.TrangThai.Trim() == "" ||
                        (
                            d.TrangThai.Trim().ToLower() != "approved" &&
                            d.TrangThai.Trim().ToLower() != "đã duyệt" &&
                            d.TrangThai.Trim().ToLower() != "rejected" &&
                            d.TrangThai.Trim().ToLower() != "đã từ chối"
                        )
                    );
                }
                else if (st == "approved")
                {
                    query = query.Where(d =>
                        d.TrangThai != null &&
                        (d.TrangThai.Trim().ToLower() == "approved"
                         || d.TrangThai.Trim().ToLower() == "đã duyệt"));
                }
                else if (st == "rejected")
                {
                    query = query.Where(d =>
                        d.TrangThai != null &&
                        (d.TrangThai.Trim().ToLower() == "rejected"
                         || d.TrangThai.Trim().ToLower() == "đã từ chối"));
                }
            }

            var list = await query
                .OrderBy(d => d.MaDanhGia)   // MÃ ĐG tăng dần
                .ToListAsync();

            return View(list);
        }

        // POST: duyệt đánh giá
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var danhGia = await _context.DanhGiaSanPhams.FindAsync(id);
            if (danhGia == null) return NotFound();

            danhGia.TrangThai = "Đã duyệt";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: từ chối đánh giá
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var danhGia = await _context.DanhGiaSanPhams.FindAsync(id);
            if (danhGia == null) return NotFound();

            danhGia.TrangThai = "Đã từ chối";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
