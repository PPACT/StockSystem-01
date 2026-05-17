using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockSystem.Common;
using StockSystem.Data;
using StockSystem.Models;
using System.Security.Claims;
using System.Text;

namespace StockSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StockController(AppDbContext _db)
        {
            this._db = _db;
        }

        private string GetCurrentUserName()
        {
            return User.FindFirstValue(ClaimTypes.Name) ?? "未知用户";
        }

        [HttpPost("In")]
        public async Task<ApiResult> InStock(int materialId, int count, string remark = "")
        {
            if (count <= 0)
                return ApiResult.Error("入库数量必须大于0");

            var material = await _db.Materials.FindAsync(materialId);
            if (material == null)
                return ApiResult.Error("物料不存在");

            var before = material.StockNumber;

            // 开启事务
            using var tran = await _db.Database.BeginTransactionAsync();

            try
            {
                // 修改库存
                material.StockNumber += count;

                // 写入库存日志
                var log = new StockLog
                {
                    MaterialId = materialId,
                    OperateType = "In",
                    ChangeCount = count,
                    BeforeStock = before,
                    AfterStock = material.StockNumber,
                    OperateUser = GetCurrentUserName(),
                    OperateTime = DateTime.Now,
                    Remark = remark
                };

                _db.StockLogs.Add(log);
                await _db.SaveChangesAsync();
                await tran.CommitAsync();

                return ApiResult.Success("入库成功");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return ApiResult.Error("入库失败：" + ex.Message);
            }
        }

        [HttpPost("Out")]
        public async Task<ApiResult> OutStock(int materialId, int count, string remark = "")
        {
            if (count <= 0)
                return ApiResult.Error("出库数量必须大于0");

            var material = await _db.Materials.FindAsync(materialId);
            if (material == null)
                return ApiResult.Error("物料不存在");

            if (material.StockNumber < count)
                return ApiResult.Error("库存不足，无法出库");

            var before = material.StockNumber;

            // 开启事务
            using var tran = await _db.Database.BeginTransactionAsync();

            try
            {
                // 修改库存
                material.StockNumber -= count;

                // 写入库存日志
                var log = new StockLog
                {
                    MaterialId = materialId,
                    OperateType = "Out",
                    ChangeCount = count,
                    BeforeStock = before,
                    AfterStock = material.StockNumber,
                    OperateUser = GetCurrentUserName(),
                    OperateTime = DateTime.Now,
                    Remark = remark
                };

                _db.StockLogs.Add(log);
                await _db.SaveChangesAsync();
                await tran.CommitAsync();

                return ApiResult.Success("出库成功");
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                return ApiResult.Error("出库失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 日志列表（支持 物料名 + 操作类型筛选）
        /// </summary>
        [HttpGet("LogList")]
        public async Task<IActionResult> LogList(
            int pageIndex = 1,
            int pageSize = 15,
            string materialName = "",
            string? operateType = null)
        {
            var query = _db.StockLogs
                .Include(x => x.Material)
                .OrderByDescending(x => x.OperateTime)
                .AsQueryable();

            // 物料名称筛选
            if (!string.IsNullOrEmpty(materialName))
            {
                query = query.Where(x => x.Material!.Name.Contains(materialName));
            }

            // 出入库类型筛选
            if (!string.IsNullOrEmpty(operateType))
            {
                query = query.Where(x => x.OperateType == operateType);
            }

            var total = await query.CountAsync();
            var list = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { code = 200, data = list, total });
        }

        /// <summary>
        /// 导出库存日志 Excel
        /// </summary>
        [HttpGet("ExportLog")]
        public async Task<IActionResult> ExportLog(string materialName = "", string? operateType = null)
        {
            var query = _db.StockLogs.Include(x => x.Material).AsQueryable();

            if (!string.IsNullOrEmpty(materialName))
                query = query.Where(x => x.Material!.Name.Contains(materialName));

            if (!string.IsNullOrEmpty(operateType))
                query = query.Where(x => x.OperateType == operateType);

            var data = await query.OrderByDescending(x => x.OperateTime).ToListAsync();

            // 拼接csv文本，简单高效导出
            var sb = new StringBuilder();
            sb.AppendLine("物料名称,操作类型,变动数量,操作前库存,操作后库存,操作人,操作时间,备注");

            foreach (var item in data)
            {
                var typeText = item.OperateType == "In" ? "入库" : "出库";
                sb.AppendLine($"{item.Material?.Name},{typeText},{item.ChangeCount},{item.BeforeStock},{item.AfterStock},{item.OperateUser},{item.OperateTime:yyyy-MM-dd HH:mm:ss},{item.Remark}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"库存操作日志_{DateTime.Now:yyyyMMddHHmmss}.csv");
        }
    }
}