namespace KIDORA.ViewModels
{
    public class ListSanPhamVM
    {
        public string MaSp { get; set; }
        public string TenSp { get; set; }
        public decimal DonGiaBan { get; set; }
        public string MoTaNgan { get; set; }
        public string AnhChinh { get; set; }
        public string MaDanhMuc { get; set; }

    }
    public class ChiTietSanPhamVM
    {
        public string MaSp { get; set; }
        public string TenSp { get; set; }
        public decimal DonGiaBan { get; set; }
        public string MoTaNgan { get; set; }
        public string AnhChinh { get; set; }
        public string MoTaChiTiet { get; set; }
        public string TenDanhMuc { get; internal set; }
    }
}
