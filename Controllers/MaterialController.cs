using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockSystem.Common;
using StockSystem.Data;
using StockSystem.Models;

namespace StockSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public MaterialsController(AppDbContext db)
        {
            _db = db;
        }

        // 原有列表（保留不动）
        [HttpGet]
        public async Task<ApiResult> GetList()
        {
            var list = await _db.Materials.ToListAsync();
            return ApiResult.Success(list);
        }

        // ==================== 新增：分页 + 搜索 ====================
        [HttpGet("page")]
        public async Task<ApiResult> GetPage(
            int pageIndex = 1,
            int pageSize = 10,
            string? name = null,
            string? code = null)
        {
            // 1. 基础查询
            var query = _db.Materials.AsQueryable();

            // 2. 条件搜索
            if (!string.IsNullOrEmpty(name))
                query = query.Where(m => m.Name.Contains(name));

            if (!string.IsNullOrEmpty(code))
                query = query.Where(m => m.Code.Contains(code));

            // 3. 总数
            var total = await query.CountAsync();

            // 4. 分页
            var data = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 5. 返回分页格式
            return ApiResult.Success(new PagedResult<Material>
            {
                List = data,
                Total = total
            });
        }

        // 原有新增（不动）
        [HttpPost]
        public async Task<ApiResult> Add([FromBody] Material material)
        {
            _db.Materials.Add(material);
            await _db.SaveChangesAsync();
            return ApiResult.Success("添加成功");
        }

        // 原有修改（不动）
        [HttpPut("{id}")]
        public async Task<ApiResult> Update(int id, [FromBody] Material material)
        {
            var item = await _db.Materials.FindAsync(id);
            if (item == null) return ApiResult.Error("物料不存在");

            item.Name = material.Name;
            item.Code = material.Code;
            item.StockNumber = material.StockNumber;
            item.Remark = material.Remark;

            await _db.SaveChangesAsync();
            return ApiResult.Success("修改成功");
        }

        // 原有删除（不动）
        [HttpDelete("{id}")]
        public async Task<ApiResult> Delete(int id)
        {
            var item = await _db.Materials.FindAsync(id);
            if (item == null) return ApiResult.Error("物料不存在");

            _db.Materials.Remove(item);
            await _db.SaveChangesAsync();
            return ApiResult.Success("删除成功");
        }
    }
}