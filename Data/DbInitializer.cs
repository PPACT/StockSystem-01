using Microsoft.EntityFrameworkCore;
using StockSystem.Data;
using StockSystem.Models;

public static class DbInitializer
{
    public static void Seed(AppDbContext db)
    {
        // 强制让 EF 把当前模型同步到数据库（真正能建表）
        db.Database.Migrate();

        // 初始化用户
        if (!db.Users.Any())
        {
            db.Users.Add(new User { Username = "admin", Password = "123456" });
            db.SaveChanges();
        }

        // 初始化物料
        if (!db.Materials.Any())
        {
            db.Materials.Add(new Material
            {
                Name = "测试物料",
                Code = "TEST001",
                StockNumber = 100
            });
            db.SaveChanges();
        }
    }
}