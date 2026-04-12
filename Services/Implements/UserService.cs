using StockSystem.Models;
using StockSystem.Repository.IRepository;
using StockSystem.Services.IServices;

namespace StockSystem.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepo.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepo.GetByIdAsync(id);
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            return await _userRepo.LoginAsync(username, password);
        }

        public async Task AddUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("用户信息不能为空");

            await _userRepo.AddAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            // Controller层已经提前校验+查库拿到完整user了，这里直接更新！不二次查库！
            await _userRepo.UpdateAsync(user);
        }

        // ✅ 修复：Delete 传入 user 而不是 id（兼容你的仓储）
        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("用户不存在，无法删除");

            await _userRepo.DeleteAsync(user); // ✅ 这里传 user，不报错！
        }
    }
}