using KIDORA.Data;
using KIDORA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace KIDORA.Controllers
{
    public class CamNangController : Controller
    {
        private readonly KidoraDbContext _context;

        public CamNangController(KidoraDbContext context)
        {
            _context = context;
        }

        // ================================
        // HIỂN THỊ DANH SÁCH BÀI VIẾT
        // ================================
        public async Task<IActionResult> Index()
        {
            var list = await _context.CamNangs
                .OrderByDescending(x => x.NgayDang)
                .Select(bai => new CamNangVM
                {
                    MaBai = bai.MaBai,
                    TieuDe = bai.TieuDe,
                    Anh = bai.Anh,
                    NgayDang = bai.NgayDang,
                    TacGia = bai.TacGia,
                    TomTat = bai.TomTat,
                    NoiDung = bai.NoiDung
                })
                .ToListAsync();

            return View(list);
        }

        // ================================
        // HIỂN THỊ CHI TIẾT BÀI VIẾT
        // ================================
        public async Task<IActionResult> ChiTiet(string id)
        {
            if (id == null) return NotFound();

            // 1. Lấy dữ liệu Data Model
            var bai = await _context.CamNangs
                .FirstOrDefaultAsync(x => x.MaBai == id);

            if (bai == null) return NotFound();

            // 2. Chuyển đổi (Map) sang View Model
            var viewModel = new CamNangVM
            {
                MaBai = bai.MaBai,
                TieuDe = bai.TieuDe,
                Anh = bai.Anh,
                NgayDang = bai.NgayDang,
                TacGia = bai.TacGia,
                TomTat = bai.TomTat,
                NoiDung = bai.NoiDung
            };

            // 3. Truyền View Model sang View
            return View(viewModel);
        }
    }
}




