using Microsoft.AspNetCore.Mvc;
using StockSystem.Common;
using StockSystem.Models;
using StockSystem.Services.IServices;

namespace StockSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        public MaterialsController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet]
        public async Task<ApiResult> GetAll()
        {
            var list = await _materialService.GetAllMaterialsAsync();
            return ApiResult.Success(list);
        }

        [HttpGet("{id}")]
        public async Task<ApiResult> GetById(int id)
        {
            var model = await _materialService.GetMaterialByIdAsync(id);
            if (model == null) return ApiResult.Error("物料不存在");
            return ApiResult.Success(model);
        }

        [HttpPost]
        public async Task<ApiResult> Add(Material material)
        {
            await _materialService.AddMaterialAsync(material);
            return ApiResult.Success(msg: "添加成功");
        }

        [HttpPut]
        public async Task<ApiResult> Update(Material material)
        {
            await _materialService.UpdateMaterialAsync(material);
            return ApiResult.Success(msg: "修改成功");
        }

        [HttpDelete("{id}")]
        public async Task<ApiResult> Delete(int id)
        {
            await _materialService.DeleteMaterialAsync(id);
            return ApiResult.Success(msg: "删除成功");
        }
    }
}