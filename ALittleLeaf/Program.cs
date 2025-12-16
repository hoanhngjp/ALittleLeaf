using ALittleLeaf.Repository;
using ALittleLeaf.Services.Auth;
using ALittleLeaf.Services.Cart;
using ALittleLeaf.Services.Order;
using ALittleLeaf.Services.Product;
using ALittleLeaf.Services.VNPay;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IPasswordHasher<string>, PasswordHasher<string>>();


builder.Services.AddHttpContextAccessor();

// Thêm dịch vụ cho session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian session tồn tại
    options.Cookie.HttpOnly = true; // Bảo mật cookie
    options.Cookie.IsEssential = true; // Bắt buộc session
});

var key = Encoding.ASCII.GetBytes(builder.Configuration["JWT_SECRET"] ?? "SecretKeyMacDinhDaiHon32KyTu_NeuKhongTimThayTrongEnv");

// Cấu hình Cookie Authentication
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Cookies["X-Access-Token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

var connectionString = builder.Configuration["DB_CONNECTION_STRING"];

Console.WriteLine("Connection String: " + connectionString);

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Không tìm thấy chuỗi kết nối 'DB_CONNECTION_STRING' trong biến môi trường hoặc file .env");
}

builder.Services.AddDbContext<AlittleLeafDecorContext>(options =>
    options.UseSqlServer(connectionString));

// Thêm dịch vụ MVC
builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
