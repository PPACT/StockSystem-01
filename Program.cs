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
using Serilog;

// 日志配置
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// JWT 密钥解析（环境变量优先，配置文件兜底）
// Docker:  docker run -e JWT_SECRET="xxx"
// 本地开发: 在 appsettings.Development.json 中配置 Jwt:SecretKey
string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                   ?? builder.Configuration["Jwt:SecretKey"];
if (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Length < 16)
    throw new InvalidOperationException(
        "JWT 密钥未配置或长度不足。请设置环境变量 JWT_SECRET（至少 16 字符），" +
        "或在 appsettings.Development.json 中配置 Jwt:SecretKey。");
builder.Configuration["Jwt:SecretKey"] = jwtSecret;

// 使用Serilog
builder.Host.UseSerilog();

// 控制器 + 全局异常
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen();

// 跨域
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost", "http://localhost:80")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// JWT
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
            Encoding.UTF8.GetBytes(jwtSecret)
        )
    };
});

// ======================================================
// 5. 数据库上下文
// 优先从环境变量读取（Docker 环境），否则使用本地 LocalDB
string conn = Environment.GetEnvironmentVariable("CONNECTION_STRING")
              ?? @"Server=(localdb)\mssqllocaldb;Database=StockDB;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(conn));

// 工具类
builder.Services.AddScoped<JwtHelper>();

// 仓储
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// 服务
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IUserService, UserService>();

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

app.UseAuthentication();
app.UseAuthorization();
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
catch (Exception ex)
{
    Log.Error(ex, "数据库初始化失败");
}

app.Run();