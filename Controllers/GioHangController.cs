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

        // Thêm sản phẩm vào giỏ (yêu cầu đăng nhập)
        public IActionResult AddToCart(string id, int quantity = 1)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("AddToCart", new { id, quantity }) });
            }

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
            // Sau khi thêm, chuyển hướng đến trang giỏ hàng để người dùng có thể tiến hành mua
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
            if (GioHang().Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }
            var model = new ThanhToanVM
            {
                GioHang = gioHang,
                // Tài khoản demo nếu có đăng nhập thì load thông tin từ DB
            };
            return View(model);
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

            if (!ModelState.IsValid)
            {
                model.GioHang = gioHang;
                return View(model);
            }

            // Tính tổng tiền
            var tongTienSP = gioHang.Sum(p => p.ThanhTien);
            var phiVanChuyen = 30000m;
            var tongCong = tongTienSP + phiVanChuyen;

            // Tạo mã đơn hàng mới
            // NOTE: Cần lock hoặc dùng sequence DB để tránh trùng lặp khi chạy concurrent
            var lastOrder = _context.DonHangs.OrderByDescending(d => d.MaDonHang).FirstOrDefault();
            var newOrderNumber = 1;
            if (lastOrder != null)
            {
                // Giả định mã luôn là DHxxxxxxx (7 số)
                if (int.TryParse(lastOrder.MaDonHang.Replace("DH", ""), out int lastNumber))
                {
                    newOrderNumber = lastNumber + 1;
                }
            }
            var maDonHang = $"DH{newOrderNumber:D7}";
            var soDonHang = $"ORD{newOrderNumber:D4}";

            // Map địa chỉ đầy đủ
            var diaChiDayDu = $"{model.DiaChi}, {model.PhuongXa}, {model.QuanHuyen}, {model.TinhThanh}".Trim(',', ' ');

            // Tạo đơn hàng mới
            // Lấy MaKh từ user đang đăng nhập nếu có để liên kết đơn hàng với khách.
            var currentUserId = User?.FindFirstValue(ClaimTypes.NameIdentifier);

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
                GiamGiaKm = 0,
                GiamGiaKcoin = 0,
                TongTienHang = tongTienSP,
                TongGiamGia = 0,
                TongThanhToan = tongCong,
                Vat = 0,
                TongSauVat = tongCong,
                TenNguoiNhan = model.HoTen,
                SdtnguoiNhan = model.DienThoai,
                DiaChiGiao = diaChiDayDu,
                GhiChu = model.GhiChu
            };

            _context.DonHangs.Add(donHang);

            // Use a transaction: save order first, then add details to avoid tracking/key conflicts
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.SaveChanges();

                // Thêm chi tiết đơn hàng — đảm bảo luôn thêm, tạo biến thể mặc định nếu cần
                // Prepare starting ID for ChiTietDonHang since DB model uses non-generated PK
                var nextCtId = (_context.ChiTietDonHangs.Select(c => (int?)c.MaCtdh).Max() ?? 0) + 1;

                foreach (var item in gioHang)
                {
                    // Tìm biến thể theo MaSp
                    var bienThe = _context.BienTheSanPhams.FirstOrDefault(bt => bt.MaSp == item.MaSp);

                    if (bienThe == null)
                    {
                        // Nếu không có, tạo biến thể mặc định tạm thời để tránh bỏ qua chi tiết
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
                // Rollback and show error
                try { transaction.Rollback(); } catch { }
                TempData["ErrorMessage"] = "Lỗi khi lưu đơn hàng: " + ex.Message;
                return RedirectToAction("Index");
            }

            // Xử lý thanh toán
            if (payment == "Thanh toán VNPay")
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = (decimal)(double)tongCong,
                    CreateDate = DateTime.Now,
                    Description = $"Thanh toan don hang {maDonHang}",
                    FullName = model.HoTen,
                    OrderId = new Random().Next(1000, 100000) // VNPay yêu cầu số int cho TxnRef? Kiểm tra lại service
                                                              // Trong VnPayService: vnpay.AddRequestData("vnp_TxnRef", tick); // Nó dùng ticks!
                                                              // Nhưng PaymentCallback lại đọc: var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                                                              // Logic cũ dùng tick làm TxnRef. Điều này làm mất liên kết với MaDonHang của mình.
                                                              // Chúng ta nên gửi MaDonHang của mình đi, HOẶC lưu TxnRef vào đơn hàng.
                                                              // Cách tốt nhất: Lưu MaDonHang vào vnp_TxnRef (nếu là số) hoặc vnp_OrderInfo.
                                                              // Tuy nhiên VnPayService đang cứng nhắc dùng DateTime.Now.Ticks cho TxnRef.
                                                              // Để đơn giản, ta sẽ lưu lại MaDonHang vào Session để map lại khi Callback về.
                };

                // HACK: Để map lại Transaction với Order, ta có thể lưu vào NguoiDung session hoặc sửa Service.
                // Ở đây tôi sẽ lưu vào Description hoặc sửa VnPayService để nhận TxnRef từ ngoài. 
                // Nhưng VnPayService đang code cứng check Ticks.
                // Giải pháp tạm thời: Lưu Session MaDonHangVuaTao
                HttpContext.Session.SetString("PendingOrderId", maDonHang);

                return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
            }

            // Mặc định COD
            HttpContext.Session.Remove(MySetting.GIOHANG_KEY);
            TempData["MaDonHang"] = maDonHang;
            TempData["MaTraCuu"] = soDonHang;
            TempData["TongCong"] = tongCong.ToString("N0");

            return RedirectToAction("ThanhToanThanhCong");
        }

        private string GenerateBienTheId()
        {
            // Generate BT + 8 hex chars (10 chars total), retry if collision detected.
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

            // Fallback: GUID-based (still 8 chars) to reduce collision chance further
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
                // Cập nhật trạng thái đơn hàng là Thất bại / Hủy
                var pendingOrderId = HttpContext.Session.GetString("PendingOrderId");
                if (!string.IsNullOrEmpty(pendingOrderId))
                {
                    var order = _context.DonHangs.FirstOrDefault(d => d.MaDonHang == pendingOrderId);
                    if (order != null)
                    {
                        order.TrangThaiDon = "Đã hủy"; // Hoặc trạng thái thất bại
                        order.TrangThaiThanhToan = "Thanh toán thất bại";
                        _context.SaveChanges();
                    }
                    HttpContext.Session.Remove("PendingOrderId");
                }

                TempData["Message"] = "Thanh toán VNPay thất bại";
                return RedirectToAction("ThanhToanThatBai");
            }

            // Thanh toán thành công
            var pendingOrderIdSuccess = HttpContext.Session.GetString("PendingOrderId");
            if (!string.IsNullOrEmpty(pendingOrderIdSuccess))
            {
                var order = _context.DonHangs.FirstOrDefault(d => d.MaDonHang == pendingOrderIdSuccess);
                if (order != null)
                {
                    order.TrangThaiThanhToan = "Đã thanh toán";
                    // Thêm record vào bảng THANH_TOAN
                    var thanhToan = new ThanhToan
                    {
                        MaTt = new Random().Next(100000, 999999), // Tạm thời random
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
}

