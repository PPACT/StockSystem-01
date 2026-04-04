using System.Threading.Tasks;
// 引用Material模型命名空间
using StockSystem.Models;

// 无命名空间，保持原有结构
public interface IMaterialService
{
    Task<object> GetListAsync();
    // 所有Material参数都指定命名空间
    Task<object> AddAsync(StockSystem.Models.Material model);
    Task<object> UpdateAsync(StockSystem.Models.Material model);
    Task<object> DeleteAsync(int id);
}