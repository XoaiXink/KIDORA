using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class SanPham
{
    public string MaSp { get; set; } = null!;

    public string Sku { get; set; } = null!;

    public string TenSp { get; set; } = null!;

    public string? MoTaNgan { get; set; }

    public string? MoTaChiTiet { get; set; }

    public string MaDanhMuc { get; set; } = null!;

    public string MaNcc { get; set; } = null!;

    public string? ThuongHieu { get; set; }

    public int? DoTuoiToiThieu { get; set; }

    public int? DoTuoiToiDa { get; set; }

    public decimal DonGiaBan { get; set; }

    public decimal GiaNiemYet { get; set; }

    public decimal? KhoiLuong { get; set; }

    public string? AnhChinh { get; set; }

    public bool DangBan { get; set; }

    public int SoLuongTon { get; set; }

    public virtual ICollection<AnhSanPham> AnhSanPhams { get; set; } = new List<AnhSanPham>();

    public virtual ICollection<BienTheSanPham> BienTheSanPhams { get; set; } = new List<BienTheSanPham>();

    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

    public virtual ICollection<DanhGiaSanPham> DanhGiaSanPhams { get; set; } = new List<DanhGiaSanPham>();

    public virtual DanhMuc MaDanhMucNavigation { get; set; } = null!;

    public virtual NhaCungCap MaNccNavigation { get; set; } = null!;

    public virtual ICollection<WishlistChiTiet> WishlistChiTiets { get; set; } = new List<WishlistChiTiet>();
}
