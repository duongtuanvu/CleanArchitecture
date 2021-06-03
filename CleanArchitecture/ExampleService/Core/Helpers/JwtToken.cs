using ExampleService.Core.DTOes;
using ExampleService.Infrastructure.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExampleService.Core.Helpers
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public int Expires { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
    public interface IJwtToken
    {
        string GenerateToken(User user, string jsonClaims);
    }
    public class JwtToken : IJwtToken
    {
        private readonly JwtSettings _jwtSettings;
        public JwtToken(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(User user, string jsonClaims)
        {
            // generate token that is valid for 7 hours
            var claims = new[]
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim("UserName", user.UserName),
                new Claim("IsAdmin", user.IsAdmin.ToString()),
                new Claim("Permissions", jsonClaims)
            };
            var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(_jwtSettings.Expires),
                    signingCredentials: new SigningCredentials(
                                            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SecretKey)),
                                            SecurityAlgorithms.HmacSha256Signature));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
