using StockSystem.Repositories.IRepository;

public class MaterialService : IMaterialService
{
    private readonly IMaterialRepository _repo;

    public MaterialService(IMaterialRepository repo)
    {
        _repo = repo;
    }

    public async Task<object> GetListAsync()
    {
        // 接收带命名空间的Material列表
        var list = await _repo.GetAllAsync();
        return new { code = 200, data = list };
    }

    public async Task<object> AddAsync(StockSystem.Models.Material model)
    {
        await _repo.AddAsync(model);
        return new { code = 200, msg = "新增成功" };
    }

    public async Task<object> UpdateAsync(StockSystem.Models.Material model)
    {
        await _repo.UpdateAsync(model);
        return new { code = 200, msg = "修改成功" };
    }

    public async Task<object> DeleteAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        if (item == null) return new { code = 500, msg = "不存在" };

        _repo.Remove(item);
        await _repo.SaveAsync();
        return new { code = 200, msg = "删除成功" };
    }
}