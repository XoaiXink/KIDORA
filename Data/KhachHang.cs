using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class KhachHang
{
    public string MaKh { get; set; } = null!;

    public string MaHang { get; set; } = null!;

    public int SoDuKcoin { get; set; }

    public DateTime NgayThamGia { get; set; }

    public virtual ICollection<DanhGiaSanPham> DanhGiaSanPhams { get; set; } = new List<DanhGiaSanPham>();

    public virtual ICollection<DiaChiKhachHang> DiaChiKhachHangs { get; set; } = new List<DiaChiKhachHang>();

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<GdKcoin> GdKcoins { get; set; } = new List<GdKcoin>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual HangThanhVien MaHangNavigation { get; set; } = null!;

    public virtual NguoiDung MaKhNavigation { get; set; } = null!;

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
