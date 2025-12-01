using System;
using System.Collections.Generic;

namespace KIDORA.Data;

public partial class DonHang
{
    public string MaDonHang { get; set; } = null!;

    public string SoDonHang { get; set; } = null!;

    public string MaKh { get; set; } = null!;

    public DateTime NgayDat { get; set; }

    public string TrangThaiDon { get; set; } = null!;

    public string TrangThaiThanhToan { get; set; } = null!;

    public string MaDvvc { get; set; } = null!;

    public decimal PhiVanChuyen { get; set; }

    public decimal GiamGiaKm { get; set; }

    public decimal GiamGiaKcoin { get; set; }

    public decimal TongTienHang { get; set; }

    public decimal TongGiamGia { get; set; }

    public decimal TongThanhToan { get; set; }

    public decimal Vat { get; set; }

    public decimal TongSauVat { get; set; }

    public string TenNguoiNhan { get; set; } = null!;

    public string SdtnguoiNhan { get; set; } = null!;

    public string DiaChiGiao { get; set; } = null!;

    public DateOnly? NgayGiaoThucTe { get; set; }

    public string? GhiChu { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<GdKcoin> GdKcoins { get; set; } = new List<GdKcoin>();

    public virtual ICollection<KmDonHang> KmDonHangs { get; set; } = new List<KmDonHang>();

    public virtual DonViVanChuyen MaDvvcNavigation { get; set; } = null!;

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
