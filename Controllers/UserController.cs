using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockSystem.Data;
using StockSystem.Models;

namespace StockSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UserController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var users = await _db.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Add(User user)
        {
            user.IsAdmin = false;
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Ok("新增成功");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User model)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return NotFound();

            u.Password = model.Password;
            await _db.SaveChangesAsync();
            return Ok("修改成功");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return NotFound();

            _db.Users.Remove(u);
            await _db.SaveChangesAsync();
            return Ok("删除成功");
        }
    }
}