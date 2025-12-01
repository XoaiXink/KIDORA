using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class BienTheHinh
{
    public int MaAnhBienThe { get; set; }

    public string MaBienThe { get; set; } = null!;

    public string DuongDanAnh { get; set; } = null!;

    public bool AnhChinh { get; set; }

    public virtual BienTheSanPham MaBienTheNavigation { get; set; } = null!;
}
