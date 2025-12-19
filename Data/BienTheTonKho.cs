using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class BienTheTonKho
{
    public string MaBienThe { get; set; } = null!;

    public int SoLuongTon { get; set; }

    public int SoLuongDatHang { get; set; }

    public int TonAnToan { get; set; }

    public DateTime NgayCapNhat { get; set; }

    public virtual BienTheSanPham MaBienTheNavigation { get; set; } = null!;
}
