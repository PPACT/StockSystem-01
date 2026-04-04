using System.Threading.Tasks;
using StockSystem.Models;

public interface IMaterialService
{
    Task<object> GetListAsync();
    // 所有Material参数都指定命名空间
    Task<object> AddAsync(Material model);
    Task<object> UpdateAsync(Material model);
    Task<object> DeleteAsync(int id);
}