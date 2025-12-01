using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class AnhSanPham
{
    public int MaAnh { get; set; }

    public string MaSp { get; set; } = null!;

    public string DuongDanAnh { get; set; } = null!;

    public bool AnhChinh { get; set; }

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
