using Microsoft.EntityFrameworkCore;
using StockSystem.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    // 明确引用带命名空间的Material，而非全局的
    public DbSet<StockSystem.Models.Material> Materials { get; set; }
}