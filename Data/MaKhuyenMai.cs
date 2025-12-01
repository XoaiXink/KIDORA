using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class MaKhuyenMai
{
    public string MaKm { get; set; } = null!;

    public string MaCode { get; set; } = null!;

    public string TenKm { get; set; } = null!;

    public string? MoTa { get; set; }

    public string KieuGiam { get; set; } = null!;

    public decimal GiaTriGiam { get; set; }

    public decimal DonToiThieu { get; set; }

    public DateOnly NgayBatDau { get; set; }

    public DateOnly NgayKetThuc { get; set; }

    public int GioiHanLuotDung { get; set; }

    public int SoLanDaDung { get; set; }

    public bool DangHoatDong { get; set; }

    public virtual ICollection<KmDonHang> KmDonHangs { get; set; } = new List<KmDonHang>();
}
