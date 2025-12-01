using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class NhanVien
{
    public string MaNv { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string MatKhauHash { get; set; } = null!;

    public bool TrangThai { get; set; }
}
