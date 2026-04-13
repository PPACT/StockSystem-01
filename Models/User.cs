using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "账号不能为空")]
    [MaxLength(20)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    [MaxLength(20)]
    public string Password { get; set; } = string.Empty;

    // 角色：admin / user
    [MaxLength(20)]
    public string Role { get; set; } = "user";
}