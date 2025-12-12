using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class DiaChiKhachHang
{
    public int MaDiaChi { get; set; }

    public string MaKh { get; set; } = null!;

    public string TenNguoiNhan { get; set; } = null!;

    public string DienThoaiNhan { get; set; } = null!;

    public string DiaChiChiTiet { get; set; } = null!;

    public string PhuongXa { get; set; } = null!;

    public string QuanHuyen { get; set; } = null!;

    public string TinhThanh { get; set; } = null!;

    public bool MacDinh { get; set; }

    public virtual KhachHang MaKhNavigation { get; set; } = null!;
}
