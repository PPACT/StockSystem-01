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

        [HttpGet]
        public async Task<ApiResult> GetAll()
        {
            var list = await _userService.GetAllUsersAsync();
            return ApiResult.Success(list);
        }

        [HttpGet("{id}")]
        public async Task<ApiResult> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return ApiResult.Error("用户不存在");
            return ApiResult.Success(user);
        }

        [HttpPost]
        public async Task<ApiResult> Add(User user)
        {
            await _userService.AddUserAsync(user);
            return ApiResult.Success(msg: "添加成功");
        }

        [HttpPut]
        public async Task<ApiResult> Update(User user)
        {
            await _userService.UpdateUserAsync(user);
            return ApiResult.Success(msg: "修改成功");
        }

        [HttpDelete("{id}")]
        public async Task<ApiResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            return ApiResult.Success(msg: "删除成功");
        }
    }
}