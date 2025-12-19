
using KIDORA.Data;
using KIDORA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KIDORA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly KidoraDbContext _context;

        public HomeController(KidoraDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            // Thống kê tổng quan
            var doanhThuHomNay = await _context.DonHangs
                .Where(d => d.NgayDat.Date == today)
                .SumAsync(d => d.TongThanhToan);

            var doanhThuThang = await _context.DonHangs
                .Where(d => d.NgayDat >= startOfMonth)
                .SumAsync(d => d.TongThanhToan);

            var tongDonHang = await _context.DonHangs.CountAsync();
            var tongKhachHang = await _context.KhachHangs.CountAsync();
            var tongSanPham = await _context.SanPhams.CountAsync();

            // Đơn hàng theo trạng thái
            var donChoXuLy = await _context.DonHangs.CountAsync(d => d.TrangThaiDon == "Chờ xác nhận" || d.TrangThaiDon == "Đang xử lý");
            var donDangGiao = await _context.DonHangs.CountAsync(d => d.TrangThaiDon == "Đang giao");
            var donHoanThanh = await _context.DonHangs.CountAsync(d => d.TrangThaiDon == "Hoàn thành");

            // 5 đơn hàng mới nhất
            var donHangMoi = await _context.DonHangs
                .OrderByDescending(d => d.NgayDat)
                .ThenByDescending(d => d.MaDonHang)
                .Take(5)
                .Select(d => new DonHangMoiViewModel
                {
                    MaDonHang = d.MaDonHang,
                    TenKhachHang = d.TenNguoiNhan ?? "N/A",
                    TongTien = d.TongThanhToan,
                    NgayDat = d.NgayDat,
                    TrangThai = d.TrangThaiDon
                })
                .ToListAsync();

            // Top 5 sản phẩm bán chạy
            var sanPhamBanChay = await _context.ChiTietDonHangs
                .Include(ct => ct.MaBienTheNavigation)
                    .ThenInclude(bt => bt.MaSpNavigation)
                .GroupBy(ct => ct.MaBienTheNavigation.MaSp)
                .Select(g => new SanPhamBanChayViewModel
                {
                    MaSP = g.Key ?? "",
                    TenSP = g.First().MaBienTheNavigation.MaSpNavigation!.TenSp ?? "",
                    AnhChinh = g.First().MaBienTheNavigation.MaSpNavigation!.AnhChinh ?? "",
                    SoLuongBan = g.Sum(x => x.SoLuong),
                    DoanhThu = g.Sum(x => x.ThanhTien)
                })
                .OrderByDescending(x => x.SoLuongBan)
                .Take(5)
                .ToListAsync();

            // Doanh thu 6 tháng gần nhất
            var doanhThuTheoThang = new List<DoanhThuThangViewModel>();
            for (int i = 5; i >= 0; i--)
            {
                var thang = today.AddMonths(-i);
                var startDate = new DateTime(thang.Year, thang.Month, 1);
                var endDate = startDate.AddMonths(1);

                var doanhThu = await _context.DonHangs
                    .Where(d => d.NgayDat >= startDate && d.NgayDat < endDate)
                    .SumAsync(d => d.TongThanhToan);

                var soDon = await _context.DonHangs
                    .Where(d => d.NgayDat >= startDate && d.NgayDat < endDate)
                    .CountAsync();

                doanhThuTheoThang.Add(new DoanhThuThangViewModel
                {
                    Thang = thang.ToString("MM/yyyy"),
                    DoanhThu = doanhThu,
                    SoDon = soDon
                });
            }

            // Thống kê trạng thái
            var thongKeTrangThai = await _context.DonHangs
                .GroupBy(d => d.TrangThaiDon)
                .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
                .ToDictionaryAsync(x => x.TrangThai, x => x.SoLuong);

            var viewModel = new DashboardViewModel
            {
                DoanhThuHomNay = doanhThuHomNay,
                DoanhThuThang = doanhThuThang,
                TongDonHang = tongDonHang,
                TongKhachHang = tongKhachHang,
                TongSanPham = tongSanPham,
                DonChoXuLy = donChoXuLy,
                DonDangGiao = donDangGiao,
                DonHoanThanh = donHoanThanh,
                DonHangMoi = donHangMoi,
                SanPhamBanChay = sanPhamBanChay,
                DoanhThuTheoThang = doanhThuTheoThang,
                ThongKeTrangThai = thongKeTrangThai
            };

            return View(viewModel);
        }

        [Area("Admin")]
        public IActionResult Profile()
        {
            return View();
        }
    }
}

