using Microsoft.AspNetCore.Mvc;
using StockSystem.Common;
using StockSystem.Models;
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

        // 获取全部用户
        [HttpGet]
        public async Task<ApiResult> GetAll()
        {
            var list = await _userService.GetAllUsersAsync();
            return ApiResult.Success(list);
        }

        // 根据ID获取用户
        [HttpGet("{id}")]
        public async Task<ApiResult> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return ApiResult.Error("用户不存在");

            return ApiResult.Success(user);
        }

        // 新增用户（禁止注册 admin）
        [HttpPost]
        public async Task<ApiResult> Add(User user)
        {
            // 安全规则：禁止注册 admin 账号
            if (user.Username.Equals("admin", System.StringComparison.OrdinalIgnoreCase))
            {
                return ApiResult.Error("禁止注册管理员账号");
            }

            await _userService.AddUserAsync(user);
            return ApiResult.Success(msg: "添加成功");
        }

        // 修改用户（管理员账号不可修改）
        // 🔥 修复：路由带 {id} + 加 [FromBody] + 只更新角色
        [HttpPut("{id}")]
        public async Task<ApiResult> Update(int id, [FromBody] User user)
        {
            // 1. 先查要修改的用户
            var existUser = await _userService.GetUserByIdAsync(id);
            if (existUser == null)
                return ApiResult.Error("用户不存在");

            // 2. 禁止修改管理员账号
            if (existUser.Username.Equals("admin", StringComparison.OrdinalIgnoreCase))
                return ApiResult.Error("管理员账号不可修改");

            // 3. 只更新角色，不覆盖其他字段
            existUser.Role = user.Role;

            // 4. 保存修改
            await _userService.UpdateUserAsync(existUser);
            return ApiResult.Success(msg: "修改成功");
        }

        // 删除用户
        [HttpDelete("{id}")]
        public async Task<ApiResult> Delete(int id)
        {
            var existUser = await _userService.GetUserByIdAsync(id);
            if (existUser == null)
                return ApiResult.Error("用户不存在");

            // 安全规则：管理员账号不可删除
            if (existUser.Username.Equals("admin", System.StringComparison.OrdinalIgnoreCase))
            {
                return ApiResult.Error("管理员账号不可删除");
            }

            await _userService.DeleteUserAsync(id);
            return ApiResult.Success(msg: "删除成功");
        }
    }
}