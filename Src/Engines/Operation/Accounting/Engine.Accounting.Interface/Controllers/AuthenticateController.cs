using System.Collections.Generic;
using System.Threading.Tasks;
using Engine.Accounting.Providers;
using Engine.Accounting.UserInfos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Engine.Accounting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase, IAuthenticateUser
    {
        private readonly IUsers _users;
        public AuthenticateController(IUsers users)
        {
            _users = users;
        }

        [HttpGet("{id}")]
        public async Task<UserInfoResponse> Get(string id)
        {
            return await _users.GetTokenUser(id);
        }



    }
}
