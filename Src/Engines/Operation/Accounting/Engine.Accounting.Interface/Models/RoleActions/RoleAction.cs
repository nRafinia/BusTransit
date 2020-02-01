namespace Engine.Accounting.Models.RoleActions
{
    public class RoleAction:IRoleAction
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string ActionId { get; set; }
    }
}
