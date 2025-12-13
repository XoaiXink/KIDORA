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
            var product = _context.SanPhams
                .Include(sp => sp.MaDanhMucNavigation)
                .Include(sp => sp.AnhSanPhams)
                .Include(sp => sp.BienTheSanPhams)
                    .ThenInclude(bt => bt.BienTheGium)
                .Include(sp => sp.BienTheSanPhams)
                    .ThenInclude(bt => bt.BienTheTonKho)
                .Include(sp => sp.BienTheSanPhams)
                    .ThenInclude(bt => bt.BienTheHinhs)
                .FirstOrDefault(sp => sp.MaSp == id);

            if (product == null) return Redirect("/404");

            var vm = new ChiTietSanPhamVM
            {
                MaSp = product.MaSp,
                TenSp = product.TenSp,
                TenDanhMuc = product.MaDanhMucNavigation.TenDanhMuc,
                MaDanhMuc = product.MaDanhMuc,

                MoTaNgan = product.MoTaNgan ?? "",
                MoTaChiTiet = product.MoTaChiTiet ?? "",
                DonGiaBan = product.DonGiaBan,
                SoLuongTon = product.SoLuongTon,

                // ===== ẢNH SẢN PHẨM =====
                AnhChinh = product.AnhChinh,
                AnhSanPhams = product.AnhSanPhams
                    .OrderByDescending(x => x.AnhChinh)
                    .Select(x => x.DuongDanAnh)
                    .ToList(),

                // ===== BIẾN THỂ =====
                BienThes = product.BienTheSanPhams.Select(bt => new BienTheVM
                {
                    MaBienThe = bt.MaBienThe,
                    TenBienThe = bt.TenBienThe,
                    GiaBan = bt.BienTheGium!.GiaBan,
                    GiaNiemYet = bt.BienTheGium!.GiaNiemYet,
                    SoLuongTon = bt.BienTheTonKho.SoLuongTon,
                    AnhBienThe = bt.BienTheHinhs
                        .OrderByDescending(x => x.AnhChinh)
                        .Select(x => x.DuongDanAnh)
                        .ToList()
                }).ToList()
            };

            // ===== RELATED PRODUCT  =====
            ViewBag.RelatedProducts = _context.SanPhams
                .Where(sp => sp.MaDanhMuc == product.MaDanhMuc
                          && sp.MaSp != product.MaSp
                          && sp.DangBan)
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

            return View(vm);
        }
    }
}
