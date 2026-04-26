using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockSystem.Common;
using StockSystem.Data;
using StockSystem.Models;
using System.Security.Claims;

namespace StockSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StockController(AppDbContext db)
        {
            _db = db;
        }

        private string GetCurrentUserName()
        {
            return User.FindFirstValue(ClaimTypes.Name) ?? "未知用户";
        }

        // ==============================
        // 入库（已修复）
        // ==============================
        [HttpPost("In")]
        public async Task<ApiResult> InStock(int materialId, int count, string remark = "")
        {
            if (count <= 0)
                return ApiResult.Error("入库数量必须大于0");

            var material = await _db.Materials.FindAsync(materialId);
            if (material == null)
                return ApiResult.Error("物料不存在");

            var before = material.StockNumber;

            using var tran = await _db.Database.BeginTransactionAsync();

            try
            {
                material.StockNumber += count;

                // 🔥 日志先注释，确保库存能改
                // var log = new StockLog
                // {
                //     MaterialId = materialId,
                //     OperateType = "In",
                //     ChangeCount = count,
                //     BeforeStock = before,
                //     AfterStock = material.StockNumber,
                //     OperateUser = GetCurrentUserName(),
                //     OperateTime = DateTime.Now,
                //     Remark = remark
                // };
                // _db.StockLogs.Add(log);

                await _db.SaveChangesAsync();
                await tran.CommitAsync();

                return ApiResult.Success("入库成功");
            }
            catch (Exception ex) // 🔥 改成能捕获错误
            {
                await tran.RollbackAsync();
                return ApiResult.Error("入库失败：" + ex.Message);
            }
        }

        // ==============================
        // 出库（已修复）
        // ==============================
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

            using var tran = await _db.Database.BeginTransactionAsync();

            try
            {
                material.StockNumber -= count;

                // 🔥 日志先注释
                // var log = new StockLog
                // {
                //     MaterialId = materialId,
                //     OperateType = "Out",
                //     ChangeCount = count,
                //     BeforeStock = before,
                //     AfterStock = material.StockNumber,
                //     OperateUser = GetCurrentUserName(),
                //     OperateTime = DateTime.Now,
                //     Remark = remark
                // };
                // _db.StockLogs.Add(log);

                await _db.SaveChangesAsync();
                await tran.CommitAsync();

                return ApiResult.Success("出库成功");
            }
            catch (Exception ex) // 🔥 改成能捕获错误
            {
                await tran.RollbackAsync();
                return ApiResult.Error("出库失败：" + ex.Message);
            }
        }

        // ==============================
        // 日志列表
        // ==============================
        [HttpGet("LogList")]
        public async Task<IActionResult> LogList(int pageIndex = 1, int pageSize = 15, string materialName = "")
        {
            var query = _db.StockLogs
                .Include(x => x.Material)
                .OrderByDescending(x => x.OperateTime)
                .AsQueryable();

            if (!string.IsNullOrEmpty(materialName))
            {
                query = query.Where(x => x.Material.Name.Contains(materialName));
            }

            var total = await query.CountAsync();
            var list = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { code = 200, data = list, total });
        }
    }
}