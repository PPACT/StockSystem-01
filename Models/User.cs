using System.ComponentModel.DataAnnotations;

namespace StockSystem.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "账号不能为空")]
        [MaxLength(20, ErrorMessage = "账号最长20字符")]
        public string Username { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Password { get; set; } = string.Empty;

        [MaxLength(10)]
        public string Role { get; set; } = "user";
    }
}