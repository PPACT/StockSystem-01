using Microsoft.AspNetCore.Mvc;
using StockSystem.Common;
using StockSystem.Models;
using StockSystem.Models.Dto;
using StockSystem.Services.IServices;

namespace StockSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ApiResult> GetAll()
        {
            var list = await _userService.GetAllUsersAsync();
            return ApiResult.Success(list);
        }

        // 新增：用户分页+搜索接口
        [HttpGet("page")]
        public async Task<ApiResult> GetPage(
            int pageIndex = 1,
            int pageSize = 10,
            string? username = null)
        {
            var list = await _userService.GetAllUsersAsync();
            var query = list.AsQueryable();

            if (!string.IsNullOrEmpty(username))
                query = query.Where(u => u.Username.Contains(username));

            var total = query.Count();
            var data = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return ApiResult.Success(new PagedResult<User>
            {
                List = data,
                Total = total
            });
        }

        [HttpGet("{id}")]
        public async Task<ApiResult> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return ApiResult.Error("用户不存在");

            return ApiResult.Success(user);
        }

        [HttpPost]
        public async Task<ApiResult> Add(User user)
        {
            if (user.Username.Equals("admin", System.StringComparison.OrdinalIgnoreCase))
                return ApiResult.Error("禁止注册管理员账号");

            await _userService.AddUserAsync(user);
            return ApiResult.Success(msg: "添加成功");
        }

        [HttpPut("{id}")]
        public async Task<ApiResult> Update(int id, [FromBody] UpdateRoleDto dto)
        {
            var existUser = await _userService.GetUserByIdAsync(id);
            if (existUser == null)
                return ApiResult.Error("用户不存在");

            if (existUser.Username.Equals("admin", StringComparison.OrdinalIgnoreCase))
                return ApiResult.Error("管理员账号不可修改");

            existUser.Role = dto.Role;
            await _userService.UpdateUserAsync(existUser);

            return ApiResult.Success("修改成功");
        }

        [HttpDelete("{id}")]
        public async Task<ApiResult> Delete(int id)
        {
            var existUser = await _userService.GetUserByIdAsync(id);
            if (existUser == null)
                return ApiResult.Error("用户不存在");

            if (existUser.Username.Equals("admin", System.StringComparison.OrdinalIgnoreCase))
                return ApiResult.Error("管理员账号不可删除");

            await _userService.DeleteUserAsync(id);
            return ApiResult.Success(msg: "删除成功");
        }

        [HttpPut("MyProfile")]
        public async Task<ApiResult> MyProfile([FromBody] User user)
        {
            var existUser = await _userService.GetUserByIdAsync(user.Id);
            if (existUser == null)
                return ApiResult.Error("用户不存在");

            if (existUser.Username == "admin" && existUser.Username != user.Username)
                return ApiResult.Error("管理员账号不允许修改名称");

            if (user.Username == "admin" && existUser.Username != "admin")
                return ApiResult.Error("不允许设置为管理员账号");

            existUser.Username = user.Username;

            if (!string.IsNullOrEmpty(user.Password))
            {
                existUser.Password = user.Password; // 会在 UpdateUserAsync 里自动加密
            }

            await _userService.UpdateUserAsync(existUser);
            return ApiResult.Success("保存成功");
        }

        
    }
}