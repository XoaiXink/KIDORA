using KIDORA.Data;
using KIDORA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KIDORA.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly KidoraDbContext _context;
        public SanPhamController(KidoraDbContext context)       // đối tượng db trong code
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? danhmuc, int? price, string? keyword, string? sort = "default", int page = 1, int pageSize = 12)
        {
            var query = _context.SanPhams
                                .Include(p => p.MaDanhMucNavigation)
                                .Where(p => p.DangBan)
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

            // ------ TÌM KIẾM ------
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim().ToLower();
                query = query.Where(sp =>
                    sp.TenSp.ToLower().Contains(k) ||
                    sp.Sku.ToLower().Contains(k) ||
                    sp.MaSp.ToLower().Contains(k));
            }

            // LẤY MIN/MAX GIÁ SAU LỌC DANH MỤC + TÌM KIẾM
            decimal minPrice = 0;
            decimal maxPrice = 0;
            if (await query.AnyAsync())
            {
                minPrice = await query.MinAsync(sp => sp.DonGiaBan);
                maxPrice = await query.MaxAsync(sp => sp.DonGiaBan);
            }

            // ------ LỌC GIÁ ------
            if (price.HasValue)
            {
                query = query.Where(sp => sp.DonGiaBan <= price.Value);
            }

            // SẮP XẾP
            query = sort switch
            {
                "price_asc" => query.OrderBy(sp => sp.DonGiaBan),
                "price_desc" => query.OrderByDescending(sp => sp.DonGiaBan),
                _ => query.OrderBy(sp => sp.MaSp)
            };

            int tongSanPham = await query.CountAsync();
            int soTrang = (int)Math.Ceiling(tongSanPham / (double)pageSize);

            if (page < 1) page = 1;
            if (page > soTrang && soTrang > 0) page = soTrang;

            var sanPhams = query
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

            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SelectedPrice = price ?? (maxPrice > 0 ? (int)Math.Ceiling(maxPrice) : 0);
            ViewBag.Keyword = keyword;
            ViewBag.DanhMuc = danhmuc;
            ViewBag.Sort = sort;

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

            // ===== RELATED PRODUCT =====
            // Lấy các sản phẩm khác cùng danh mục (không bao gồm sản phẩm hiện tại), loại bỏ trùng lặp,
            // sắp xếp và giới hạn 8 mục. Dùng AsNoTracking vì chỉ đọc.
            var relatedProducts = _context.SanPhams
                .AsNoTracking()
                .Where(sp => sp.MaDanhMuc == product.MaDanhMuc
                          && sp.MaSp != product.MaSp
                          && sp.DangBan)
                .GroupBy(sp => sp.MaSp)
                .Select(g => g.First())
                .OrderBy(sp => sp.MaSp)
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

            ViewBag.RelatedProducts = relatedProducts;

            // ===== ĐÁNH GIÁ SẢN PHẨM =====
            vm.DanhGias = _context.DanhGiaSanPhams
     .Include(dg => dg.MaKhNavigation)              // KHACHHANG
         .ThenInclude(kh => kh.MaKhNavigation)     // NGUOIDUNG
     .Where(dg => dg.MaSp == product.MaSp)
     .OrderByDescending(dg => dg.NgayDanhGia)
     .Select(dg => new DanhGiaVM
     {
         MaDanhGia = dg.MaDanhGia,
         DiemDanhGia = dg.DiemDanhGia,
         NoiDung = dg.NoiDung ?? "",
         NgayDanhGia = dg.NgayDanhGia,
         MaKH = dg.MaKh,

         // ⭐ LẤY TÊN TỪ NGUOIDUNG
         HoTen = dg.MaKhNavigation.MaKhNavigation.HoTen
     })
     .ToList();

            return View(vm);
        }
        [HttpPost]
        public IActionResult ThemDanhGia(
    string MaSp,
    string NoiDung,
    int DiemDanhGia)
        {
            // 1. Lấy ID người dùng đang login
            var Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Id == null)
                return RedirectToAction("Login", "Account");

            // 2. Lấy KhachHang tương ứng
            var kh = _context.KhachHangs
                .FirstOrDefault(k => k.MaKh == Id);

            if (kh == null)
                return BadRequest("Không tìm thấy khách hàng");

            // 3. Tạo đánh giá
            var danhGia = new DanhGiaSanPham
            {
                MaSp = MaSp,
                MaKh = kh.MaKh,           // ✅ ĐÚNG: FK KHACHHANG
                NoiDung = NoiDung,
                DiemDanhGia = DiemDanhGia,
                NgayDanhGia = DateTime.Now,
                TrangThai = "Chờ duyệt"
            };

            _context.DanhGiaSanPhams.Add(danhGia);
            _context.SaveChanges();

            // 4. Redirect về đúng tab
            return Redirect($"/SanPham/ChiTietSanPham/{MaSp}#nav-mission");
        }


    }
}
