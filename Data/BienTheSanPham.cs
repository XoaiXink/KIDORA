using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class BienTheSanPham
{
    public string MaBienThe { get; set; } = null!;

    public string MaSp { get; set; } = null!;

    public string Sku { get; set; } = null!;

    public string TenBienThe { get; set; } = null!;

    public bool DangBan { get; set; }

    public virtual BienTheGium? BienTheGium { get; set; }

    public virtual ICollection<BienTheHinh> BienTheHinhs { get; set; } = new List<BienTheHinh>();

    public virtual BienTheTonKho? BienTheTonKho { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

    public virtual SanPham MaSpNavigation { get; set; } = null!;

    public virtual ICollection<WishlistChiTiet> WishlistChiTiets { get; set; } = new List<WishlistChiTiet>();

    public virtual ICollection<GiaTriThuocTinh> MaGiaTris { get; set; } = new List<GiaTriThuocTinh>();
}
