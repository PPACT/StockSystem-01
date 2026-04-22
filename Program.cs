using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StockSystem.Common;
using StockSystem.Data;
using StockSystem.Repository.Implements;
using StockSystem.Repository.IRepository;
using StockSystem.Services.Implements;
using StockSystem.Services.IServices;
using System.Text;

// 🔥 1. 先加这一句（必须在最顶部）
using Serilog;

// 🔥 2. 配置 Serilog（放在 var builder = ... 之前）
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// 🔥 3. 使用 Serilog（必须在 Build 之前）
builder.Host.UseSerilog();

// 1. 控制器 + 全局配置
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>(); // 全局异常
});
builder.Services.AddEndpointsApiExplorer();

// ======================================================
// 2. Swagger API 文档
builder.Services.AddSwaggerGen();

// ======================================================
// 3. 跨域配置
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ======================================================
// 4. JWT 身份验证 【已规范化】
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("12345678901234567890123456789012")
        )
    };
});

// ======================================================
// 5. 数据库上下文
string conn = @"Server=(localdb)\mssqllocaldb;Database=StockDB;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(conn));

// ======================================================
// 6. 注册 JWT 工具类
builder.Services.AddScoped<JwtHelper>();

// ======================================================
// 7. 仓储层注入
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// ======================================================
// 8. 服务层注入
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IUserService, UserService>();

// ======================================================
// 9. 应用启动配置
var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "库存管理系统 API");
        c.RoutePrefix = "swagger";
    });
}

app.UseStaticFiles();
app.UseCors("AllowAll");

// 认证 & 授权 【顺序不能变】
app.UseAuthentication();
app.UseAuthorization();

// 🔥 4. 启用 Serilog 请求日志（放在中间件最后，Run 之前）
app.UseSerilogRequestLogging();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("login.html"));

// 初始化数据
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Seed(db);
}
catch { }

app.Run();