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
        public async Task<ApiResult> Update(int id, [FromBody] UpdateRoleDto dto)
        {
            // 👉 这里只会绑定Id和Role，完全不依赖Username/Password，绝对不会400！
            Console.WriteLine("【Debug-进入Update】路由ID：" + id + " 新角色：" + dto.Role);

            var existUser = await _userService.GetUserByIdAsync(id);
            if (existUser == null)
                return ApiResult.Error("用户不存在");

            if (existUser.Username.Equals("admin", StringComparison.OrdinalIgnoreCase))
                return ApiResult.Error("管理员账号不可修改");

            // 只改角色，其他字段完全不动
            existUser.Role = dto.Role;
            await _userService.UpdateUserAsync(existUser);

            Console.WriteLine("【Debug-更新成功】用户ID：" + id + " 新角色：" + existUser.Role);
            return ApiResult.Success("修改成功");
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


        // 临时测试用：写死ID切换角色，不用前端，排查底层更新能不能连库生效
        [HttpGet("TestUpdateRole")]
        public async Task<ApiResult> TestUpdateRole()
        {
            Console.WriteLine("===== 后端自测更新接口执行了 =====");
            // 这里写死你要测的用户ID，比如张三id=2
            int testUserId = 2;
            var oldUser = await _userService.GetUserByIdAsync(testUserId);
            if (oldUser == null)
            {
                Console.WriteLine("自测：用户不存在");
                return ApiResult.Error("用户不存在");
            }

            // 判断切换：是管理员就切普通，是普通就切管理员
            if (oldUser.Role == "admin")
            {
                oldUser.Role = "user";
            }
            else
            {
                oldUser.Role = "admin";
            }

            // 执行更新
            await _userService.UpdateUserAsync(oldUser);
            Console.WriteLine($"自测完成：原角色切换为--> {oldUser.Role}");
            return ApiResult.Success("自测更新成功，新角色：" + oldUser.Role);
        }


        // 个人设置：修改自己账号密码（核心安全接口）
        [HttpPut("MyProfile")]
        public async Task<ApiResult> MyProfile([FromBody] UpdateProfileDto dto)
        {
            Console.WriteLine($"【个人设置】用户 {dto.Id} 尝试修改资料");

            // 1. 查数据库获取真实用户（不从前端信任角色）
            var user = await _userService.GetUserByIdAsync(dto.Id);
            if (user == null)
                return ApiResult.Error("用户不存在");

            // 2. 安全规则：admin 账号不允许改名
            if (user.Username == "admin" && user.Username != dto.Username)
            {
                return ApiResult.Error("管理员账号不允许修改名称");
            }

            // 3. 禁止任何人把账号改成 admin
            if (dto.Username == "admin" && user.Username != "admin")
            {
                return ApiResult.Error("不允许设置为管理员账号");
            }

            // 4. 只修改允许的字段
            user.Username = dto.Username;

            // 5. 密码不为空才修改
            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.Password = dto.Password;
            }

            await _userService.UpdateUserAsync(user);
            return ApiResult.Success("保存成功");
        }
    }
}