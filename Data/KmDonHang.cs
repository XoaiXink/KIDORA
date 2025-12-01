using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class KmDonHang
{
    public int MaKmDon { get; set; }

    public string MaDonHang { get; set; } = null!;

    public string MaKm { get; set; } = null!;

    public decimal SoTienGiam { get; set; }

    public string? GhiChu { get; set; }

    public virtual DonHang MaDonHangNavigation { get; set; } = null!;

    public virtual MaKhuyenMai MaKmNavigation { get; set; } = null!;
}
