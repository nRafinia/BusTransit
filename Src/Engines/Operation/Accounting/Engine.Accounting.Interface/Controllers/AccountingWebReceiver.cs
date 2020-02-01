using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Common.Attributes;
using Common.Extensions;
using Common.Models;
using Common.QMessageModels.RequestMessages;
using Common.Receivers;
using Engine.Accounting.Login;
using Engine.Accounting.Providers;
using Engine.Accounting.Register;
using Engine.Accounting.UserInfos;

namespace Engine.Accounting.Controllers
{
    public class AccountingWebReceiver : WebServiceReceiver
    {
        private readonly IUsers _users;

        public AccountingWebReceiver(IUsers users)
        {
            _users = users;
        }

        [QPost]
        public async Task<LoginResponse> Login([QFromBody] LoginRequest request)
        {
            var res = await _users.Login(request);
            return res;
        }

        public async Task<RegisterUserResponse> CreateTestUser()
        {
            return await _users.CreateTestUser();
        }

        [QRoute("id")]
        [QAuth]
        public async Task<UserInfoResponse> Get(string id)
        {
            return await _users.Get(id);
        }
    }
}