using System.Collections.Generic;
using System.Threading.Tasks;
using StockSystem.Models;

namespace StockSystem.Repositories.IRepository
{
    public interface IMaterialRepository
    {
        
        Task<List<Material>> GetAllAsync();
        Task AddAsync(Material m);
        Task UpdateAsync(Material m);
        Task<Material?> GetByIdAsync(int id);
        void Remove(Material m);
        Task SaveAsync();
    }
}