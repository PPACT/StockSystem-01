using StockSystem.Models;
using StockSystem.Repository.IRepository;
using StockSystem.Services.IServices;

namespace StockSystem.Services.Implements
{
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _materialRepo;

        public MaterialService(IMaterialRepository materialRepo)
        {
            _materialRepo = materialRepo;
        }

        public async Task<List<Material>> GetAllMaterialsAsync()
        {
            return await _materialRepo.GetAllAsync();
        }

        public async Task<Material?> GetMaterialByIdAsync(int id)
        {
            return await _materialRepo.GetByIdAsync(id);
        }

        public async Task AddMaterialAsync(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("物料信息不能为空");

            await _materialRepo.AddAsync(material);
        }

        public async Task UpdateMaterialAsync(Material material)
        {
            var exist = await _materialRepo.GetByIdAsync(material.Id);
            if (exist == null)
                throw new KeyNotFoundException("物料不存在");

            await _materialRepo.UpdateAsync(material);
        }

        public async Task DeleteMaterialAsync(int id)
        {
            var material = await _materialRepo.GetByIdAsync(id);
            if (material == null)
                throw new KeyNotFoundException("物料不存在，无法删除");

            await _materialRepo.DeleteAsync(material);
        }
    }
}