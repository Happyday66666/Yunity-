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

// ���U DbContext �A�ȡA���]�z���b�ϥ� SQL Server
builder.Services.AddDbContext<YunityContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
// options.UseSqlServer(connectionString));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ���U�������ҪA��
builder.Services.AddDefaultIdentity<YunityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<YunityContext>();
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

// �]�w��x�O���t�m
builder.Logging.ClearProviders();  // �M���w�]�����Ѫ̡]�p�G���ݭn���ܡ^
builder.Logging.AddConsole();     // �K�[����x��x���Ѫ�

// ���U�غc�̪���L�A��
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<WeatherService>();


builder.Services.AddControllers();

// ���U�t�@�� DbContext �A�� (���]�O�ΨӺ޲z�j�Ӹ��)
builder.Services.AddDbContext<BuildingDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ���U Session �A�Ȩðt�m�ﶵ
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // �]�w Session �L���ɶ�
    options.Cookie.HttpOnly = true;                  // �T�O�u�� HTTP �ШD�i�H�X�� cookie
    options.Cookie.IsEssential = true;               // �]�w�o�� cookie �����n��
});


builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddSingleton<IEmailSender, EmailSender>();

// ���U BatchGeocodingService (Scoped)
builder.Services.AddScoped<BatchGeocodingService>();

// ���U�I���A�� (�۰ʧ�s)
builder.Services.AddHostedService<VendorCoordinatesBackgroundService>();


// ���U HttpClient �u�t (�p�G�|�����U)
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

// �ҥ� Session �����n��
app.UseSession();

// �]�w����
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

// �ҥα������
app.MapControllers();

// �]�w�������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=HomePage}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();