namespace Engine.Accounting.Models.RoleActions
{
    public interface  IRoleAction
    {
        /// <summary>
        /// کد
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// کد نقش
        /// </summary>
        string RoleId{  get; set; }
        /// <summary>
        /// کد اکشن
        /// </summary>
        string ActionId { get; set; }
    }
}
