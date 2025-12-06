using KIDORA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KIDORA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminHangThanhViensController : Controller
    {
        private readonly KidoraDbContext _context;

        public AdminHangThanhViensController(KidoraDbContext context)
        {
            _context = context;
        }

        // GET: Admin/HangThanhVien
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách hạng + kèm số khách ở từng hạng
            var list = await _context.HangThanhViens
                                     .Include(h => h.KhachHangs)
                                     .ToListAsync();

            return View(list);
        }
    }
}


