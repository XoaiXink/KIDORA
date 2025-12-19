using KIDORA.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class AccountController : Controller
{
    private readonly KidoraDbContext _context;
    private readonly EmailService _emailService;

    public AccountController(KidoraDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    // ===================== LOGIN =====================
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _context.NguoiDungs
            .FirstOrDefaultAsync(x => x.Email == email);

        if (user == null)
        {
            ViewBag.Error = "Email không tồn tại!";
            return View();
        }

        // ❗ Chặn login nếu chưa xác thực email
        if (!user.EmailConfirmed && !user.Id.StartsWith("AD"))
        {
            ViewBag.Error = "Email chưa được xác thực. Vui lòng kiểm tra hộp thư!";
            return View();
        }

        // Sai mật khẩu?
        if (!BCrypt.Net.BCrypt.Verify(password, user.MatKhauHash))
        {
            ViewBag.Error = "Sai mật khẩu!";
            return View();
        }

        // Tạo claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.HoTen),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("UserId", user.Id),
            new Claim(ClaimTypes.Role, user.LoaiNguoiDung)
        };

        var identity = new ClaimsIdentity(
             claims,
             CookieAuthenticationDefaults.AuthenticationScheme
         );

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity)
        );

        // ===== PHÂN LUỒNG =====
        if (user.LoaiNguoiDung == "ADMIN")
        {
            return RedirectToAction(
                "Index",
                "Home",
                new { area = "Admin" }
            );
        }

        return RedirectToAction("Index", "Home");
    }
    public IActionResult ExternalLogin(string provider)
    {
        var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl
        };

        return Challenge(properties, provider);
    }
    public async Task<IActionResult> ExternalLoginCallback()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded)
            return RedirectToAction("Login");

        var email = User.FindFirstValue(ClaimTypes.Email);
        var name = User.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Login");

        var user = await _context.NguoiDungs
            .FirstOrDefaultAsync(x => x.Email == email);

        // ===== CHƯA CÓ USER → TẠO MỚI =====
        if (user == null)
        {
            string newId = GenerateKhachHangId(); // ND000xxx

            user = new NguoiDung
            {
                Id = newId,
                HoTen = name ?? email,
                Email = email,
                LoaiNguoiDung = "KHACH_HANG",
                EmailConfirmed = true, // Google/Facebook đã xác thực
                MatKhauHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString())
            };

            _context.NguoiDungs.Add(user);

            _context.KhachHangs.Add(new KhachHang
            {
                MaKh = newId,
                MaHang = "MEMB",
                SoDuKcoin = 0,
                NgayThamGia = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        // ===== ĐĂNG NHẬP COOKIE =====
        var claims = new List<Claim>
   {
       new Claim(ClaimTypes.Name, user.HoTen),
       new Claim(ClaimTypes.Email, user.Email),
       new Claim("UserId", user.Id),
       new Claim(ClaimTypes.Role, user.LoaiNguoiDung)
   };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        // ===== PHÂN QUYỀN =====
        if (user.Id.StartsWith("AD"))
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        return RedirectToAction("Index", "Home");
    }
    private string GenerateKhachHangId()
    {
        string lastId = _context.NguoiDungs
            .Where(x => x.Id.StartsWith("KH"))
            .OrderByDescending(x => x.Id)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (string.IsNullOrEmpty(lastId))
            return "KH000001";

        int number = int.Parse(lastId.Substring(2)) + 1;
        return "KH" + number.ToString("D6");
    }

    // ===================== FORGOT PASSWORD =====================
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        var user = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.Email == email);

        if (user == null)
        {
            ViewBag.Error = "Email không tồn tại trong hệ thống!";
            return View();
        }

        // Tạo token reset mật khẩu
        string resetToken = Guid.NewGuid().ToString("N");
        user.ResetToken = resetToken;
        user.ResetTokenExpiry = DateTime.Now.AddMinutes(30);

        await _context.SaveChangesAsync();

        // Gửi email reset
        string resetUrl = Url.Action("ResetPassword", "Account",
            new { token = resetToken }, Request.Scheme);

        await _emailService.SendEmailAsync(
            email,
            "Đặt lại mật khẩu Kidora",
            $@"
            <h3>Đặt lại mật khẩu</h3>
            <p>Nhấn vào liên kết dưới đây để đặt lại mật khẩu:</p>
            <a href='{resetUrl}'>ĐẶT LẠI MẬT KHẨU</a>
        "
        );

        TempData["Success"] = "Vui lòng kiểm tra email để đặt lại mật khẩu!";
        return RedirectToAction("Login");
    }
    // ===================== RESET PASSWORD =====================
    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        if (string.IsNullOrEmpty(token))
            return Content("Token không hợp lệ!");

        return View(model: token);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(string token, string newPass, string confirmPass)
    {
        var user = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.ResetToken == token);

        if (user == null || user.ResetTokenExpiry < DateTime.Now)
        {
            ViewBag.Error = "Token không hợp lệ hoặc đã hết hạn!";
            return View(model: token);
        }

        if (newPass != confirmPass)
        {
            ViewBag.Error = "Mật khẩu xác nhận không khớp!";
            return View(model: token);
        }

        user.MatKhauHash = BCrypt.Net.BCrypt.HashPassword(newPass);
        user.ResetToken = null;
        user.ResetTokenExpiry = null;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Đặt lại mật khẩu thành công!";
        return RedirectToAction("Login");
    }

    // ===================== REGISTER KHÁCH HÀNG =====================
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(
        string hoten,
        string email,
        string phone,
        string password,
        string tinh,          // Tỉnh / Thành phố
        string quan,          // Quận / Huyện
        string phuong,        // ⭐ Phường / Xã (bạn cần thêm input ở View)
        string diachi,        // Địa chỉ chi tiết
        string tenNguoiNhan,
        string dienThoaiNhan
    )
    {
        // Email trùng?
        if (_context.NguoiDungs.Any(x => x.Email == email))
        {
            ViewBag.Error = "Email đã tồn tại!";
            return View();
        }

        // Tạo ID dạng ND000001
        string lastId = _context.NguoiDungs
            .OrderByDescending(x => x.Id)
            .Select(x => x.Id)
            .FirstOrDefault();

        string newId = "ND000001";
        if (!string.IsNullOrEmpty(lastId))
        {
            int number = int.Parse(lastId.Substring(2)) + 1;
            newId = "KH" + number.ToString("D6");
        }

        // Tạo token xác thực email
        string verifyToken = Guid.NewGuid().ToString("N");

        // -------------------
        // NGUOI_DUNG
        // -------------------
        var user = new NguoiDung
        {
            Id = newId,
            HoTen = hoten,
            Email = email,
            DienThoai = phone,
            MatKhauHash = BCrypt.Net.BCrypt.HashPassword(password),
            LoaiNguoiDung = "KHACH_HANG",
            EmailConfirmed = false,
            VerificationToken = verifyToken
        };
        _context.NguoiDungs.Add(user);

        // -------------------
        // KHACH_HANG
        // -------------------
        var kh = new KhachHang
        {
            MaKh = newId,
            MaHang = "MEMB",
            SoDuKcoin = 0,
            NgayThamGia = DateTime.Now
        };
        _context.KhachHangs.Add(kh);

        // -------------------
        // DIA_CHI_KHACH_HANG (Model mới)
        // -------------------
        var diaChiKH = new DiaChiKhachHang
        {
            MaKh = newId,
            TenNguoiNhan = tenNguoiNhan,
            DienThoaiNhan = dienThoaiNhan,
            DiaChiChiTiet = diachi,
            PhuongXa = phuong,
            QuanHuyen = quan,
            TinhThanh = tinh,
            MacDinh = true
        };

        _context.DiaChiKhachHangs.Add(diaChiKH);

        await _context.SaveChangesAsync();

        // ============================================
        // GỬI EMAIL XÁC THỰC
        // ============================================
        string verifyUrl = Url.Action("VerifyEmail", "Account",
            new { token = verifyToken }, Request.Scheme);

        await _emailService.SendEmailAsync(
            email,
            "Xác thực tài khoản Kidora",
            $@"
            <h3>Chào {hoten}</h3>
            <p>Vui lòng nhấn vào liên kết dưới đây để kích hoạt tài khoản:</p>
            <a href='{verifyUrl}'>XÁC THỰC EMAIL</a>
            "
        );

        TempData["Success"] = "Đăng ký thành công! Hãy kiểm tra email để kích hoạt tài khoản.";
        return RedirectToAction("Login");
    }

    // ===================== XÁC THỰC EMAIL =====================
    public async Task<IActionResult> VerifyEmail(string token)
    {
        var user = await _context.NguoiDungs
            .FirstOrDefaultAsync(x => x.VerificationToken == token);

        if (user == null)
            return Content("Token không hợp lệ hoặc đã hết hạn!");

        user.EmailConfirmed = true;
        user.VerificationToken = null;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Xác thực email thành công! Hãy đăng nhập.";
        return RedirectToAction("Login");
    }

    // ===================== LOGOUT =====================
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Login");
    }
}
