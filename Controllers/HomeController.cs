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
        private readonly EmailService _emailService;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, KidoraDbContext context, EmailService emailService, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _emailService = emailService;
            _config = config;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(string name, string email, string message)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin.";
                return RedirectToAction("Contact");
            }

            var adminEmail = _config["Contact:AdminEmail"] ?? "shopkidora@gmail.com";
            var subject = $"Liên hệ mới từ {name}";
            var body = $@"<h3>Thông tin liên hệ</h3>
                          <p><strong>Họ tên:</strong> {name}</p>
                          <p><strong>Email:</strong> {email}</p>
                          <p><strong>Nội dung:</strong><br/>{message}</p>";

            await _emailService.SendEmailAsync(adminEmail, subject, body);
            TempData["Success"] = "Đã gửi thông tin liên hệ. Chúng tôi sẽ phản hồi sớm nhất.";
            return RedirectToAction("Contact");
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
