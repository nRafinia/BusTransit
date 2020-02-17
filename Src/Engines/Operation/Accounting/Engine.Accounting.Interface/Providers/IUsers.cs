using System.Threading.Tasks;
using Common.Containers;
using Common.Models;
using Engine.Accounting.Login;
using Engine.Accounting.Register;
using Engine.Accounting.UserInfos;
using F4ST.Common.Containers;

namespace Engine.Accounting.Providers
{
    public interface IUsers : ISingleton
    {
        Task<RegisterUserResponse> CreateTestUser();
        Task<UserInfoResponse> Get(string id);
        Task<LoginResponse> Login(LoginRequest request);
        Task<UserInfoResponse> GetTokenUser(string id);
    }
}