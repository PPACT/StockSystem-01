using Microsoft.AspNetCore.Identity;
using StockSystem.Data;
using StockSystem.Models;

public static class DbInitializer
{
    public static void Seed(AppDbContext db)
    {
        db.Database.EnsureCreated();

        if (!db.Users.Any())
        {
            var hasher = new PasswordHasher<User>();

            string adminPassword = Environment.GetEnvironmentVariable("STOCK_ADMIN_PASSWORD");
            bool isDefaultPassword = false;
            if (string.IsNullOrEmpty(adminPassword))
            {
                adminPassword = Guid.NewGuid().ToString("N")[..16];
                isDefaultPassword = true;
            }

            var admin = new User
            {
                Username = "admin",
                Role = "admin"
            };
            admin.Password = hasher.HashPassword(admin, adminPassword);
            db.Users.Add(admin);
            db.SaveChanges();

            if (isDefaultPassword)
            {
                Console.WriteLine("=================================================");
                Console.WriteLine("  安全警告：未设置 STOCK_ADMIN_PASSWORD 环境变量");
                Console.WriteLine($"  已生成随机管理员密码: {adminPassword}");
                Console.WriteLine("  请立即登录并修改密码，或设置环境变量后重建数据库");
                Console.WriteLine("=================================================");
            }
        }
    }
}