using KIDORA.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ProfileController : Controller
{
    private readonly KidoraDbContext _context;

    public ProfileController(KidoraDbContext context)
    {
        _context = context;
    }

    // ==========================
    // 1. TRANG THÔNG TIN CÁ NHÂN
    // ==========================
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        string userId = User.FindFirst("UserId")?.Value;
        if (userId == null) return RedirectToAction("Login", "Account");

        var user = await _context.NguoiDungs
            .Include(x => x.KhachHang)   // ⭐ Load bảng KHACH_HANG
            .FirstOrDefaultAsync(x => x.Id == userId);

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Update(string hoten, string phone, DateTime? ngaysinh, string? gioitinh)
    {
        string userId = User.FindFirst("UserId")?.Value;
        if (userId == null) return RedirectToAction("Login", "Account");

        var user = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null) return NotFound();

        user.HoTen = hoten;
        user.DienThoai = phone;
        if (ngaysinh != null)
            user.NgaySinh = DateOnly.FromDateTime(ngaysinh.Value);
        user.GioiTinh = gioitinh;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Cập nhật thông tin thành công!";
        return RedirectToAction("Index");
    }

    // ==========================
    // 2. QUẢN LÝ ĐỊA CHỈ KHÁCH HÀNG
    // ==========================

    [HttpGet]
    public async Task<IActionResult> Address()
    {
        string userId = User.FindFirst("UserId")?.Value;

        var address = await _context.DiaChiKhachHangs
            .FirstOrDefaultAsync(x => x.MaKh == userId && x.MacDinh == true);

        return View(address);
    }


    [HttpPost]
    public async Task<IActionResult> UpdateAddress(
        string tenNguoiNhan,
        string dienThoaiNhan,
        string diachi,   // Chi tiết số nhà, đường
        string phuong,   // Phường/Xã
        string quan,     // Quận/Huyện
        string tinh      // Tỉnh/Thành phố
    )
    {
        string userId = User.FindFirst("UserId")?.Value;

        var address = await _context.DiaChiKhachHangs
            .FirstOrDefaultAsync(x => x.MaKh == userId && x.MacDinh == true);

        if (address == null)
        {
            address = new DiaChiKhachHang
            {
                MaKh = userId!,
                MacDinh = true
            };

            _context.DiaChiKhachHangs.Add(address);
        }

        address.TenNguoiNhan = tenNguoiNhan;
        address.DienThoaiNhan = dienThoaiNhan;

        // ⭐ LƯU ĐÚNG CẤU TRÚC DATABASE MỚI
        address.DiaChiChiTiet = diachi;
        address.PhuongXa = phuong;
        address.QuanHuyen = quan;
        address.TinhThanh = tinh;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Cập nhật địa chỉ thành công!";
        return RedirectToAction("Address");
    }


    // ==========================
    // 3. ĐỔI MẬT KHẨU
    // ==========================

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(string oldPass, string newPass, string confirmPass)
    {
        string userId = User.FindFirst("UserId")?.Value;
        if (userId == null) return RedirectToAction("Login", "Account");

        var user = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null) return NotFound();

        if (!BCrypt.Net.BCrypt.Verify(oldPass, user.MatKhauHash))
        {
            ViewBag.Error = "Mật khẩu cũ không đúng!";
            return View();
        }

        if (newPass != confirmPass)
        {
            ViewBag.Error = "Mật khẩu nhập lại không khớp!";
            return View();
        }

        user.MatKhauHash = BCrypt.Net.BCrypt.HashPassword(newPass);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đổi mật khẩu thành công!";
        return RedirectToAction("Index");
    }
}
