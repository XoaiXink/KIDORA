using AspNetCoreHero.ToastNotification;
using KIDORA.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// --- READ appsettings.json ---
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// --- GET CONNECTION STRING ---
var connectionString = builder.Configuration.GetConnectionString("KidoraDB");
Console.WriteLine($"Connection string: {connectionString}");

// --- ADD DB CONTEXT (QUAN TRỌNG NHẤT) ---
builder.Services.AddDbContext<KidoraDbContext>(options =>
    options.UseSqlServer(connectionString)
);

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

//using AspNetCoreHero.ToastNotification;
//using System.Text.Encodings.Web;
//using System.Text.Unicode;


//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
//builder.Services.AddSingleton<HtmlEncoder>(
//    HtmlEncoder.Create(new TextEncoderSettings(UnicodeRanges.All))
//);

//builder.Services.AddNotyf(config =>
//{
//    config.DurationInSeconds = 10;
//    config.IsDismissable = true;
//    config.Position = NotyfPosition.BottomRight;
//});

//builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
//var connectionString = builder.Configuration.GetConnectionString("KidoraDB");
//Console.WriteLine($"Connection string: {connectionString}");

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();
//app.MapControllerRoute(
//    name: "areas",
//    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
//);

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();