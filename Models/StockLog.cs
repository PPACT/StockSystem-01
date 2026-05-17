using System;
using System.ComponentModel.DataAnnotations;

namespace StockSystem.Models
{
    public class StockLog
    {
        public int Id { get; set; }

        // 物料ID
        [Required(ErrorMessage = "物料ID不能为空")]
        [Range(1, int.MaxValue, ErrorMessage = "物料ID必须大于0")]
        public int MaterialId { get; set; }

        // 操作类型：入库 In / 出库 Out
        [Required(ErrorMessage = "操作类型不能为空")]
        [MaxLength(10, ErrorMessage = "操作类型长度不能超过10")]
        public string OperateType { get; set; } = string.Empty;

        // 变动数量
        [Required(ErrorMessage = "变动数量不能为空")]
        [Range(1, int.MaxValue, ErrorMessage = "变动数量必须大于0")]
        public int ChangeCount { get; set; }

        // 操作前库存
        [Range(0, int.MaxValue, ErrorMessage = "操作前库存不能小于0")]
        public int BeforeStock { get; set; }

        // 操作后库存
        [Range(0, int.MaxValue, ErrorMessage = "操作后库存不能小于0")]
        public int AfterStock { get; set; }

        // 操作人（从token里拿）
        [Required(ErrorMessage = "操作人不能为空")]
        [MaxLength(50, ErrorMessage = "操作人名称长度不能超过50")]
        public string OperateUser { get; set; } = string.Empty;

        // 操作时间
        public DateTime OperateTime { get; set; }

        // 备注（可选）
        [MaxLength(200, ErrorMessage = "备注长度不能超过200")]
        public string? Remark { get; set; }

        // 导航属性
        public Material? Material { get; set; }
    }
}