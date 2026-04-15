using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty; // 账号不可空
    public string? Password { get; set; } // 密码可空，解决400
    public string Role { get; set; } = "user"; // 角色不可空，默认普通用户
}