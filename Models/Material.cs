using System.ComponentModel.DataAnnotations;

namespace StockSystem.Models
{
    public class Material
    {
        public int Id { get; set; }

        // 物料名称：最大长度50，必填
        [MaxLength(50, ErrorMessage = "物料名称最长50个字符")]
        [Required(ErrorMessage = "物料名称不能为空")]
        public string Name { get; set; } = string.Empty;

        // 物料编码：最大长度20，必填
        [MaxLength(20, ErrorMessage = "物料编码最长20个字符")]
        [Required(ErrorMessage = "物料编码不能为空")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "物料编码仅支持大写字母和数字")]
        public string Code { get; set; } = string.Empty;

        // 存量：非负整数（可选约束）
        [Range(0, int.MaxValue, ErrorMessage = "存量不能为负数")]
        public int StockNumber { get; set; }

        // 备注：最大长度200，可选（允许null）
        [MaxLength(200, ErrorMessage = "备注最长200个字符")]
        public string? Remark { get; set; }
    }
}