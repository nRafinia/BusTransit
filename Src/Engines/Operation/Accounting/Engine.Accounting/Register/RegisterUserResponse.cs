using Common.Authenticates;
using Common.Models;

namespace Engine.Accounting.Register
{
    public class RegisterUserResponse : BaseResponse
    {
        public IBaseUserInfo UserInfo { get; set; }
    }
}