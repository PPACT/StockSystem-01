using System.Collections.Generic;
using System.Threading.Tasks;
using StockSystem.Models;

namespace StockSystem.Repositories.IRepository
{
    public interface IMaterialRepository
    {
        // 所有Material都指定命名空间
        Task<List<StockSystem.Models.Material>> GetAllAsync();
        Task AddAsync(StockSystem.Models.Material m);
        Task UpdateAsync(StockSystem.Models.Material m);
        Task<StockSystem.Models.Material?> GetByIdAsync(int id);
        void Remove(StockSystem.Models.Material m);
        Task SaveAsync();
    }
}