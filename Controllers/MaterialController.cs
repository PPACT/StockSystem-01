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

        // 列表
        [HttpGet]
        public async Task<ApiResult> GetList()
        {
            var list = await _db.Materials.ToListAsync();
            return ApiResult.Success(list);
        }

        // 新增
        [HttpPost]
        public async Task<ApiResult> Add([FromBody] Material material)
        {
            _db.Materials.Add(material);
            await _db.SaveChangesAsync();
            return ApiResult.Success("添加成功");
        }

        // 修改
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

        // 删除
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