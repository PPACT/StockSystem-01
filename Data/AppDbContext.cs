using Microsoft.EntityFrameworkCore;
using StockSystem.Models;

namespace StockSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 物料表
        public DbSet<Material> Materials { get; set; }

        // 用户表
        public DbSet<User> Users { get; set; }
    }
}