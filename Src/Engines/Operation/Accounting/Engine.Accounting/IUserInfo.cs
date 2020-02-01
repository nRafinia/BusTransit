using Common.Authenticates;

namespace Engine.Accounting
{
    public interface IUserInfo : IBaseUserInfo
    {
        /// <summary>
        /// کلمه عبور
        /// </summary>
        string Password { get; set; }
    }
}