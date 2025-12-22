
using KIDORA.Data;
using System.ComponentModel.DataAnnotations;

namespace KIDORA.ViewModels
{
    public class ThanhToanVM
    {
        public List<SanPhamGioHang> GioHang { get; set; } = new();
        // Thông tin người nhận hàng bắt buộc phải có
        public bool GiongKhachHang { get; set; } = true;

        [Display(Name = "Họ tên người nhận")]
        [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận")]
        public string HoTen { get; set; }

        [Display(Name = "Địa chỉ nhận hàng")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng")]
        public string DiaChi { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string DienThoai { get; set; }

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        public string? TinhThanh { get; set; }
        public string? QuanHuyen { get; set; }
        public string? PhuongXa { get; set; }
        public List<DiaChiKhachHang> DiaChiDaLuu { get; set; } = new();
        [Display(Name = "Mã giảm giá")]
        public string? MaGiamGia { get; set; }
        public decimal SoTienGiam { get; set; }
    }
}
