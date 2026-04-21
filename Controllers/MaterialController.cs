using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockSystem.Common;
using StockSystem.Data;
using StockSystem.Models;

namespace StockSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public MaterialsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ApiResult> GetList()
        {
            var list = await _db.Materials.ToListAsync();
            return ApiResult.Success(list);
        }

        [HttpGet("page")]
        public async Task<ApiResult> GetPage(
            int pageIndex = 1,
            int pageSize = 10,
            string? name = null,
            string? code = null)
        {
            var query = _db.Materials.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(m => m.Name.Contains(name));

            if (!string.IsNullOrEmpty(code))
                query = query.Where(m => m.Code.Contains(code));

            var total = await query.CountAsync();
            var data = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return ApiResult.Success(new PagedResult<Material>
            {
                List = data,
                Total = total
            });
        }

        [HttpPost]
        public async Task<ApiResult> Add([FromBody] Material material)
        {
            // 🔥 校验：物料编码不能重复
            var existCode = await _db.Materials.AnyAsync(m => m.Code == material.Code);
            if (existCode)
                return ApiResult.Error("物料编码已存在，请更换");

            _db.Materials.Add(material);
            await _db.SaveChangesAsync();
            return ApiResult.Success("添加成功");
        }

        [HttpPut("{id}")]
        public async Task<ApiResult> Update(int id, [FromBody] Material material)
        {
            var item = await _db.Materials.FindAsync(id);
            if (item == null) return ApiResult.Error("物料不存在");

            // 🔥 校验：不能改成别人已用的编码
            var existCode = await _db.Materials.AnyAsync(m => m.Code == material.Code && m.Id != id);
            if (existCode)
                return ApiResult.Error("物料编码已被其他物料占用");

            item.Name = material.Name;
            item.Code = material.Code;
            item.StockNumber = material.StockNumber;
            item.Remark = material.Remark;

            await _db.SaveChangesAsync();
            return ApiResult.Success("修改成功");
        }

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