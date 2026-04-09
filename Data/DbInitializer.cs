using StockSystem.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext db)
    {
        db.Database.EnsureCreated();

        // 只有表完全空的时候，才插入默认 admin
        if (!db.Users.Any())
        {
            db.Users.Add(new User
            {
                Username = "admin",
                Password = "123456",
                Role = "admin"  // 直接标记是管理员
            });
            db.SaveChanges();
        }
    }
}