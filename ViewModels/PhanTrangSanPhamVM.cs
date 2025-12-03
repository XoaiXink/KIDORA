namespace KIDORA.ViewModels
{
    public class PhanTrangSanPhamVM
    {
        // danh sách sản phẩm hiển thị ở trang hiện tại
        public IEnumerable<ListSanPhamVM> SanPhams { get; set; } = new List<ListSanPhamVM>();

        // số trang (tổng trang)
        public int SoTrang { get; set; }

        // trang đang đứng
        public int TrangHienTai { get; set; }

        // tổng số sản phẩm sau khi lọc
        public int TongSanPham { get; set; }

        // số sản phẩm mỗi trang
        public int PageSize { get; set; }
    }
}
