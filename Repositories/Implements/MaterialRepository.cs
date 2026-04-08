using Microsoft.EntityFrameworkCore;
using StockSystem.Data;
using StockSystem.Models;
using StockSystem.Repository.IRepository;

namespace StockSystem.Repository.Implements
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly AppDbContext _db;

        public MaterialRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Material>> GetAllAsync()
        {
            return await _db.Materials.ToListAsync();
        }

        public async Task<Material?> GetByIdAsync(int id)
        {
            return await _db.Materials.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Material material)
        {
            await _db.Materials.AddAsync(material);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Material material)
        {
            _db.Materials.Update(material);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Material material)
        {
            _db.Materials.Remove(material);
            await _db.SaveChangesAsync();
        }
    }
}