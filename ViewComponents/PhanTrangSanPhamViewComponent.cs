using Microsoft.AspNetCore.Mvc;

namespace KIDORA.ViewComponents
{
    public class PhanTrangSanPhamViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int soTrang, int trangHienTai)
        {
            ViewBag.SoTrang = soTrang;
            ViewBag.TrangHienTai = trangHienTai;
            return View("PhanTrangSanPham");
        }
    }

}
