using KIDORA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KIDORA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminDonHangsController : Controller
    {
        private readonly KidoraDbContext _context;
        private static readonly string[] _allowedStatuses = new[]
        {
            "Chờ xử lý",
            "Đang giao hàng",
            "Hoàn thành",
            "Hủy"
        };

        public AdminDonHangsController(KidoraDbContext context)
        {
            _context = context;
        }
        // GET: Admin/AdminDonHangs/Details
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            id = id.Trim();   

            // Lấy đơn hàng
            var donHang = await _context.DonHangs
                .FirstOrDefaultAsync(d => d.MaDonHang == id);

            if (donHang == null)
                return NotFound();

            // Lấy chi tiết đơn hàng
            var chiTiet = await _context.ChiTietDonHangs
                .Where(ct => ct.MaDonHang == id)
                .ToListAsync();

            ViewBag.ChiTiet = chiTiet;
            ViewBag.AvailableStatus = _allowedStatuses;

            return View(donHang);    // model: DonHang
        }

        // GET: Admin/AdminDonHangs
        public async Task<IActionResult> Index(
            string? keyword,
            string? trangThaiDon,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 10)
        {
            var query = _context.DonHangs.AsQueryable();

            // Lọc từ khóa: Mã đơn / Số đơn / Tên người nhận / SĐT
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim().ToLower();
                query = query.Where(d =>
                    d.MaDonHang.ToLower().Contains(k) ||
                    d.SoDonHang.ToLower().Contains(k) ||
                    d.TenNguoiNhan.ToLower().Contains(k) ||
                    d.SdtnguoiNhan.ToLower().Contains(k));
            }

            // Lọc theo khoảng ngày
            if (fromDate.HasValue)
            {
                var start = fromDate.Value.Date;
                query = query.Where(d => d.NgayDat >= start);
            }

            if (toDate.HasValue)
            {
                var end = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(d => d.NgayDat <= end);
            }

            // Lọc theo trạng thái đơn
            if (!string.IsNullOrWhiteSpace(trangThaiDon))
            {
                var st = trangThaiDon.Trim().ToLower();
                query = query.Where(d =>
                    d.TrangThaiDon != null &&
                    d.TrangThaiDon.Trim().ToLower() == st);
            }

            // Lấy danh sách trạng thái có trong DB để build dropdown
            var allStatus = await _context.DonHangs
                .Select(d => d.TrangThaiDon)
                .Where(s => s != null && s != "")
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            ViewBag.AllStatus = allStatus;
            ViewBag.Keyword = keyword;
            ViewBag.TrangThaiDon = trangThaiDon;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            var tongDon = await query.CountAsync();
            var soTrang = (int)Math.Ceiling(tongDon / (double)pageSize);

            if (page < 1) page = 1;
            if (page > soTrang && soTrang > 0) page = soTrang;

            var result = await query
                .OrderByDescending(d => d.NgayDat)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SoTrang = soTrang;
            ViewBag.TrangHienTai = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TongDon = tongDon;

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(string id, string trangThaiMoi)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var donHang = await _context.DonHangs.FirstOrDefaultAsync(d => d.MaDonHang == id.Trim());
            if (donHang == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(trangThaiMoi) || !_allowedStatuses.Contains(trangThaiMoi))
            {
                TempData["Error"] = "Trạng thái không hợp lệ.";
                return RedirectToAction(nameof(Details), new { id = id.Trim() });
            }

            donHang.TrangThaiDon = trangThaiMoi.Trim();
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật trạng thái đơn hàng thành công.";
            return RedirectToAction(nameof(Details), new { id = id.Trim() });
        }
    }
}
