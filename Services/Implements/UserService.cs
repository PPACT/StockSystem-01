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
            var exist = await _userRepo.GetByIdAsync(user.Id);
            if (exist == null)
                throw new KeyNotFoundException("用户不存在");

            await _userRepo.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("用户不存在，无法删除");

            await _userRepo.DeleteAsync(user);
        }
    }
}