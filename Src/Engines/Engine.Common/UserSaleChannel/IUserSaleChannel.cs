namespace Engine.Common.UserSaleChannel
{
    public interface  IUserSaleChannel
    {
        /// <summary>
        /// کد
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// کد کاربر
        /// </summary>
        string UserId{  get; set; }
        /// <summary>
        /// کد کانال فروش
        /// </summary>
        string SaleChannelId { get; set; }
    }
}
