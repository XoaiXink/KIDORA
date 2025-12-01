using KIDORA.Data;
using KIDORA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KIDORA.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly KidoraDbContext _context;
        public SanPhamController(KidoraDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string? danhmuc)
        {
            var sanPhams = _context.SanPhams
                                    .Include(p => p.MaDanhMucNavigation)
                                    .AsQueryable();

            if (!string.IsNullOrEmpty(danhmuc))
            {
                // Lấy toàn bộ danh mục
                var allCats = _context.DanhMucs.ToList();

                // Lấy danh mục con của category được chọn
                var childCats = allCats
                    .Where(dm => dm.MaDanhMucCha?.Trim() == danhmuc.Trim())
                    .Select(dm => dm.MaDanhMuc.Trim())
                    .ToList();

                // Thêm chính danh mục cha
                childCats.Add(danhmuc.Trim());

                // Lọc sản phẩm theo tất cả danh mục con + cha
                sanPhams = sanPhams.Where(sp => childCats.Contains(sp.MaDanhMuc.Trim()));
            }

            var result = sanPhams.Select(p => new ListSanPhamVM
            {
                MaSp = p.MaSp,
                TenSp = p.TenSp,
                DonGiaBan = p.DonGiaBan,
                AnhChinh = p.AnhChinh ?? "",
                MoTaNgan = p.MoTaNgan ?? "",
                MaDanhMuc = p.MaDanhMucNavigation.TenDanhMuc
            })
            .ToList();

            return View(result);
        }


        public IActionResult ChiTietSanPham(string id)
        {
            var product = _context.SanPhams
                                  .Include(sp => sp.MaDanhMucNavigation)
                                  .Include(sp => sp.BienTheSanPhams)
                                  .Include(sp => sp.AnhSanPhams)
                                  .FirstOrDefault(sp => sp.MaSp == id);

            if (product == null)
            {
                TempData["ErrorMessage"] = $"Không tìm thấy sản phẩm với mã {id}.";
                return Redirect("/404");

            }
            var result = new ChiTietSanPhamVM
            {

                MaSp = product.MaSp,
                TenSp = product.TenSp,
                TenDanhMuc = product.MaDanhMucNavigation.TenDanhMuc,
                MoTaNgan = product.MoTaNgan ?? string.Empty,
                AnhChinh = product.AnhChinh ?? string.Empty,
                DonGiaBan = product.DonGiaBan,
                MoTaChiTiet = product.MoTaChiTiet ?? string.Empty,
            };
            return View(result);
        }
    }
}
