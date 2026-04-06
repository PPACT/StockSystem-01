using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StockSystem.Data;
using StockSystem.Repositories.Implements;
using StockSystem.Repositories.IRepository;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// -------------- 开启跨域
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// =============== JWT 鉴权配置 START ===============
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        // 密钥（必须和 LoginController 里的一致）
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySecretKey1234567890123456"))
    };
});
// =============== JWT 鉴权配置 END ===============

string conn = @"Server=(localdb)\mssqllocaldb;Database=StockDB;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(conn));

builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IMaterialService, MaterialService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseCors("AllowAll");

// =============== 必须加这两行，顺序不能乱！===============
app.UseAuthentication(); // 先登录验证
app.UseAuthorization();  // 后权限校验
// ======================================================

app.MapControllers();

app.MapGet("/", () => Results.Redirect("index.html"));

app.Run();