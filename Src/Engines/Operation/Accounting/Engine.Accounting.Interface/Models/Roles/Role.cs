namespace Engine.Accounting.Models.Roles
{
    public class Role:IRole
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }
}
