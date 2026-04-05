using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using StockSystem.Models;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class MaterialController : ControllerBase
{
    // 注入Service，不再直接注入DbContext
    private readonly IMaterialService _materialService;

    // 构造函数接收Service（DI自动注入）
    public MaterialController(IMaterialService materialService)
    {
        _materialService = materialService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        // 只调用Service，不写任何逻辑
        var result = await _materialService.GetListAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Add(Material model)
    {
        var result = await _materialService.AddAsync(model);
        return Ok(result);
    }


    [HttpPut("{id}")] // 这里必须加 {id}
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