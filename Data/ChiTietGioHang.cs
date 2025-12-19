using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class ChiTietGioHang
{
    public int MaCtgh { get; set; }

    public string MaGioHang { get; set; } = null!;

    public string MaSp { get; set; } = null!;

    public string? MaBienThe { get; set; }

    public int SoLuong { get; set; }

    public virtual BienTheSanPham? MaBienTheNavigation { get; set; }

    public virtual GioHang MaGioHangNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
