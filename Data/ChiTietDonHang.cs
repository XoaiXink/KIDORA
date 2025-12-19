using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KIDORA.Data;

public partial class ChiTietDonHang
{
    public int MaCtdh { get; set; }

    public string MaDonHang { get; set; } = null!;

    public string MaBienThe { get; set; } = null!;

    public string Sku { get; set; } = null!;
    [Column("TenSP_HienThi")]

    public string TenSpHienThi { get; set; } = null!;

    public decimal DonGia { get; set; }

    public int SoLuong { get; set; }

    public decimal TyLeGiam { get; set; }

    public decimal ThanhTien { get; set; }

    public virtual BienTheSanPham MaBienTheNavigation { get; set; } = null!;

    public virtual DonHang MaDonHangNavigation { get; set; } = null!;
}


