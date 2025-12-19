using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class NguoiDung
{
    public string Id { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? DienThoai { get; set; }

    public string MatKhauHash { get; set; } = null!;

    public string? GioiTinh { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string LoaiNguoiDung { get; set; } = null!;

    public bool EmailConfirmed { get; set; }

    public string? VerificationToken { get; set; }

    public string? ResetToken { get; set; }

    public DateTime? ResetTokenExpiry { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual KhachHang? KhachHang { get; set; }
}
