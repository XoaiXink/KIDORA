using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class HangThanhVien
{
    public string MaHang { get; set; } = null!;

    public string TenHang { get; set; } = null!;

    public decimal MucChiTieu6Thang { get; set; }

    public decimal TyLeHoanKcoin { get; set; }

    public string? MoTa { get; set; }

    public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();
}
