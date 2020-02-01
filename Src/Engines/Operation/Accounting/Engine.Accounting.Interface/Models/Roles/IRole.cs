namespace Engine.Accounting.Models.Roles
{
    public interface  IRole
    {
        /// <summary>
        /// کد نقش
        /// </summary>
         string RoleId { get; set; }
        /// <summary>
        /// نام نقش
        /// </summary>
         string RoleName { get; set; }
        /// <summary>
        /// فعال/غیرفعال
        /// </summary>
         bool IsActive { get; set; }
    }
}
