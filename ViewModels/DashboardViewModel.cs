namespace KIDORA.ViewModels
{
    public class DashboardViewModel
    {
        // Thống kê tổng quan
        public decimal DoanhThuHomNay { get; set; }
        public decimal DoanhThuThang { get; set; }
        public int TongDonHang { get; set; }
        public int TongKhachHang { get; set; }
        public int TongSanPham { get; set; }

        // Đơn hàng mới nhất
        public List<DonHangMoiViewModel> DonHangMoi { get; set; } = new();

        // Sản phẩm bán chạy
        public List<SanPhamBanChayViewModel> SanPhamBanChay { get; set; } = new();

        // Doanh thu theo tháng (12 tháng gần nhất)
        public List<DoanhThuThangViewModel> DoanhThuTheoThang { get; set; } = new();

        // Thống kê trạng thái đơn hàng
        public Dictionary<string, int> ThongKeTrangThai { get; set; } = new();

        // Đơn hàng chờ xử lý
        public int DonChoXuLy { get; set; }
        public int DonDangGiao { get; set; }
        public int DonHoanThanh { get; set; }
    }

    public class DonHangMoiViewModel
    {
        public string MaDonHang { get; set; } = string.Empty;
        public string TenKhachHang { get; set; } = string.Empty;
        public decimal TongTien { get; set; }
        public DateTime NgayDat { get; set; }
        public string TrangThai { get; set; } = string.Empty;
    }

    public class SanPhamBanChayViewModel
    {
        public string MaSP { get; set; } = string.Empty;
        public string TenSP { get; set; } = string.Empty;
        public string AnhChinh { get; set; } = string.Empty;
        public int SoLuongBan { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class DoanhThuThangViewModel
    {
        public string Thang { get; set; } = string.Empty;
        public decimal DoanhThu { get; set; }
        public int SoDon { get; set; }
    }
}

