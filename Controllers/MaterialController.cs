using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockSystem.Models;
using System.Threading.Tasks;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MaterialController : ControllerBase
{
    private readonly IMaterialService _materialService;

    public MaterialController(IMaterialService materialService)
    {
        _materialService = materialService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var result = await _materialService.GetListAsync();
        return Ok(result);
    }

    // ✅ 正确获取单条数据（给修改弹窗用）
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _materialService.GetByIdAsync(id);

        // ✅ 如果 null，返回 404，前端不会报错
        if (item == null)
        {
            return NotFound(new { code = 404, msg = "物料不存在" });
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Add(Material model)
    {
        var result = await _materialService.AddAsync(model);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Material model)
    {
        if (id != model.Id) return BadRequest("参数不匹配");

        var result = await _materialService.UpdateAsync(model);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _materialService.DeleteAsync(id);
        return Ok(result);
    }
}