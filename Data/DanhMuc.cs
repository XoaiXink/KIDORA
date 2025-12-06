using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class DanhMuc
{
    public string MaDanhMuc { get; set; } = null!;

    public string? MaDanhMucCha { get; set; }

    public string TenDanhMuc { get; set; } = null!;

    public string? MoTa { get; set; }

    public string? AnhDanhMuc { get; set; }

    public bool DangHoatDong { get; set; }

    public virtual ICollection<DanhMuc> InverseMaDanhMucChaNavigation { get; set; } = new List<DanhMuc>();

    public virtual DanhMuc? MaDanhMucChaNavigation { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
