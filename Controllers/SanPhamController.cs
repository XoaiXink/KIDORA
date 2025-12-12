using KIDORA.Data;
using KIDORA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KIDORA.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly KidoraDbContext _context;
        public SanPhamController(KidoraDbContext context)       // đối tượng db trong code
        {
            _context = context;
        }

        public IActionResult Index(string? danhmuc, int? price, int page = 1, int pageSize = 12)
        {
            var query = _context.SanPhams
                                .Include(p => p.MaDanhMucNavigation)
                                .AsQueryable();

            // ------ LỌC DANH MỤC (CHA + CON) ------
            if (!string.IsNullOrEmpty(danhmuc))
            {
                var cat = danhmuc.Trim();

                var allCats = _context.DanhMucs.ToList();

                var childCats = allCats
                    .Where(dm => dm.MaDanhMucCha != null &&
                                 dm.MaDanhMucCha.Trim() == cat)
                    .Select(dm => dm.MaDanhMuc.Trim())
                    .ToList();

                childCats.Add(cat);

                query = query.Where(sp => childCats.Contains(sp.MaDanhMuc.Trim()));
            }

            // ------ LỌC GIÁ ------
            if (price.HasValue)
            {
                query = query.Where(sp => sp.DonGiaBan <= price.Value);
            }

            int tongSanPham = query.Count();
            int soTrang = (int)Math.Ceiling(tongSanPham / (double)pageSize);

            if (page < 1) page = 1;
            if (page > soTrang && soTrang > 0) page = soTrang;

            var sanPhams = query
                .OrderBy(p => p.MaSp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ListSanPhamVM
                {
                    MaSp = p.MaSp,
                    TenSp = p.TenSp,
                    DonGiaBan = p.DonGiaBan,
                    AnhChinh = p.AnhChinh ?? "",
                    MoTaNgan = p.MoTaNgan ?? "",
                    MaDanhMuc = p.MaDanhMucNavigation.TenDanhMuc
                })
                .ToList();

            var vm = new PhanTrangSanPhamVM
            {
                SanPhams = sanPhams,
                SoTrang = soTrang,
                TrangHienTai = page,
                TongSanPham = tongSanPham,
                PageSize = pageSize
            };

            return View(vm);
        }


        public IActionResult ChiTietSanPham(string id)
        {
            // 1. Lấy sản phẩm hiện tại
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

            // 2. Map sang ViewModel chi tiết
            var result = new ChiTietSanPhamVM
            {
                MaSp = product.MaSp,
                TenSp = product.TenSp,
                TenDanhMuc = product.MaDanhMucNavigation.TenDanhMuc,
                MoTaNgan = product.MoTaNgan ?? string.Empty,
                AnhChinh = product.AnhChinh ?? string.Empty,
                DonGiaBan = product.DonGiaBan,
                SoLuongTon = product.SoLuongTon,
                MoTaChiTiet = product.MoTaChiTiet ?? string.Empty,

                // ⭐ rất quan trọng để lấy related
                MaDanhMuc = product.MaDanhMuc
            };

            // 3. LẤY SẢN PHẨM LIÊN QUAN (CÙNG DANH MỤC)
            var relatedProducts = _context.SanPhams
                .Where(sp =>
                    sp.MaDanhMuc == product.MaDanhMuc &&   // cùng danh mục
                    sp.MaSp != product.MaSp &&             // không lấy chính nó
                    sp.DangBan == true                     // đang bán
                )
                .OrderByDescending(sp => sp.MaSp)
                .Take(8)
                .Select(sp => new ListSanPhamVM
                {
                    MaSp = sp.MaSp,
                    TenSp = sp.TenSp,
                    DonGiaBan = sp.DonGiaBan,
                    MoTaNgan = sp.MoTaNgan ?? "",
                    AnhChinh = sp.AnhChinh ?? ""
                })
                .ToList();

            // 4. Truyền sang View
            ViewBag.RelatedProducts = relatedProducts;

            return View(result);
        }
    }
}
