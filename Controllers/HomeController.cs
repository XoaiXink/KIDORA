using KIDORA.Data;
using KIDORA.Models;
using KIDORA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace KIDORA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly KidoraDbContext _context;

        public HomeController(ILogger<HomeController> logger, KidoraDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy toàn bộ sản phẩm từ database
            var list = _context.SanPhams
                .Include(p => p.MaDanhMucNavigation)
                .Select(p => new ListSanPhamVM
                {
                    MaSp = p.MaSp,
                    TenSp = p.TenSp,
                    DonGiaBan = p.DonGiaBan,
                    MoTaNgan = p.MoTaNgan,
                    MaDanhMuc = p.MaDanhMucNavigation.TenDanhMuc,
                    AnhChinh = p.AnhChinh ?? ""  // tránh lỗi null
                })
                .ToList();

            // gửi sang View
            ViewBag.ListSP = list;

            return View();
        }

        [Route("/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult Gioithieu()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
