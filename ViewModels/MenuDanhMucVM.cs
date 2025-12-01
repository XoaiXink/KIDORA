namespace KIDORA.ViewModels
{
    public class MenuDanhMucVM
    {
        public string MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }
        public string? MaDanhMucCha { get; set; }
        public int SoLuongSanPham { get; set; }
        public List<MenuDanhMucVM> DanhMucCon { get; set; } = new();
    }
}
