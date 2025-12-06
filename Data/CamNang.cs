using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class CamNang
{
    public string MaBai { get; set; } = null!;

    public string TieuDe { get; set; } = null!;

    public string? Anh { get; set; }

    public DateOnly NgayDang { get; set; }

    public string? TacGia { get; set; }

    public string? TomTat { get; set; }

    public string NoiDung { get; set; } = null!;
}
