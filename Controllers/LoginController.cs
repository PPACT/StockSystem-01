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
        private readonly JwtHelper _jwtHelper;

        public LoginController(IUserService userService, JwtHelper jwtHelper)
        {
            _userService = userService;
            _jwtHelper = jwtHelper;
        }

        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ApiResult> Login([FromForm] string username, [FromForm] string password)
        {
            var user = await _userService.LoginAsync(username, password);
            if (user == null)
                return ApiResult.Error("账号或密码错误");

            // 生成 JWT Token
            var token = _jwtHelper.GenerateToken(user);

            return ApiResult.Success(new
            {
                id = user.Id,
                username = user.Username,
                role = user.Role,
                token
            }, "登录成功");
        }
    }
}