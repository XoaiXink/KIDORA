using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class GiaTriThuocTinh
{
    public int MaGiaTri { get; set; }

    public int MaThuocTinh { get; set; }

    public string GiaTri { get; set; } = null!;

    public string? GiaTriHienThi { get; set; }

    public virtual ThuocTinh MaThuocTinhNavigation { get; set; } = null!;

    public virtual ICollection<BienTheSanPham> MaBienThes { get; set; } = new List<BienTheSanPham>();
}
