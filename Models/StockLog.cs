using System;

namespace StockSystem.Models
{
    public class StockLog
    {
        public int Id { get; set; }

        // 物料ID
        public int MaterialId { get; set; }

        // 操作类型：入库 In / 出库 Out
        public string OperateType { get; set; }

        // 变动数量
        public int ChangeCount { get; set; }

        // 操作前库存
        public int BeforeStock { get; set; }

        // 操作后库存
        public int AfterStock { get; set; }

        // 操作人（从token里拿）
        public string OperateUser { get; set; }

        // 操作时间
        public DateTime OperateTime { get; set; }

        // 备注（可选）
        public string Remark { get; set; }

        // 导航属性
        public Material Material { get; set; }
    }
}