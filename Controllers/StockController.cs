using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // 从token拿当前登录用户名
        private string GetCurrentUserName()
        {
            return User.FindFirstValue(ClaimTypes.Name) ?? "未知用户";
        }

        // ==============================
        // 入库
        // ==============================
        [HttpPost("In")]
        public async Task<IActionResult> InStock(int materialId, int count, string remark = "")
        {
            if (count <= 0)
                return BadRequest("入库数量必须大于0");

            var material = await _db.Materials.FindAsync(materialId);
            if (material == null)
                return NotFound("物料不存在");

            var before = material.StockNumber;

            using var tran = await _db.Database.BeginTransactionAsync();

            try
            {
                material.StockNumber += count;

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

                return Ok(new { code = 200, msg = "入库成功" });
            }
            catch
            {
                await tran.RollbackAsync();
                return BadRequest("入库失败");
            }
        }

        // ==============================
        // 出库
        // ==============================
        [HttpPost("Out")]
        public async Task<IActionResult> OutStock(int materialId, int count, string remark = "")
        {
            if (count <= 0)
                return BadRequest("出库数量必须大于0");

            var material = await _db.Materials.FindAsync(materialId);
            if (material == null)
                return NotFound("物料不存在");

            if (material.StockNumber < count)
                return BadRequest("库存不足，无法出库");

            var before = material.StockNumber;

            using var tran = await _db.Database.BeginTransactionAsync();

            try
            {
                material.StockNumber -= count;

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

                return Ok(new { code = 200, msg = "出库成功" });
            }
            catch
            {
                await tran.RollbackAsync();
                return BadRequest("出库失败");
            }
        }

        
        // 库存流水日志列表（完整联表+搜索+分页）
        [HttpGet("LogList")]
        public async Task<IActionResult> LogList(int pageIndex = 1, int pageSize = 15, string materialName = "")
        {
            var query = _db.StockLogs
                .Include(x => x.Material)  // 关联查询物料信息
                .OrderByDescending(x => x.OperateTime)
                .AsQueryable();

            // 按物料名称模糊搜索
            if (!string.IsNullOrEmpty(materialName))
            {
                query = query.Where(x => x.Material.Name.Contains(materialName));
            }

            // 总条数
            var total = await query.CountAsync();

            // 分页查询
            var list = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { code = 200, data = list, total });
        }
    }
}