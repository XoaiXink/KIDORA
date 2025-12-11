using AspNetCoreHero.ToastNotification;
using KIDORA.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// --- READ appsettings.json ---
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// --- GET CONNECTION STRING ---
var connectionString = builder.Configuration.GetConnectionString("KidoraDB");
Console.WriteLine($"Connection string: {connectionString}");

// --- ADD DB CONTEXT  ---
builder.Services.AddDbContext<KidoraDbContext>(options =>
    options.UseSqlServer(connectionString)
);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// --- MVC + RUNTIME COMPILATION ---
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// --- UTF8 / UNICODE SUPPORT ---
builder.Services.AddSingleton<HtmlEncoder>(
    HtmlEncoder.Create(new TextEncoderSettings(UnicodeRanges.All))
);

// --- NOTYF CONFIG ---
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;
});

// BUILD APP
var app = builder.Build();

// --- MIDDLEWARE ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// --- ROUTES ---
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
