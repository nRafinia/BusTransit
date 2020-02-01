using Common.Containers;
using Common.Models;

namespace Common.Authenticates
{
    public interface IAuthenticateUtil : ISingleton
    {
        string CreateToken(UserTokenModel data);
        UserTokenModel ValidateToken(string token);
    }
}