
using Common.Authenticates;
using Common.Models;

namespace Engine.Accounting.UserInfos
{
    public class UserInfoResponse : BaseResponse
    {
        public BaseUserInfo UserInfo { get; set; }
    }
}