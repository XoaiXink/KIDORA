namespace KIDORA.ViewModels
{
    public class SanPhamGioHang
    {
        public string MaSp { get; set; }
        public string TenSp { get; set; } = null!;
        public decimal DonGiaBan { get; set; }
        public string AnhChinh { get; set; } = null!;
        public int SoLuong { get; set; }
        public decimal ThanhTien
        {
            get
            {
                return DonGiaBan * SoLuong;
            }
        }
    }
}
