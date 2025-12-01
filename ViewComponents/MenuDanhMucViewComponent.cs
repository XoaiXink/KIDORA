using KIDORA.Data;
using KIDORA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KIDORA.ViewComponents
{
    public class MenuDanhMucViewComponent : ViewComponent
    {
        private readonly KidoraDbContext db;

        public MenuDanhMucViewComponent(KidoraDbContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // 1. Đếm sản phẩm theo từng danh mục (Trim để loại bỏ khoảng trắng CHAR)
            var categoryProductCounts = await db.SanPhams
                .GroupBy(sp => sp.MaDanhMuc.Trim())
                .Select(g => new
                {
                    MaDanhMuc = g.Key,
                    SoLuong = g.Count()
                })
                .ToDictionaryAsync(x => x.MaDanhMuc, x => x.SoLuong);

            // 2. Lấy toàn bộ danh mục
            var allCategories = await db.DanhMucs
                .Where(dm => dm.DangHoatDong)
                .Select(dm => new MenuDanhMucVM
                {
                    MaDanhMuc = dm.MaDanhMuc.Trim(),
                    TenDanhMuc = dm.TenDanhMuc,
                    MaDanhMucCha = dm.MaDanhMucCha != null ? dm.MaDanhMucCha.Trim() : null,
                    SoLuongSanPham = 0
                })
                .ToListAsync();

            // 3. Gán số sản phẩm trực tiếp cho từng danh mục
            foreach (var category in allCategories)
            {
                if (categoryProductCounts.TryGetValue(category.MaDanhMuc, out int count))
                    category.SoLuongSanPham = count;
            }

            // 4. Lấy danh mục cha (cấp 1)
            var parentCategories = allCategories
                .Where(c => string.IsNullOrEmpty(c.MaDanhMucCha))
                .OrderBy(c => c.TenDanhMuc)
                .ToList();

            // 5. Tạo phân cấp (Category tree)
            foreach (var parent in parentCategories)
            {
                parent.DanhMucCon = allCategories
                    .Where(c => c.MaDanhMucCha == parent.MaDanhMuc)
                    .OrderBy(c => c.TenDanhMuc)
                    .ToList();

                // cộng dồn số SP của con cấp 1 vào parent
                parent.SoLuongSanPham += parent.DanhMucCon.Sum(c => c.SoLuongSanPham);
            }

            return View(parentCategories);
        }
    }
}
