using Common.Authenticates;

namespace Engine.Accounting
{
    public class UserInfo : BaseUserInfo, IUserInfo
    {
        /// <summary>
        /// کلمه عبور
        /// </summary>
        public string Password { get; set; }
    }
}