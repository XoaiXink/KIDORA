using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class ChiTietDonHang
{
    public int MaCtdh { get; set; }

    public string MaDonHang { get; set; } = null!;

    public string MaBienThe { get; set; } = null!;

    public string Sku { get; set; } = null!;

    public string TenSpHienThi { get; set; } = null!;

    public decimal DonGia { get; set; }

    public int SoLuong { get; set; }

    public decimal TyLeGiam { get; set; }

    public decimal ThanhTien { get; set; }

    public virtual BienTheSanPham MaBienTheNavigation { get; set; } = null!;

    public virtual DonHang MaDonHangNavigation { get; set; } = null!;
}
