using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class ThanhToan
{
    public int MaTt { get; set; }

    public string MaDonHang { get; set; } = null!;

    public string PhuongThuc { get; set; } = null!;

    public string? MaGiaoDich { get; set; }

    public decimal SoTienThanhToan { get; set; }

    public DateTime NgayThanhToan { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual DonHang MaDonHangNavigation { get; set; } = null!;
}
