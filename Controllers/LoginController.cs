using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StockSystem.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using StockSystem.Data;

namespace StockSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _db;

        public LoginController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            var u = await _db.Users
                .FirstOrDefaultAsync(x => x.Username == user.Username
                                        && x.Password == user.Password);

            if (u == null)
                return Unauthorized("账号或密码错误");

            // 生成 JWT Token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, u.Username)
            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySecretKey1234567890123456"));
            var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenStr });
        }
    }
}