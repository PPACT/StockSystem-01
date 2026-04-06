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
builder.Services.AddEndpointsApiExplorer();

// 跨域
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JWT 认证（密钥统一 32 位，不报错）
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        // 32位密钥，和 LoginController 一致
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("12345678901234567890123456789012"))
    };
});

// 数据库连接
string conn = @"Server=(localdb)\mssqllocaldb;Database=StockDB;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(conn));

// 注入你的服务
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IMaterialService, MaterialService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseCors("AllowAll");

// 认证顺序（必须正确）
app.UseAuthentication();
app.UseAuthorization();

// 路由必须在 app.Run() 之前！！！
app.MapControllers();
app.MapGet("/", () => Results.Redirect("login.html"));

// 初始化
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Seed(db);
}
catch { }

// 只留一个 app.Run()！！！
app.Run(); 