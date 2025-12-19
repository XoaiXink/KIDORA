using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KIDORA.Data;

namespace KIDORA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhachHangsController : Controller
    {
        private readonly KidoraDbContext _context;

        public KhachHangsController(KidoraDbContext context)
        {
            _context = context;
        }
        private async Task<string> GenerateNewCustomerIdAsync()
        {
            var lastId = await _context.NguoiDungs
                .Where(x => x.Id.StartsWith("KH"))
                .OrderByDescending(x => x.Id)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (!string.IsNullOrEmpty(lastId) && lastId.Length == 8)
            {
                if (int.TryParse(lastId.Substring(2), out int current))
                {
                    nextNumber = current + 1;
                }
            }

            return "KH" + nextNumber.ToString("D6");   // KH000001, KH000002, ...
        }

        // GET: Admin/KhachHangs
        // Danh sách khách hàng
        // GET: Admin/KhachHangs
        public async Task<IActionResult> Index(
            string searchString,
            string hang,          // mã hạng TV để lọc
            string emailStatus    // trạng thái email
        )
        {
            // Lưu lại giá trị filter để đổ về View
            ViewBag.SearchString = searchString;
            ViewBag.Hang = hang;
            ViewBag.EmailStatus = emailStatus;

            // Lấy danh sách hạng TV cho dropdown
            ViewBag.HangList = new SelectList(
                await _context.HangThanhViens
                    .OrderBy(h => h.MucChiTieu6Thang)
                    .ToListAsync(),
                "MaHang",
                "TenHang"
            );

            // Query khách hàng + join sang bảng người dùng & hạng
            var query = _context.KhachHangs
                .Include(k => k.MaHangNavigation)
                .Include(k => k.MaKhNavigation)   // NGUOI_DUNG
                .AsQueryable();

            // TÌM KIẾM: mã KH, họ tên, email, SĐT
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var keyword = searchString.Trim().ToLower();

                query = query.Where(k =>
                    k.MaKh.ToLower().Contains(keyword) ||
                    k.MaKhNavigation.HoTen.ToLower().Contains(keyword) ||
                    k.MaKhNavigation.Email.ToLower().Contains(keyword) ||
                    k.MaKhNavigation.DienThoai.ToLower().Contains(keyword)
                );
            }

            // LỌC THEO HẠNG THÀNH VIÊN
            if (!string.IsNullOrEmpty(hang) && hang != "all")
            {
                query = query.Where(k => k.MaHang == hang);
            }

            // LỌC THEO TRẠNG THÁI EMAIL (dựa trên NGUOI_DUNG.EmailConfirmed)
            if (!string.IsNullOrEmpty(emailStatus) && emailStatus != "all")
            {
                bool isConfirmed = emailStatus == "confirmed";
                query = query.Where(k => k.MaKhNavigation.EmailConfirmed == isConfirmed);
            }

            // Sắp xếp theo mã KH tăng dần cho gọn
            var result = await query
                .OrderBy(k => k.MaKh)
                .ToListAsync();

            return View(result);
        }


        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .Include(k => k.MaKhNavigation)   // load NGUOI_DUNG
                .Include(k => k.MaHangNavigation) // load HANG_THANH_VIEN
                .FirstOrDefaultAsync(m => m.MaKh == id);

            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }


        // GET: Admin/KhachHangs/Create
        public IActionResult Create()
        {
            ViewData["MaHang"] = new SelectList(_context.HangThanhViens, "MaHang", "TenHang");

            var model = new KhachHang
            {
                SoDuKcoin = 0,
                NgayThamGia = DateTime.Now,
                MaKhNavigation = new NguoiDung() // để binding cho phần thông tin tài khoản
            };

            return View(model);
        }


        // POST: Admin/KhachHangs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("MaHang,SoDuKcoin,NgayThamGia,MaKhNavigation")] KhachHang model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["MaHang"] = new SelectList(_context.HangThanhViens,
                                                    "MaHang", "TenHang", model.MaHang);
                return View(model);
            }

            // 1. Sinh mã KH mới
            var newId = await GenerateNewCustomerIdAsync();

            // 2. Chuẩn bị bản ghi NGUOI_DUNG
            var user = model.MaKhNavigation ?? new NguoiDung();
            user.Id = newId;
            user.LoaiNguoiDung = "KHACH_HANG";
            user.MatKhauHash = "TEMP_HASH";   // chỗ này sau có login thì thay bằng hash thật
            user.EmailConfirmed = false;

            // 3. Chuẩn bị bản ghi KHACH_HANG
            var khachHang = new KhachHang
            {
                MaKh = newId,
                MaHang = model.MaHang,
                SoDuKcoin = model.SoDuKcoin,
                NgayThamGia = model.NgayThamGia,
                MaKhNavigation = user
            };

            _context.NguoiDungs.Add(user);
            _context.KhachHangs.Add(khachHang);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Admin/KhachHangs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .Include(k => k.MaKhNavigation)     // load NGUOI_DUNG
                .Include(k => k.MaHangNavigation)   // (nếu muốn dùng thêm)
                .FirstOrDefaultAsync(k => k.MaKh == id);

            if (khachHang == null)
            {
                return NotFound();
            }

            ViewData["MaHang"] = new SelectList(_context.HangThanhViens, "MaHang", "MaHang", khachHang.MaHang);

            return View(khachHang);
        }


        // POST: Admin/KhachHangs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaKh,MaHang,SoDuKcoin,NgayThamGia")] KhachHang khachHang)
        {
            if (id != khachHang.MaKh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khachHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachHangExists(khachHang.MaKh))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaHang"] = new SelectList(
                _context.HangThanhViens,
                "MaHang",
                "TenHang",
                khachHang.MaHang
            );

            ViewData["MaKh"] = new SelectList(
                _context.NguoiDungs
                    .Where(nd => nd.LoaiNguoiDung == "KHACH_HANG"),
                "Id",
                "Email",
                khachHang.MaKh
            );

            return View(khachHang);
        }

        // GET: Admin/KhachHangs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .Include(k => k.MaHangNavigation)
                .Include(k => k.MaKhNavigation)
                .FirstOrDefaultAsync(m => m.MaKh == id);

            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        // POST: Admin/KhachHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang != null)
            {
                _context.KhachHangs.Remove(khachHang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KhachHangExists(string id)
        {
            return _context.KhachHangs.Any(e => e.MaKh == id);
        }
    }
}
