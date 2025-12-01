using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class ThuocTinh
{
    public int MaThuocTinh { get; set; }

    public string TenThuocTinh { get; set; } = null!;

    public string KieuDuLieu { get; set; } = null!;

    public string? MoTa { get; set; }

    public virtual ICollection<GiaTriThuocTinh> GiaTriThuocTinhs { get; set; } = new List<GiaTriThuocTinh>();
}
