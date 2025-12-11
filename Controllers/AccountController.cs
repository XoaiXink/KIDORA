using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class AccountController : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        // TODO: Kiểm tra username/password từ DB
        if (username == "admin" && password == "123")
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // ⭐ LUÔN PHẢI CHỈ ĐỊNH SCHEME
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Sai thông tin đăng nhập";
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(string username, string password, string email)
    {
        // TODO: Lưu user vào database
        return RedirectToAction("Login");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}

