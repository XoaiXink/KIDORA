using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class BienTheGium
{
    public string MaBienThe { get; set; } = null!;

    public decimal GiaBan { get; set; }

    public decimal GiaNiemYet { get; set; }

    public DateTime NgayCapNhat { get; set; }

    public virtual BienTheSanPham MaBienTheNavigation { get; set; } = null!;
}
