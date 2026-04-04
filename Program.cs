using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors; // 加这个
using StockSystem.Repositories.IRepository;
using StockSystem.Repositories.Implements;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// -------------- 开启跨域（关键！解决 Failed to fetch）
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

string conn = @"Server=(localdb)\mssqllocaldb;Database=StockDB;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(conn));

builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IMaterialService, MaterialService>();

var app = builder.Build();

app.UseStaticFiles(); // 支持前端页面
app.UseCors("AllowAll"); // 启用跨域
app.MapControllers();

app.MapGet("/", () => Results.Redirect("index.html"));

app.Run();