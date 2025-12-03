using KIDORA.Data;
using KIDORA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KIDORA.ViewComponents
{
    public class BestSellerViewComponent : ViewComponent
    {
        private readonly KidoraDbContext _context;

        public BestSellerViewComponent(KidoraDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int count = 4)
        {
            // Lấy TOP MaSP bán chạy
            var top = await (
                from ctdh in _context.ChiTietDonHangs
                join bt in _context.BienTheSanPhams on ctdh.MaBienThe equals bt.MaBienThe
                join sp in _context.SanPhams on bt.MaSp equals sp.MaSp
                group ctdh by new
                {
                    sp.MaSp,
                    sp.TenSp,
                    sp.AnhChinh,
                    sp.DonGiaBan
                }
                into g
                orderby g.Sum(x => x.SoLuong) descending
                select new BestSellerVM
                {
                    MaSp = g.Key.MaSp,
                    TenSp = g.Key.TenSp,
                    AnhChinh = g.Key.AnhChinh,
                    DonGiaBan = g.Key.DonGiaBan,
                    SoldCount = g.Sum(x => x.SoLuong)
                }
            )
            .Take(count)
            .ToListAsync();

            return View(top);
        }
    }
}
