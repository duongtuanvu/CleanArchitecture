using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Core.Helpers.Enums;

namespace Core.Extensions
{
    public interface IJwtToken
    {
        /// <summary>
        /// Generate Jwt token
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="audience"></param>
        /// <param name="secret"></param>
        /// <param name="claims"></param>
        /// <param name="expires"></param>
        /// <param name="expirationType"></param>
        /// <returns></returns>
        string GenerateToken(string issuer, string audience, string secret, IEnumerable<Claim> claims, int expires, ExpirationType expirationType);
    }
    public class JwtToken : IJwtToken
    {
        public string GenerateToken(string issuer, string audience, string secret, IEnumerable<Claim> claims, int expires, ExpirationType expirationType)
        {
            DateTime expire = DateTime.UtcNow;
            switch (expirationType)
            {
                case ExpirationType.Second:
                    expire = expire.AddSeconds(expires);
                    break;
                case ExpirationType.Minute:
                    expire = expire.AddMinutes(expires);
                    break;
                case ExpirationType.Hour:
                    expire = expire.AddHours(expires);
                    break;
                case ExpirationType.Day:
                    expire = expire.AddDays(expires);
                    break;
                case ExpirationType.Month:
                    expire = expire.AddMonths(expires);
                    break;
                case ExpirationType.Year:
                    expire = expire.AddYears(expires);
                    break;
                default:
                    break;
            }
            var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: expire,
                    signingCredentials: new SigningCredentials(
                                            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                                            SecurityAlgorithms.HmacSha256Signature));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
