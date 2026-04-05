using StockSystem.Repositories.IRepository;
using StockSystem.Models;

public class MaterialService : IMaterialService
{
    private readonly IMaterialRepository _repo;

    public MaterialService(IMaterialRepository repo)
    {
        _repo = repo;
    }

    public async Task<object> GetListAsync()
    {
        var list = await _repo.GetAllAsync();
        return new { code = 200, data = list };
    }

    // ✅ 安全获取单条，允许返回 null
    public async Task<Material> GetByIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<object> AddAsync(Material model)
    {
        await _repo.AddAsync(model);
        await _repo.SaveAsync();
        return new { code = 200, msg = "新增成功" };
    }

    public async Task<object> UpdateAsync(Material model)
    {
        // ✅ 查库
        var exist = await _repo.GetByIdAsync(model.Id);

        // ✅ 防 null 崩溃！
        if (exist == null)
        {
            return new { code = 500, msg = "物料不存在" };
        }

        // ✅ 赋值更新
        exist.Name = model.Name;
        exist.Code = model.Code;
        exist.StockNumber = model.StockNumber;
        exist.Remark = model.Remark;

        await _repo.UpdateAsync(exist);
        await _repo.SaveAsync();

        return new { code = 200, msg = "修改成功" };
    }

    public async Task<object> DeleteAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null)
        {
            return new { code = 500, msg = "不存在" };
        }

        _repo.Remove(item);
        await _repo.SaveAsync();
        return new { code = 200, msg = "删除成功" };
    }
}