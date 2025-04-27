using Hgzn.Mes.Domain.Utilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Hgzn.Mes.Application.Main.Utilities
{
    public static class JwtTokenUtil
    {
        //public static string GenerateJwtToken(JwtPayload pairs)
        //{
        //    var credentials = new SigningCredentials(new ECDsaSecurityKey(CryptoUtil.PrivateECDsa), SecurityAlgorithms.EcdsaSha256);
        //    var token = new JwtSecurityToken(new JwtHeader(credentials), pairs);
        //    return token.ToString();
        //}

        public static string GenerateJwtToken(string issuer, string audience, TimeSpan expires, params Claim[] claims)
        {
            var handler = new JwtSecurityTokenHandler();
            var credentials = new SigningCredentials(CryptoUtil.PrivateECDsaSecurityKey, SecurityAlgorithms.EcdsaSha256);
            // 生成令牌时，使用带有 KeyId 的 SecurityKey
            // var credentials = new SigningCredentials(new ECDsaSecurityKey(CryptoUtil.PrivateECDsaSecurityKey.ECDsa){ KeyId ="EC_KEY_2024" }, SecurityAlgorithms.EcdsaSha256);
            var nbf = DateTime.UtcNow;
            var expire = nbf + expires;
            // var token = new JwtSecurityToken(issuer, audience,
            //     claims, nbf, expire, credentials);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = new ClaimsIdentity(claims),
                Expires = expire,
                SigningCredentials = credentials
            };
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
    }
}