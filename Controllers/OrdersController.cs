using KIDORA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class OrdersController : Controller
{
    private readonly KidoraDbContext _context;

    public OrdersController(KidoraDbContext context)
    {
        _context = context;
    }

    // =================== DANH SÁCH ĐƠN HÀNG ===================
    public IActionResult Index()
    {
        // Lấy email đăng nhập
        var email = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Login", "Account");

        // Tìm user theo Email → lấy Id (VD: KH000008)
        var user = _context.NguoiDungs
            .FirstOrDefault(x => x.Email == email);

        if (user == null)
            return RedirectToAction("Login", "Account");

        // Lấy danh sách đơn theo MaKh = user.Id
        var donhang = _context.DonHangs
            .Where(x => x.MaKh == user.Id)
            .OrderByDescending(x => x.NgayDat)
            .Include(x => x.ChiTietDonHangs)
            .ToList();

        return View(donhang);
    }

    // =================== CHI TIẾT ĐƠN HÀNG ===================
    public IActionResult Detail(string id)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        var user = _context.NguoiDungs
            .FirstOrDefault(x => x.Email == email);

        if (user == null)
            return RedirectToAction("Login", "Account");

        var dh = _context.DonHangs
            .Include(x => x.ChiTietDonHangs)
            .FirstOrDefault(x => x.MaDonHang == id && x.MaKh == user.Id);

        if (dh == null)
            return NotFound();

        return View(dh);
    }

    // =================== HỦY ĐƠN HÀNG ===================
    [HttpPost]
    public IActionResult CancelOrder(string id)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        var user = _context.NguoiDungs
            .FirstOrDefault(x => x.Email == email);

        if (user == null)
            return RedirectToAction("Login", "Account");

        var dh = _context.DonHangs
            .FirstOrDefault(x => x.MaDonHang == id && x.MaKh == user.Id);

        if (dh == null)
            return NotFound();

        if (dh.TrangThaiDon != "Chờ xác nhận")
            return BadRequest("Không thể hủy đơn.");

        dh.TrangThaiDon = "Đã hủy";
        _context.SaveChanges();

        TempData["success"] = "Đã hủy đơn hàng thành công!";
        return RedirectToAction("Index");
    }
}
