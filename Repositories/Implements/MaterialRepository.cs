using Microsoft.EntityFrameworkCore;
using StockSystem.Models;
using StockSystem.Repositories.IRepository;

namespace StockSystem.Repositories.Implements
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
            return await _db.Materials.ToListAsync<Material>();
        }

        public async Task AddAsync(Material m)
        {
            _db.Materials.Add(m);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Material m)
        {
            _db.Materials.Update(m);
            await _db.SaveChangesAsync();
        }

        public async Task<Material?> GetByIdAsync(int id)
        {
            return await _db.Materials.FindAsync(id);
        }

        public void Remove(Material m)
        {
            _db.Materials.Remove(m);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}