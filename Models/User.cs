using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty; // 账号不可空

    [MaxLength(255)]
    public string? Password { get; set; } = string.Empty;
    public string Role { get; set; } = "user"; // 角色不可空，默认普通用户
}