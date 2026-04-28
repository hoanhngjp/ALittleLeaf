using System.Text;
using ALittleLeaf.Api.Data;
using ALittleLeaf.Api.Repositories.Cart;
using ALittleLeaf.Api.Repositories.Category;
using ALittleLeaf.Api.Repositories.Order;
using ALittleLeaf.Api.Repositories.Product;
using ALittleLeaf.Api.Services.Auth;
using ALittleLeaf.Api.Services.Cart;
using ALittleLeaf.Api.Services.Category;
using ALittleLeaf.Api.Services.Cloudinary;
using ALittleLeaf.Api.Services.Order;
using ALittleLeaf.Api.Services.Product;
using ALittleLeaf.Api.Services.VNPay;
using ALittleLeaf.Api.Repositories.Admin;
using ALittleLeaf.Api.Services.Admin;
using ALittleLeaf.Api.Repositories.Banner;
using ALittleLeaf.Api.Services.Banner;
using ALittleLeaf.Api.Services.Shipping;
using ALittleLeaf.Api.Services.Background;
using ALittleLeaf.Api.Options;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Load .env file if it exists (development convenience)
Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// ── Connection String ──────────────────────────────────────────────────────
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Database connection string is not configured.");

// ── Entity Framework Core ──────────────────────────────────────────────────
builder.Services.AddDbContext<AlittleLeafDecorContext>(options =>
    options.UseSqlServer(connectionString));

// ── JWT Authentication ─────────────────────────────────────────────────────
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("JWT secret is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ── AutoMapper ─────────────────────────────────────────────────────────────
// Mapping profiles are registered here in Phase 3+ as DTOs are introduced.
// AutoMapper v15: use builder.Services.AddAutoMapper(cfg => cfg.AddProfile<YourProfile>())
// when the first Profile class is created.

// ── CORS ───────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",   // Vite dev server
                "http://localhost:3000"    // Docker nginx (production)
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ── GHN config: bridge flat env vars → Ghn:* section ─────────────────────
var ghnApiKey  = Environment.GetEnvironmentVariable("GHN_API_KEY");
var ghnShopId  = Environment.GetEnvironmentVariable("GHN_SHOP_ID");
var ghnBaseUrl = Environment.GetEnvironmentVariable("GHN_BASE_URL");

if (!string.IsNullOrWhiteSpace(ghnApiKey))
    builder.Configuration["Ghn:ApiKey"]  = ghnApiKey;
if (!string.IsNullOrWhiteSpace(ghnShopId))
    builder.Configuration["Ghn:ShopId"] = ghnShopId;
if (!string.IsNullOrWhiteSpace(ghnBaseUrl))
    builder.Configuration["Ghn:BaseUrl"] = ghnBaseUrl;

// ── Google OAuth config: bridge env var → Google:ClientId section ────────
var googleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
if (!string.IsNullOrWhiteSpace(googleClientId))
    builder.Configuration["Google:ClientId"] = googleClientId;

// ── Application Services ───────────────────────────────────────────────────
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddSingleton<IGoogleTokenValidator, GoogleTokenValidator>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ── Product & Category (Phase 4) ───────────────────────────────────────────
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// ── Cart (Phase 5) ─────────────────────────────────────────────────────────
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

// ── Order & Payment (Phase 6) ──────────────────────────────────────────────
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IVnPayService, VnPayService>();

// ── Admin (Phase 7) ────────────────────────────────────────────────────────
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();

// ── Banner (Phase 18) ──────────────────────────────────────────────────────
builder.Services.AddScoped<IBannerRepository, BannerRepository>();
builder.Services.AddScoped<IBannerService, BannerService>();

// GHN Logistics
builder.Services.AddMemoryCache();
builder.Services.Configure<GhnOptions>(builder.Configuration.GetSection(GhnOptions.Section));
builder.Services.AddHttpClient<IGhnService, GhnService>((sp, client) =>
{
    var opts = sp.GetRequiredService<IOptions<GhnOptions>>().Value;
    client.BaseAddress = new Uri(opts.BaseUrl.TrimEnd('/') + "/");
    client.DefaultRequestHeaders.Add("Token", opts.ApiKey);
});

// ── Background Services ────────────────────────────────────────────────────
builder.Services.AddHostedService<ExpiredOrderCancellationService>();

// ── Controllers ────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Swagger / OpenAPI ──────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ALittleLeaf API",
        Version = "v1",
        Description = "REST API for the ALittleLeaf e-commerce platform"
    });

    // Enable JWT bearer auth in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token. Example: eyJhbGci..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ────────────────────────────────────────────────────────────────────────────
var app = builder.Build();
// ────────────────────────────────────────────────────────────────────────────

// ── Swagger UI (all environments for now; restrict in production as needed) ─
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ALittleLeaf API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ── Auto Migration cho Docker ──────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Đang kiểm tra và chạy Migration cho Database...");
        var context = services.GetRequiredService<AlittleLeafDecorContext>();
        
        // Tự động tạo DB và cập nhật các bảng
        context.Database.Migrate(); 
        logger.LogInformation("Migration Database thành công!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Đã xảy ra lỗi khi thực thi Migration vào Database.");
    }
}
// ────────────────────────────────────────────────────────────────────────────

app.Run();