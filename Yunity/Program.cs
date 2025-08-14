using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using WebPWrecover.Services;
using Yunity.Areas.Identity.Data;
using Yunity.Data;
using Yunity.Models;
using Yunity.Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 註冊 DbContext 服務，假設您正在使用 SQL Server
builder.Services.AddDbContext<YunityContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
// options.UseSqlServer(connectionString));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 註冊身份驗證服務
builder.Services.AddDefaultIdentity<YunityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<YunityContext>();
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

// 設定日誌記錄配置
builder.Logging.ClearProviders();  // 清除預設的提供者（如果有需要的話）
builder.Logging.AddConsole();     // 添加控制台日誌提供者

// 註冊建構者的其他服務
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<WeatherService>();


builder.Services.AddControllers();

// 註冊另一個 DbContext 服務 (假設是用來管理大樓資料)
builder.Services.AddDbContext<BuildingDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 註冊 Session 服務並配置選項
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // 設定 Session 過期時間
    options.Cookie.HttpOnly = true;                  // 確保只有 HTTP 請求可以訪問 cookie
    options.Cookie.IsEssential = true;               // 設定這個 cookie 為必要的
});


builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddSingleton<IEmailSender, EmailSender>();

// 註冊 BatchGeocodingService (Scoped)
builder.Services.AddScoped<BatchGeocodingService>();

// 註冊背景服務 (自動更新)
builder.Services.AddHostedService<VendorCoordinatesBackgroundService>();


// 註冊 HttpClient 工廠 (如果尚未註冊)
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 啟用 Session 中介軟體
app.UseSession();

// 設定路由
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
    context.Response.Headers.Add("Pragma", "no-cache");
    context.Response.Headers.Add("Expires", "0");
    context.Response.Headers.Add("Content-Type", "text/html; charset=utf-8");

    await next();
});

// 啟用控制器路由
app.MapControllers();

// 設定控制器路由
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=HomePage}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();