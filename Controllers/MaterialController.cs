using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.XSSF.UserModel;
using StockSystem.Common;
using StockSystem.Data;
using StockSystem.Models;
using System.IO; // 🔥 修复：必须加这个
using System.Collections.Generic; // 🔥 修复：必须加这个

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

            var existCode = await _db.Materials.AnyAsync(m => m.Code == material.Code && m.Id != id);
            if (existCode)
                return ApiResult.Error("物料编码已被其他物料占用");

            item.Name = material.Name;
            item.Code = material.Code;
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

        // ==========================
        // 导出 Excel
        // ==========================
        [HttpGet("Export")]
        public async Task<IActionResult> Export()
        {
            var list = await _db.Materials.ToListAsync();

            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet("物料数据");

            var header = sheet.CreateRow(0);
            header.CreateCell(0).SetCellValue("ID");
            header.CreateCell(1).SetCellValue("物料编码");
            header.CreateCell(2).SetCellValue("物料名称");
            header.CreateCell(3).SetCellValue("当前库存");
            header.CreateCell(4).SetCellValue("备注");

            int rowIndex = 1;
            foreach (var m in list)
            {
                var row = sheet.CreateRow(rowIndex++);
                row.CreateCell(0).SetCellValue(m.Id);
                row.CreateCell(1).SetCellValue(m.Code);
                row.CreateCell(2).SetCellValue(m.Name);
                row.CreateCell(3).SetCellValue(m.StockNumber);
                row.CreateCell(4).SetCellValue(m.Remark);
            }

            var ms = new MemoryStream();
            workbook.Write(ms);
            var bytes = ms.ToArray();

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"物料数据_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        // ==========================
        // 导入 Excel
        // ==========================
        [HttpPost("Import")]
        public async Task<ApiResult> Import(IFormFile file) // 🔥 修复：统一返回 ApiResult
        {
            if (file == null || file.Length == 0)
                return ApiResult.Error("请上传文件");

            var workbook = new XSSFWorkbook(file.OpenReadStream());
            var sheet = workbook.GetSheetAt(0);

            var list = new List<Material>();

            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null) continue;

                var code = row.GetCell(1)?.ToString()?.Trim();
                var name = row.GetCell(2)?.ToString()?.Trim();
                var stockStr = row.GetCell(3)?.ToString()?.Trim();
                var remark = row.GetCell(4)?.ToString()?.Trim();

                if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name))
                    continue;

                var hasSame = await _db.Materials.AnyAsync(x => x.Code == code);
                if (hasSame) continue;

                if (!int.TryParse(stockStr, out int stock))
                    stock = 0;

                list.Add(new Material
                {
                    Code = code,
                    Name = name,
                    StockNumber = stock,
                    Remark = remark
                });
            }

            if (list.Any())
            {
                await _db.Materials.AddRangeAsync(list);
                await _db.SaveChangesAsync();
            }

            // 🔥 修复：统一返回格式
            return ApiResult.Success($"导入成功，共导入 {list.Count} 条数据");
        }
    }
}