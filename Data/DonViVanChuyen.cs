using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class DonViVanChuyen
{
    public string MaDvvc { get; set; } = null!;

    public string TenDvvc { get; set; } = null!;

    public string? DienThoai { get; set; }

    public string? TrackingUrl { get; set; }

    public bool DangHoatDong { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
