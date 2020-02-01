using System.Threading.Tasks;
using Common.Containers;
using Common.Receivers;
using Engine.Accounting.Providers;
using Engine.Accounting.UserInfos;

namespace Engine.Accounting.Controllers
{
    public class AccountingRpcReceiver : RPCReceiver, IAuthenticateUser
    {

        private readonly IUsers _users;
        public AccountingRpcReceiver(IUsers users)
        {
            _users = users;
        }

        public async Task<UserInfoResponse> Get(string id)
        {
            return await _users.Get(id);
        }
    }
}