using Microsoft.AspNetCore.Mvc;
using StockSystem.Common;
using StockSystem.Services.IServices;

namespace StockSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        // 修复：支持表单提交，HTML 直接调用
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ApiResult> Login([FromForm] string username, [FromForm] string password)
        {
            var user = await _userService.LoginAsync(username, password);
            if (user == null)
                return ApiResult.Error("账号或密码错误");

            return ApiResult.Success(new
            {
                user.Id,
                user.Username
            }, "登录成功");
        }
    }
}