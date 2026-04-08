using StockSystem.Models;

namespace StockSystem.Services.IServices
{
    public interface IMaterialService
    {
        Task<List<Material>> GetAllMaterialsAsync();
        Task<Material?> GetMaterialByIdAsync(int id);
        Task AddMaterialAsync(Material material);
        Task UpdateMaterialAsync(Material material);
        Task DeleteMaterialAsync(int id);
    }
}