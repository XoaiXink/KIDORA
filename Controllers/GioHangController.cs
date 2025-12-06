using KIDORA.Data;
using KIDORA.Helpers;
using KIDORA.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace KIDORA.Controllers
{

    public class GioHangController : Controller
    {
        private readonly KidoraDbContext _context;
        public GioHangController(KidoraDbContext context) { _context = context; }

        public List<SanPhamGioHang> GioHang() =>
     HttpContext.Session.GetT<List<SanPhamGioHang>>(MySetting.GIOHANG_KEY)
     ?? new List<SanPhamGioHang>();
        public IActionResult Index()
        {
            return View(GioHang());
        }

        // Thêm sản phẩm vào giỏ
        public IActionResult AddToCart(string id, int quantity = 1)
        {
            var gioHang = GioHang();

            // Kiểm tra sản phẩm đã có trong giỏ chưa
            var sanPham = gioHang.FirstOrDefault(p => p.MaSp == id);

            if (sanPham == null)
            {
                // Lấy từ DB
                var sp = _context.SanPhams.FirstOrDefault(p => p.MaSp == id);
                if (sp == null)
                {
                    TempData["ErrorMessage"] = $"Không tìm thấy sản phẩm với mã {id}.";
                    return Redirect("/404");
                }

                // Thêm mới vào giỏ
                sanPham = new SanPhamGioHang
                {
                    MaSp = sp.MaSp,
                    TenSp = sp.TenSp,
                    DonGiaBan = sp.DonGiaBan,
                    AnhChinh = sp.AnhChinh,
                    SoLuong = quantity
                };

                gioHang.Add(sanPham);
            }
            else
            {
                // Nếu đã có → tăng số lượng
                sanPham.SoLuong += quantity;
            }

            // Lưu vào session
            HttpContext.Session.Set(MySetting.GIOHANG_KEY, gioHang);

            TempData["Success"] = "Đã thêm vào giỏ!";
            return RedirectToAction("Index");
        }
        public IActionResult XoaGioHang(string id)
        {
            var gioHang = GioHang();
            var sanpham = gioHang.FirstOrDefault(p => p.MaSp == id);
            if (sanpham != null)
            {
                gioHang.Remove(sanpham);
                HttpContext.Session.Set(MySetting.GIOHANG_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UpdateQuantity(string id, int quantity)
        {
            var gioHang = GioHang();
            var item = gioHang.FirstOrDefault(p => p.MaSp == id);

            if (item == null) return Json(new { success = false });

            item.SoLuong = quantity;

            HttpContext.Session.Set(MySetting.GIOHANG_KEY, gioHang);

            return Json(new
            {
                success = true,
                subtotal = item.ThanhTien,
                cartTotal = gioHang.Sum(p => p.ThanhTien)
            });

        }
        public IActionResult ThanhToan()
        {

            if (GioHang().Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }
            return View(GioHang());
        }


    }
}
