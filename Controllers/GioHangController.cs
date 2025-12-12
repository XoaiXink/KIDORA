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

        [HttpPost]
        public IActionResult XuLyThanhToan(string TenNguoiNhan, string SDTNguoiNhan,
            string DiaChiGiao, string? GhiChu, string PhuongThucThanhToan)
        {
            var gioHang = GioHang();
            if (gioHang.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            // Tính tổng tiền
            var tongTienSP = gioHang.Sum(p => p.ThanhTien);
            var phiVanChuyen = 30000m;
            var tongCong = tongTienSP + phiVanChuyen;

            // Tạo mã đơn hàng mới
            var lastOrder = _context.DonHangs.OrderByDescending(d => d.MaDonHang).FirstOrDefault();
            var newOrderNumber = 1;
            if (lastOrder != null)
            {
                var lastNumber = int.Parse(lastOrder.MaDonHang.Replace("DH", ""));
                newOrderNumber = lastNumber + 1;
            }
            var maDonHang = $"DH{newOrderNumber:D7}";
            var soDonHang = $"ORD{newOrderNumber:D4}";

            // Tạo đơn hàng mới
            var donHang = new DonHang
            {
                MaDonHang = maDonHang,
                SoDonHang = soDonHang,
                MaKh = "KH000001", // Khách mặc định (cần sửa khi có đăng nhập)
                NgayDat = DateTime.Now,
                TrangThaiDon = "Chờ xác nhận",
                TrangThaiThanhToan = PhuongThucThanhToan == "COD" ? "Chưa thanh toán" : "Đang xử lý",
                MaDvvc = "DVVC01",
                PhiVanChuyen = phiVanChuyen,
                GiamGiaKm = 0,
                GiamGiaKcoin = 0,
                TongTienHang = tongTienSP,
                TongGiamGia = 0,
                TongThanhToan = tongCong,
                Vat = 0,
                TongSauVat = tongCong,
                TenNguoiNhan = TenNguoiNhan,
                SdtnguoiNhan = SDTNguoiNhan,
                DiaChiGiao = DiaChiGiao,
                GhiChu = GhiChu
            };

            _context.DonHangs.Add(donHang);

            // Thêm chi tiết đơn hàng
            foreach (var item in gioHang)
            {
                // Lấy biến thể đầu tiên của sản phẩm (nếu có)
                var bienThe = _context.BienTheSanPhams.FirstOrDefault(bt => bt.MaSp == item.MaSp);

                if (bienThe != null)
                {
                    var chiTiet = new ChiTietDonHang
                    {
                        MaDonHang = maDonHang,
                        MaBienThe = bienThe.MaBienThe,
                        Sku = bienThe.Sku ?? item.MaSp,
                        TenSpHienThi = item.TenSp,
                        DonGia = item.DonGiaBan,
                        SoLuong = item.SoLuong,
                        TyLeGiam = 0,
                        ThanhTien = item.ThanhTien
                    };
                    _context.ChiTietDonHangs.Add(chiTiet);
                }
            }

            _context.SaveChanges();

            // Xóa giỏ hàng
            HttpContext.Session.Remove(MySetting.GIOHANG_KEY);

            // Lưu thông tin đơn hàng vào TempData để hiển thị
            TempData["MaDonHang"] = maDonHang;
            TempData["MaTraCuu"] = soDonHang;
            TempData["TongCong"] = tongCong.ToString("N0");

            return RedirectToAction("ThanhToanThanhCong");
        }

        public IActionResult ThanhToanThanhCong()
        {
            return View();
        }

    }
}
