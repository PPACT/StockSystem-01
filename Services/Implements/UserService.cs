using Microsoft.AspNetCore.Identity;
using StockSystem.Models;
using StockSystem.Repository.IRepository;
using StockSystem.Services.IServices;

namespace StockSystem.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepo.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepo.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var userList = await _userRepo.GetAllAsync();
            return userList.FirstOrDefault(u => u.Username == username);
        }

        // 🔥 最终安全版：只验证密文，不兼容明文
        public async Task<User?> LoginAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = await GetUserByUsernameAsync(username);
            if (user == null) return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }

        // 新增：自动加密
        public async Task AddUserAsync(User user)
        {
            if (user == null) throw new ArgumentNullException("用户信息不能为空");
            user.Password = _passwordHasher.HashPassword(user, user.Password);
            await _userRepo.AddAsync(user);
        }

        // 修改：改密码才加密
        public async Task UpdateUserAsync(User user)
        {
            var existUser = await _userRepo.GetByIdAsync(user.Id);
            if (existUser == null) throw new KeyNotFoundException("用户不存在");

            existUser.Username = user.Username;
            existUser.Role = user.Role;

            if (!string.IsNullOrEmpty(user.Password))
            {
                existUser.Password = _passwordHasher.HashPassword(existUser, user.Password);
            }

            await _userRepo.UpdateAsync(existUser);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) throw new KeyNotFoundException("用户不存在");
            await _userRepo.DeleteAsync(user);
        }


    }
}