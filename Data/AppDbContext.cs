using Microsoft.EntityFrameworkCore;
// 关键：引用Material模型的命名空间
using StockSystem.Models;

// 无命名空间，保持原有结构
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    // 明确引用带命名空间的Material，而非全局的
    public DbSet<StockSystem.Models.Material> Materials { get; set; }
}