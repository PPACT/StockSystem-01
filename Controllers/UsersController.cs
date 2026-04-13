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
            Console.WriteLine("=== 后端 Update 接口被调用 ===");
            Console.WriteLine("路由ID: " + id);
            Console.WriteLine("前端传过来的 Role: " + user.Role);

            try
            {
                var existUser = await _userService.GetUserByIdAsync(id);
                if (existUser == null)
                    return ApiResult.Error("用户不存在");

                if (existUser.Username.Equals("admin", StringComparison.OrdinalIgnoreCase))
                    return ApiResult.Error("管理员账号不可修改");

                // 🔥 这里正确写法：把前端传的角色赋值给数据库里的用户
                existUser.Role = user.Role;

                await _userService.UpdateUserAsync(existUser);
                return ApiResult.Success("修改成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine("错误: " + ex.Message);
                return ApiResult.Error("修改失败: " + ex.Message);
            }
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