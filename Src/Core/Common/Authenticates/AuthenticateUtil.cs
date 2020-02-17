using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Common.Models;
using Common.Tools;
using F4ST.Common.Tools;
using Microsoft.IdentityModel.Tokens;

namespace Common.Authenticates
{
    public class AuthenticateUtil : IAuthenticateUtil
    {
        private readonly IAppSetting _appSetting;

        public AuthenticateUtil(IAppSetting appSetting)
        {
            _appSetting = appSetting;
        }

        /// <summary>
        /// ایجاد توکن جهت ارسال به کاربر
        /// </summary>
        /// <param name="data">اطلاعات توکن</param>
        /// <returns>توکن</returns>
        public string CreateToken(UserTokenModel data)
        {
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, data.TokenId),
                    new Claim(ClaimTypes.Name, data.UserId),
                    new Claim("IsTemp", data.IsTemp.ToString())
                }),
                EncryptingCredentials = GetEncryptCredential(),
                Expires = data.ExpireDate,
                SigningCredentials =
                    new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256Signature),
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return token;
        }

        /// <summary>
        /// بررسی صحبح بودن توکن و بازگرداندن اطلاعات آن
        /// </summary>
        /// <param name="token">توکن</param>
        /// <returns>اطلاعات توکن، در صورت صحیح نبودن مقدار null برمیگردد</returns>
        public UserTokenModel ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var tokenValidationParameters = GetTokenValidationParameters();

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters,
                    out var validatedToken);

                var exDate = validatedToken.ValidTo.ToLocalTime();
                if (exDate < DateTime.Now)
                    return null;

                return new UserTokenModel()
                {
                    TokenId = tokenValid.Claims.First(t => t.Type == ClaimTypes.NameIdentifier).Value,
                    UserId = tokenValid.Claims.First(t => t.Type == ClaimTypes.Name).Value,
                    IsTemp = Convert.ToBoolean(tokenValid.Claims.First(t => t.Type == "IsTemp").Value),
                    ExpireDate = exDate
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

#region private methods

        private SecurityKey GetSymmetricSecurityKey()
        {
            var symmetricKey = Convert.FromBase64String(_appSetting.Get("AuthSignKey"));
            return new SymmetricSecurityKey(symmetricKey);
        }

        private SymmetricSecurityKey GetEncryptKey()
        {
            var encKey = _appSetting.Get("AuthEncKey");
            if (string.IsNullOrWhiteSpace(encKey))
                return null;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSetting.Get("AuthEncKey")));
            return key;
        }

        private EncryptingCredentials GetEncryptCredential()
        {
            var key = GetEncryptKey();
            if (key == null)
                return null;

            var res = new EncryptingCredentials(key, JwtConstants.DirectKeyUseAlg,
                SecurityAlgorithms.Aes256CbcHmacSha512);

            return res;
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSymmetricSecurityKey(),
                TokenDecryptionKey = GetEncryptKey(),
                ClockSkew = TimeSpan.FromMinutes(0),
            };
        }

#endregion
    }
}