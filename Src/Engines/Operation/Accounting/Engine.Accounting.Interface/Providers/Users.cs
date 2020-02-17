using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Attributes;
using Common.Authenticates;
using Common.CacheMemory;
using Common.Containers;
using Common.Extensions;
using Common.Models;
using Engine.Accounting.Data;
using Engine.Accounting.Login;
using Engine.Accounting.Register;
using Engine.Accounting.Resources;
using Engine.Accounting.UserInfos;
using F4ST.Common.Containers;
using F4ST.Common.Mappers;
using F4ST.Data;

namespace Engine.Accounting.Providers
{
    public class Users : IUsers
    {
        private readonly ICacheMemory _cacheMemory;
        private readonly IAuthenticateUtil _authenticateUtil;

        public Users(ICacheMemory cacheMemory, IAuthenticateUtil authenticateUtil)
        {
            _cacheMemory = cacheMemory;
            _authenticateUtil = authenticateUtil;
        }

        public async Task<RegisterUserResponse> CreateTestUser()
        {
            var res = await CreateUser(new RegisterUserRequest()
            {
                UserName = "test",
                Password = "test",
                FirstName = "Test",
                LastName = "User",
            }, UserStatus.Active);

            BaseUserInfo user = null;
            if (res.Item1 == 0)
            {
                user = res.Item2.MapTo<BaseUserInfo>();
                await _cacheMemory.Set(user.Id.ToString(), user);
            }

            return new RegisterUserResponse()
            {
                Status = res.Item1,
                UserInfo = user
            };
        }

        private async Task<(int, IUserInfo)> CreateUser(RegisterUserRequest data, UserStatus status)
        {
            data.UserName = data.UserName.ToLower();

            using (var db = IoC.Resolve<IRepository>())
            {
                var users = await db.Find<User>(f => f.UserName == data.UserName);
                if (users.Any())
                {
                    return (-1, null);
                }

                var u = new User()
                {
                    Id = string.Empty,
                    UserName = data.UserName.ToLower(),
                    Password = data.Password.ToMd5(),
                    Firstname = data.FirstName,
                    LastName = data.LastName,
                    Email = data.EMail,
                    Status = status,
                };
                await db.Add(u);
                await db.SaveChanges();

                return (0, u.MapTo<UserInfo>());
            }
        }

        public async Task<UserInfoResponse> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new UserInfoResponse()
                {
                    Status = -1
                };
            }

            var res = await _cacheMemory.Get<BaseUserInfo>(id, Provider.Globals);
            if (res != null)
            {
                return new UserInfoResponse()
                {
                    Status = 0,
                    UserInfo = res
                };
            }

            using (var db = IoC.Resolve<IRepository>())
            {
                var r = await db.Get<User>(id);
                if (r == null)
                {
                    return new UserInfoResponse()
                    {
                        Status = -2
                    };
                }

                var user = r.MapTo<BaseUserInfo>();
                await _cacheMemory.Set(id, user, provider: Provider.Globals);

                return new UserInfoResponse()
                {
                    Status = 0,
                    UserInfo = user
                };
            }
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            if (!request.IsValid(out var results))
            {
                return new LoginResponse()
                {
                    Status = (int) GlobalStatusCode.ParameterInvalid,
                    Message = GlobalStatusCode.ParameterInvalid.GetEnumName(),
                    ValidationResult = results
                };
            }

            var userName = request.UserName.ToLower();
            var pass = request.Password.ToMd5();
            using (var db = IoC.Resolve<IRepository>())
            {
                var users = (await db.Find<User>(
                        u => u.Status == UserStatus.Active && u.UserName == userName && u.Password == pass))
                    .ToArray();

                if (!users.Any())
                {
                    return new LoginResponse()
                    {
                        Status = -1,
                        Message = UserRes.UserNotFound
                    };
                }

                var user = users.FirstOrDefault(); //?.MapTo<UserInfo>();
                if (user == null)
                {
                    return new LoginResponse()
                    {
                        Status = -1,
                        Message = UserRes.UserNotFound
                    };
                }

                var cachedUser = await _cacheMemory.Get<BaseUserInfo>(user.Id.ToString(), Provider.Globals);
                if (cachedUser == null)
                {
                    await _cacheMemory.Set(user.Id.ToString(), user.MapTo<BaseUserInfo>(), provider: Provider.Globals);
                }

                var tokenE = new UserToken()
                {
                    Id = string.Empty,
                    UserId = user.Id.ToString(),
                    ExpireDate = DateTime.Now.AddMonths(6),
                    Expired = false
                };

                await db.Add(tokenE);

                await db.SaveChanges();

                var token = _authenticateUtil.CreateToken(new UserTokenModel()
                {
                    TokenId = tokenE.Id.ToString(),
                    UserId = tokenE.UserId,
                    ExpireDate = tokenE.ExpireDate,
                    IsTemp = !request.RememberMe
                });

                await _cacheMemory.Set(tokenE.Id.ToString(), user.MapTo<BaseUserInfo>(), provider: Provider.Globals);

                return new LoginResponse()
                {
                    Status = (int) GlobalStatusCode.Ok,
                    Token = token
                };
            }
        }

        public async Task<UserInfoResponse> GetTokenUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new UserInfoResponse()
                {
                    Status = -1
                };
            }

            var res = await _cacheMemory.Get<BaseUserInfo>(id, Provider.Globals);
            if (res != null)
            {
                return new UserInfoResponse()
                {
                    Status = 0,
                    UserInfo = res
                };
            }

            using var db = IoC.Resolve<IRepository>();
            var r = await db.Get<UserToken, User>(id, t => t.UserId, u => u.User);
            if (r == null)
            {
                return new UserInfoResponse()
                {
                    Status = -2
                };
            }

            var user = r.User.MapTo<BaseUserInfo>();
            await _cacheMemory.Set(id, user, provider: Provider.Globals);

            return new UserInfoResponse()
            {
                Status = 0,
                UserInfo = user
            };
        }
    }
}