using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class DiaChiKhachHang
{
    public int MaDiaChi { get; set; }

    public string MaKh { get; set; } = null!;

    public string TenNguoiNhan { get; set; } = null!;

    public string DienThoaiNhan { get; set; } = null!;

    public string DiaChiDayDu { get; set; } = null!;

    public string ThanhPho { get; set; } = null!;

    public string QuocGia { get; set; } = null!;

    public bool MacDinh { get; set; }

    public virtual KhachHang MaKhNavigation { get; set; } = null!;
}
