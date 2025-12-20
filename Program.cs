using AspNetCoreHero.ToastNotification;
using KIDORA.Data;
using KIDORA.Models.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// ================= SERVICES =================
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<VnPayService>();
builder.Services.AddSingleton<IVnPayServices, VnPayService>();

// ================= CONFIG =================
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// ================= DB =================
var connectionString = builder.Configuration.GetConnectionString("KidoraDB");
builder.Services.AddDbContext<KidoraDbContext>(options =>
    options.UseSqlServer(connectionString)
);

// ================= SESSION =================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ================= AUTHENTICATION =================
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    })
    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    });

// ================= MVC =================
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

// ================= UNICODE =================
builder.Services.AddSingleton<HtmlEncoder>(
    HtmlEncoder.Create(new TextEncoderSettings(UnicodeRanges.All))
);

// ================= NOTYF =================
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;
});

// ================= BUILD =================
var app = builder.Build();

// ================= MIDDLEWARE =================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
// Ensure HTML responses include charset=utf-8. Run the rest of the pipeline first so
// framework components (MVC, static files) can set Content-Type, then we add
// charset if missing.
app.Use(async (context, next) =>
{
    await next();

    // Prefer Response.ContentType (may be null) and only adjust text/html responses.
    var contentType = context.Response.ContentType;
    if (string.IsNullOrEmpty(contentType))
    {
        // If nothing set, default to HTML UTF-8
        context.Response.ContentType = "text/html; charset=utf-8";
    }
    else if (contentType.StartsWith("text/html", StringComparison.OrdinalIgnoreCase)
             && !contentType.Contains("charset=", StringComparison.OrdinalIgnoreCase))
    {
        // Append charset if missing
        context.Response.ContentType = contentType + "; charset=utf-8";
    }
});

// ================= ROUTES =================
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
