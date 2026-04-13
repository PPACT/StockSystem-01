namespace StockSystem.Models.Dto
{
    // 只存修改角色需要的字段，没有多余的Username/Password
    public class UpdateRoleDto
    {
        public int Id { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}