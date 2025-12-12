using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class Admin
{
    public string MaAdmin { get; set; } = null!;

    public string? CuaHang { get; set; }

    public string? Quyen { get; set; }

    public virtual NguoiDung MaAdminNavigation { get; set; } = null!;
}
