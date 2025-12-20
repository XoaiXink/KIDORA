namespace KIDORA.ViewModels
{
    // ====== DÙNG CHO LIST / RELATED PRODUCT ======
    public class ListSanPhamVM
    {
        public string MaSp { get; set; }
        public string TenSp { get; set; }
        public decimal DonGiaBan { get; set; }
        public string MoTaNgan { get; set; }
        public string AnhChinh { get; set; }
        public string MaDanhMuc { get; set; }
    }

    // ====== DÙNG CHO TRANG CHI TIẾT ======
    public class ChiTietSanPhamVM
    {
        // Thông tin cơ bản
        public string MaSp { get; set; }
        public string TenSp { get; set; }
        public decimal DonGiaBan { get; set; }
        public string MoTaNgan { get; set; }
        public string MoTaChiTiet { get; set; }

        public string TenDanhMuc { get; set; }
        public string MaDanhMuc { get; set; }
        public int SoLuongTon { get; set; }

        // =======================
        // ẢNH SẢN PHẨM
        // =======================
        public string AnhChinh { get; set; }               // để không vỡ code cũ
        public List<string> AnhSanPhams { get; set; } = new();

        // =======================
        // BIẾN THỂ
        // =======================
        public List<BienTheVM> BienThes { get; set; } = new();
        public List<DanhGiaVM> DanhGias { get; set; } = new();

    }

    // ====== BIẾN THỂ ======
    public class BienTheVM
    {
        public string MaBienThe { get; set; }
        public string TenBienThe { get; set; }

        public decimal GiaBan { get; set; }
        public decimal GiaNiemYet { get; set; }
        public int SoLuongTon { get; set; }

        public List<string> AnhBienThe { get; set; } = new();
    }
    public class DanhGiaVM
    {
        public int MaDanhGia { get; set; }
        public int DiemDanhGia { get; set; }

        public string NoiDung { get; set; }
        public DateTime NgayDanhGia { get; set; }
        public string TrangThai { get; set; }
        public string MaKH { get; set; }
        public string HoTen { get; set; }

    }
}
