using Common.Models;
using F4ST.Common.Containers;

namespace Common.Authenticates
{
    public interface IAuthenticateUtil : ISingleton
    {
        string CreateToken(UserTokenModel data);
        UserTokenModel ValidateToken(string token);
    }
}