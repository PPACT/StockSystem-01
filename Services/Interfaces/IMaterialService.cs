using System.Threading.Tasks;
using StockSystem.Models;

public interface IMaterialService
{
    Task<object> GetListAsync();
    Task<Material> GetByIdAsync(int id); // 👈 加这个
    Task<object> AddAsync(Material model);
    Task<object> UpdateAsync(Material model);
    Task<object> DeleteAsync(int id);
}