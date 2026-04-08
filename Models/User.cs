using System.ComponentModel.DataAnnotations;

namespace StockSystem.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "账号不能为空")]
        [MaxLength(20, ErrorMessage = "账号最长20个字符")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "密码不能为空")]
        [MaxLength(20, ErrorMessage = "密码最长20个字符")]
        public string Password { get; set; } = string.Empty;
    }
}