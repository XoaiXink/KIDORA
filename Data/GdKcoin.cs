using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class GdKcoin
{
    public int MaGdKcoin { get; set; }

    public string MaKh { get; set; } = null!;

    public string? MaDonHang { get; set; }

    public string KieuGd { get; set; } = null!;

    public int SoKcoin { get; set; }

    public int SoDuSauGd { get; set; }

    public string? MoTa { get; set; }

    public DateTime ThoiGianGd { get; set; }

    public virtual DonHang? MaDonHangNavigation { get; set; }

    public virtual KhachHang MaKhNavigation { get; set; } = null!;
}
