using Common.Authenticates;
using Common.Data;
using F4ST.Data;
using F4ST.Data.RavenDB;

namespace Engine.Accounting.Data
{
    public class User : DbEntity, IUserInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string LastName { get; set; }
        public UserStatus Status { get; set; }
    }
}