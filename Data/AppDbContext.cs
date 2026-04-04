using Microsoft.EntityFrameworkCore;
using StockSystem.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Material> Materials { get; set; }
}