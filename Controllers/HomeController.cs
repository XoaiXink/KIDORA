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

        // Aggregated search across products, cam nang, and static pages (policy/contact)
        [HttpGet]
        public IActionResult Search(string q)
        {
            var vm = new ViewModels.SearchResultVM { Keyword = q ?? string.Empty };

            if (string.IsNullOrWhiteSpace(q))
            {
                return View(vm);
            }

            var key = q.Trim().ToLower();

            // Products
            vm.Products = _context.SanPhams
                .Where(p => p.DangBan && (
                    p.TenSp.ToLower().Contains(key) ||
                    p.Sku.ToLower().Contains(key) ||
                    p.MaSp.ToLower().Contains(key) ||
                    (p.MoTaNgan ?? "").ToLower().Contains(key)
                ))
                .Select(p => new ListSanPhamVM
                {
                    MaSp = p.MaSp,
                    TenSp = p.TenSp,
                    DonGiaBan = p.DonGiaBan,
                    MoTaNgan = p.MoTaNgan ?? "",
                    AnhChinh = p.AnhChinh ?? ""
                })
                .Take(50)
                .ToList();

            // Cam nang (articles)
            // Use database collation that is accent-insensitive so searches without diacritics match Vietnamese text
            var pattern = $"%{q}%";
            const string viCollation = "Vietnamese_CI_AI"; // case-insensitive, accent-insensitive

            vm.CamNangs = _context.CamNangs
                .Where(c =>
                    EF.Functions.Like(EF.Functions.Collate(c.TieuDe, viCollation), pattern) ||
                    EF.Functions.Like(EF.Functions.Collate(c.TomTat ?? string.Empty, viCollation), pattern) ||
                    EF.Functions.Like(EF.Functions.Collate(c.NoiDung ?? string.Empty, viCollation), pattern)
                )
                .Select(c => new CamNangVM
                {
                    MaBai = c.MaBai,
                    TieuDe = c.TieuDe,
                    Anh = c.Anh,
                    NgayDang = c.NgayDang,
                    TacGia = c.TacGia,
                    TomTat = c.TomTat ?? "",
                    NoiDung = c.NoiDung ?? ""
                })
                .Take(50)
                .ToList();

            // Static pages: we'll search small set of known pages by keyword and build simple results
            var pages = new List<ViewModels.PageResultVM>();
            var lower = key;

            // Check if the user's query mentions policy/payment/shipping/privacy (use contains on the query)
            if (lower.Contains("chính sách") || lower.Contains("chinh sach") || lower.Contains("bảo mật") || lower.Contains("bao mat") || lower.Contains("giao hàng") || lower.Contains("giao hang") || lower.Contains("thanh toán") || lower.Contains("thanh toan"))
            {
                pages.Add(new ViewModels.PageResultVM { Title = "Chính sách bảo mật", Url = Url.Action("ChinhSachBaoMat", "DieuKhoanChinhSach"), Snippet = "Chính sách bảo mật thông tin khách hàng" });
                pages.Add(new ViewModels.PageResultVM { Title = "Chính sách giao hàng", Url = Url.Action("ChinhSachGiaoHang", "DieuKhoanChinhSach"), Snippet = "Thông tin về phí và điều kiện giao hàng" });
                pages.Add(new ViewModels.PageResultVM { Title = "Chính sách thanh toán", Url = Url.Action("ChinhSachThanhToan", "DieuKhoanChinhSach"), Snippet = "Hướng dẫn các phương thức thanh toán" });
            }

            // Contact page
            if (lower.Contains("liên hệ") || lower.Contains("lien he") || lower.Contains("contact") || lower.Contains("hotline"))
            {
                pages.Add(new ViewModels.PageResultVM { Title = "Liên hệ", Url = Url.Action("Contact", "Home"), Snippet = "Gửi liên hệ hoặc thông tin hỗ trợ" });
            }

            vm.Pages = pages;

            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
