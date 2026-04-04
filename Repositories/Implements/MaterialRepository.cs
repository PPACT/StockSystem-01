using Microsoft.EntityFrameworkCore;
using StockSystem.Models;
using StockSystem.Repositories.IRepository;

namespace StockSystem.Repositories.Implements
{
    public class MaterialRepository : IMaterialRepository
    {
        // AppDbContext无命名空间，直接用，无需using
        private readonly AppDbContext _db;

        public MaterialRepository(AppDbContext db)
        {
            _db = db;
        }

        // 所有Material都明确指定命名空间，避免歧义
        public async Task<List<StockSystem.Models.Material>> GetAllAsync()
        {
            return await _db.Materials.ToListAsync<StockSystem.Models.Material>();
        }

        public async Task AddAsync(StockSystem.Models.Material m)
        {
            _db.Materials.Add(m);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(StockSystem.Models.Material m)
        {
            _db.Materials.Update(m);
            await _db.SaveChangesAsync();
        }

        public async Task<StockSystem.Models.Material?> GetByIdAsync(int id)
        {
            return await _db.Materials.FindAsync(id);
        }

        public void Remove(StockSystem.Models.Material m)
        {
            _db.Materials.Remove(m);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}