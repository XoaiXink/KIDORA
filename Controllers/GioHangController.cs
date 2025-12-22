using KIDORA.Data;
using KIDORA.Helpers;
using KIDORA.Models.Services;
using KIDORA.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KIDORA.Controllers
{
    public class GioHangController : Controller
    {
        private readonly KidoraDbContext _context;
        private readonly IVnPayServices _vnPayservice;

        public GioHangController(KidoraDbContext context, IVnPayServices vnPayservice)
        {
            _context = context;
            _vnPayservice = vnPayservice;
        }

        public List<SanPhamGioHang> GioHang() =>
            HttpContext.Session.GetT<List<SanPhamGioHang>>(MySetting.GIOHANG_KEY)
            ?? new List<SanPhamGioHang>();

        public IActionResult Index()
        {
            return View(GioHang());
        }

        public IActionResult AddToCart(string id, int quantity = 1)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("AddToCart", new { id, quantity }) });
            }

            var gioHang = GioHang();
            var sanPham = gioHang.FirstOrDefault(p => p.MaSp == id);

            if (sanPham == null)
            {
                var sp = _context.SanPhams.FirstOrDefault(p => p.MaSp == id);
                if (sp == null)
                {
                    TempData["ErrorMessage"] = $"Không tìm thấy sản phẩm với mã {id}.";
                    return Redirect("/404");
                }

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
                sanPham.SoLuong += quantity;
            }

            HttpContext.Session.Set(MySetting.GIOHANG_KEY, gioHang);
            TempData["Success"] = "Đã thêm vào giỏ!";
            return RedirectToAction("Index", "GioHang");
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

        [HttpGet]
        public IActionResult ThanhToan()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("ThanhToan") });
            }

            var gioHang = GioHang();
            if (gioHang.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            var model = new ThanhToanVM
            {
                GioHang = gioHang,
            };

            var currentUserId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(currentUserId))
            {
                var diaChis = _context.DiaChiKhachHangs
                    .Where(d => d.MaKh == currentUserId)
                    .OrderByDescending(d => d.MacDinh)
                    .ToList();

                model.DiaChiDaLuu = diaChis;

                var defaultAddr = diaChis.FirstOrDefault(d => d.MacDinh) ?? diaChis.FirstOrDefault();
                if (defaultAddr != null)
                {
                    model.HoTen = defaultAddr.TenNguoiNhan;
                    model.DienThoai = defaultAddr.DienThoaiNhan;
                    model.DiaChi = defaultAddr.DiaChiChiTiet;
                    model.TinhThanh = defaultAddr.TinhThanh;
                    model.QuanHuyen = defaultAddr.QuanHuyen;
                    model.PhuongXa = defaultAddr.PhuongXa;
                }
                else
                {
                    var nguoiDung = _context.NguoiDungs.FirstOrDefault(n => n.Id == currentUserId);
                    if (nguoiDung != null)
                    {
                        model.HoTen = nguoiDung.HoTen;
                        model.DienThoai = nguoiDung.DienThoai;
                    }
                }
            }

            // ✅ Lấy mã giảm giá từ Session nếu có
            var savedDiscount = HttpContext.Session.GetT<DiscountInfo>("AppliedDiscount");
            if (savedDiscount != null)
            {
                model.MaGiamGia = savedDiscount.MaCode;
                model.SoTienGiam = savedDiscount.SoTienGiam;
            }

            return View(model);
        }

        // ✅ ACTION MỚI: Áp dụng mã giảm giá
        [HttpPost]
        public IActionResult ApDungMaGiamGia(ThanhToanVM model)
        {
            var gioHang = GioHang();
            if (gioHang.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            var currentUserId = User?.FindFirstValue(ClaimTypes.NameIdentifier);

            // Xử lý địa chỉ đã lưu nếu có
            var savedAddressIdStr = Request.Form["savedAddress"].FirstOrDefault();
            if (!string.IsNullOrEmpty(savedAddressIdStr) && int.TryParse(savedAddressIdStr, out int maDiaChi))
            {
                var dc = _context.DiaChiKhachHangs.FirstOrDefault(d => d.MaDiaChi == maDiaChi);
                if (dc != null)
                {
                    model.HoTen = dc.TenNguoiNhan ?? "";
                    model.DienThoai = dc.DienThoaiNhan ?? "";
                    model.DiaChi = dc.DiaChiChiTiet ?? "";
                    model.TinhThanh = dc.TinhThanh;
                    model.QuanHuyen = dc.QuanHuyen;
                    model.PhuongXa = dc.PhuongXa;
                }
            }

            var tongTienSP = gioHang.Sum(p => p.ThanhTien);
            decimal soTienGiam = 0;

            if (!string.IsNullOrWhiteSpace(model.MaGiamGia))
            {
                var maCode = model.MaGiamGia.Trim();
                var today = DateOnly.FromDateTime(DateTime.Now);

                var maKhuyenMai = _context.MaKhuyenMais.FirstOrDefault(k =>
                    k.MaCode.ToLower() == maCode.ToLower());

                var hopLe =
                    maKhuyenMai != null &&
                    maKhuyenMai.DangHoatDong &&
                    today >= maKhuyenMai.NgayBatDau &&
                    today <= maKhuyenMai.NgayKetThuc &&
                    maKhuyenMai.SoLanDaDung < maKhuyenMai.GioiHanLuotDung &&
                    tongTienSP >= maKhuyenMai.DonToiThieu;

                if (!hopLe)
                {
                    TempData["ErrorMessage"] = "Mã giảm giá không hợp lệ hoặc không đáp ứng điều kiện.";
                    HttpContext.Session.Remove("AppliedDiscount");
                }
                else
                {
                    var kieu = maKhuyenMai.KieuGiam?.ToLower() ?? string.Empty;
                    if (kieu.Contains("phantram") || kieu.Contains("percent") || kieu.Contains("%"))
                    {
                        soTienGiam = Math.Round(tongTienSP * maKhuyenMai.GiaTriGiam / 100m, 0);
                    }
                    else
                    {
                        soTienGiam = maKhuyenMai.GiaTriGiam;
                    }

                    if (soTienGiam > tongTienSP)
                    {
                        soTienGiam = tongTienSP;
                    }

                    // ✅ Lưu vào Session
                    HttpContext.Session.Set("AppliedDiscount", new DiscountInfo
                    {
                        MaCode = maCode,
                        MaKm = maKhuyenMai.MaKm,
                        SoTienGiam = soTienGiam
                    });

                    TempData["SuccessMessage"] = $"Áp dụng mã thành công! Giảm {soTienGiam:N0}đ";
                }
            }
            else
            {
                HttpContext.Session.Remove("AppliedDiscount");
            }

            return RedirectToAction("ThanhToan");
        }

        [HttpPost]
        public IActionResult ThanhToan(ThanhToanVM model, string payment = "COD")
        {
            var gioHang = GioHang();
            if (gioHang.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            var currentUserId = User?.FindFirstValue(ClaimTypes.NameIdentifier);

            var savedAddressIdStr = Request.Form["savedAddress"].FirstOrDefault();
            if (!string.IsNullOrEmpty(savedAddressIdStr) && int.TryParse(savedAddressIdStr, out int maDiaChi))
            {
                var dc = _context.DiaChiKhachHangs.FirstOrDefault(d => d.MaDiaChi == maDiaChi);
                if (dc != null)
                {
                    model.HoTen = dc.TenNguoiNhan ?? "";
                    model.DienThoai = dc.DienThoaiNhan ?? "";
                    model.DiaChi = dc.DiaChiChiTiet ?? "";
                    model.TinhThanh = dc.TinhThanh;
                    model.QuanHuyen = dc.QuanHuyen;
                    model.PhuongXa = dc.PhuongXa;
                }
            }

            if (!ModelState.IsValid)
            {
                model.GioHang = gioHang;
                if (!string.IsNullOrEmpty(currentUserId))
                {
                    model.DiaChiDaLuu = _context.DiaChiKhachHangs
                        .Where(d => d.MaKh == currentUserId)
                        .OrderByDescending(d => d.MacDinh)
                        .ToList();
                }
                return View(model);
            }

            var tongTienSP = gioHang.Sum(p => p.ThanhTien);
            var phiVanChuyen = 30000m;
            var tongCongTruocGiam = tongTienSP + phiVanChuyen;

            // ✅ Lấy mã giảm giá từ Session
            decimal soTienGiam = 0;
            MaKhuyenMai? maKhuyenMai = null;
            var savedDiscount = HttpContext.Session.GetT<DiscountInfo>("AppliedDiscount");

            if (savedDiscount != null)
            {
                maKhuyenMai = _context.MaKhuyenMais.FirstOrDefault(k => k.MaKm == savedDiscount.MaKm);
                if (maKhuyenMai != null)
                {
                    soTienGiam = savedDiscount.SoTienGiam;
                }
            }

            var tongCongSauGiam = Math.Max(0, tongCongTruocGiam - soTienGiam);

            // Tạo mã đơn hàng
            var lastOrder = _context.DonHangs.OrderByDescending(d => d.MaDonHang).FirstOrDefault();
            var newOrderNumber = 1;
            if (lastOrder != null && int.TryParse(lastOrder.MaDonHang.Replace("DH", ""), out int lastNumber))
            {
                newOrderNumber = lastNumber + 1;
            }
            var maDonHang = $"DH{newOrderNumber:D7}";
            var soDonHang = $"ORD{newOrderNumber:D4}";

            var diaChiDayDu = $"{model.DiaChi}, {model.PhuongXa}, {model.QuanHuyen}, {model.TinhThanh}".Trim(',', ' ');

            var donHang = new DonHang
            {
                MaDonHang = maDonHang,
                SoDonHang = soDonHang,
                MaKh = string.IsNullOrEmpty(currentUserId) ? "KH000001" : currentUserId,
                NgayDat = DateTime.Now,
                TrangThaiDon = "Chờ xác nhận",
                TrangThaiThanhToan = "Chưa thanh toán",
                MaDvvc = "DVVC01",
                PhiVanChuyen = phiVanChuyen,
                GiamGiaKm = soTienGiam,
                GiamGiaKcoin = 0,
                TongTienHang = tongTienSP,
                TongGiamGia = soTienGiam,
                TongThanhToan = tongCongSauGiam,
                Vat = 0,
                TongSauVat = tongCongSauGiam,
                TenNguoiNhan = model.HoTen,
                SdtnguoiNhan = model.DienThoai,
                DiaChiGiao = diaChiDayDu,
                GhiChu = model.GhiChu ?? string.Empty
            };

            _context.DonHangs.Add(donHang);
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.SaveChanges();

                if (maKhuyenMai != null && soTienGiam > 0)
                {
                    var nextKmDonId = (_context.KmDonHangs.Select(k => (int?)k.MaKmDon).Max() ?? 0) + 1;
                    var kmDonHang = new KmDonHang
                    {
                        MaKmDon = nextKmDonId,
                        MaDonHang = maDonHang,
                        MaKm = maKhuyenMai.MaKm,
                        SoTienGiam = soTienGiam,
                        GhiChu = $"Áp mã {maKhuyenMai.MaCode}"
                    };

                    maKhuyenMai.SoLanDaDung += 1;
                    _context.KmDonHangs.Add(kmDonHang);
                    _context.MaKhuyenMais.Update(maKhuyenMai);
                }

                var nextCtId = (_context.ChiTietDonHangs.Select(c => (int?)c.MaCtdh).Max() ?? 0) + 1;

                foreach (var item in gioHang)
                {
                    var bienThe = _context.BienTheSanPhams.FirstOrDefault(bt => bt.MaSp == item.MaSp);

                    if (bienThe == null)
                    {
                        var sp = _context.SanPhams.FirstOrDefault(s => s.MaSp == item.MaSp);
                        var newBienThe = new BienTheSanPham
                        {
                            MaBienThe = GenerateBienTheId(),
                            MaSp = item.MaSp,
                            Sku = sp?.Sku ?? $"{item.MaSp}-DEF",
                            TenBienThe = "Mặc định",
                            DangBan = true
                        };
                        _context.BienTheSanPhams.Add(newBienThe);
                        bienThe = newBienThe;
                    }

                    var chiTiet = new ChiTietDonHang
                    {
                        MaCtdh = nextCtId++,
                        MaDonHang = maDonHang,
                        MaBienThe = bienThe.MaBienThe,
                        Sku = bienThe.Sku,
                        TenSpHienThi = item.TenSp,
                        DonGia = item.DonGiaBan,
                        SoLuong = item.SoLuong,
                        TyLeGiam = 0,
                        ThanhTien = item.ThanhTien
                    };

                    _context.ChiTietDonHangs.Add(chiTiet);
                }

                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                try { transaction.Rollback(); } catch { }
                TempData["ErrorMessage"] = "Lỗi khi lưu đơn hàng: " + ex.Message;
                return RedirectToAction("Index");
            }

            // ✅ Xóa mã giảm giá khỏi Session
            HttpContext.Session.Remove("AppliedDiscount");

            if (payment == "Thanh toán VNPay")
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = (decimal)(double)tongCongSauGiam,
                    CreateDate = DateTime.Now,
                    Description = $"Thanh toan don hang {maDonHang}",
                    FullName = model.HoTen,
                    OrderId = new Random().Next(1000, 100000)
                };
                HttpContext.Session.SetString("PendingOrderId", maDonHang);
                return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
            }

            HttpContext.Session.Remove(MySetting.GIOHANG_KEY);
            TempData["MaDonHang"] = maDonHang;
            TempData["MaTraCuu"] = soDonHang;
            TempData["TongCong"] = tongCongSauGiam.ToString("N0");

            return RedirectToAction("ThanhToanThanhCong");
        }

        private string GenerateBienTheId()
        {
            for (int attempt = 0; attempt < 5; attempt++)
            {
                var bytes = new byte[4];
                System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
                var id = "BT" + BitConverter.ToString(bytes).Replace("-", "").ToUpperInvariant();

                if (!_context.BienTheSanPhams.Any(b => b.MaBienThe == id))
                {
                    return id;
                }
            }
            return "BT" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();
        }

        public IActionResult ThanhToanThanhCong()
        {
            return View();
        }

        [Authorize]
        public IActionResult PaymentCallback()
        {
            var response = _vnPayservice.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                var pendingOrderId = HttpContext.Session.GetString("PendingOrderId");
                if (!string.IsNullOrEmpty(pendingOrderId))
                {
                    var order = _context.DonHangs.FirstOrDefault(d => d.MaDonHang == pendingOrderId);
                    if (order != null)
                    {
                        order.TrangThaiDon = "Đã hủy";
                        order.TrangThaiThanhToan = "Thanh toán thất bại";
                        _context.SaveChanges();
                    }
                    HttpContext.Session.Remove("PendingOrderId");
                }

                TempData["Message"] = "Thanh toán VNPay thất bại";
                return RedirectToAction("ThanhToanThatBai");
            }

            var pendingOrderIdSuccess = HttpContext.Session.GetString("PendingOrderId");
            if (!string.IsNullOrEmpty(pendingOrderIdSuccess))
            {
                var order = _context.DonHangs.FirstOrDefault(d => d.MaDonHang == pendingOrderIdSuccess);
                if (order != null)
                {
                    order.TrangThaiThanhToan = "Đã thanh toán";
                    var thanhToan = new ThanhToan
                    {
                        MaTt = new Random().Next(100000, 999999),
                        MaDonHang = order.MaDonHang,
                        PhuongThuc = "VNPay",
                        TrangThai = "Thành công",
                        SoTienThanhToan = order.TongThanhToan,
                        NgayThanhToan = DateTime.Now,
                        MaGiaoDich = response.TransactionId
                    };
                    _context.ThanhToans.Add(thanhToan);
                    _context.SaveChanges();

                    TempData["MaDonHang"] = order.MaDonHang;
                    TempData["MaTraCuu"] = order.SoDonHang;
                    TempData["TongCong"] = order.TongThanhToan.ToString("N0");
                }
                HttpContext.Session.Remove("PendingOrderId");
            }

            TempData["Message"] = "Thanh toán VNPay thành công";
            HttpContext.Session.Remove(MySetting.GIOHANG_KEY);

            return RedirectToAction("ThanhToanThanhCong");
        }

        [Authorize]
        public IActionResult ThanhToanThatBai()
        {
            return View();
        }
    }

    // ✅ Class helper để lưu thông tin mã giảm giá
    public class DiscountInfo
    {
        public string MaCode { get; set; }
        public string MaKm { get; set; }
        public decimal SoTienGiam { get; set; }
    }
}