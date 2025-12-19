using KIDORA.Helpers;
using KIDORA.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace KIDORA.ViewComponents
{
    public class GioHangViewComponent : ViewComponent

    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.GetT<List<SanPhamGioHang>>(MySetting.GIOHANG_KEY)
             ?? new List<SanPhamGioHang>();
            return View("GioHangPanel", new GioHangVM
            {
                quantity = cart.Sum(sp => sp.SoLuong),
                Total = cart.Sum(sp => sp.ThanhTien)
            });
        }
    }
}
