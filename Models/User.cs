namespace StockSystem.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // 验证管理员
    public bool IsAdmin { get; set; } = false;
}