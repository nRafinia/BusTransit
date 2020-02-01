using System.Threading.Tasks;
using Engine.Accounting.UserInfos;
using Refit;

namespace Engine.Accounting
{
    public interface IAuthenticateUser
    {
        [Get("/api/Authenticate/{userId}")]
        Task<UserInfoResponse> Get(string userId);
    }
}