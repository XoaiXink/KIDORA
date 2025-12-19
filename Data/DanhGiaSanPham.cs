using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class DanhGiaSanPham
{
    public int MaDanhGia { get; set; }

    public string MaSp { get; set; } = null!;

    public string MaKh { get; set; } = null!;

    public int DiemDanhGia { get; set; }

    public string? NoiDung { get; set; }

    public DateTime NgayDanhGia { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
