using ExampleService.Core.DTOs;
using ExampleService.Infrastructure.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExampleService.Core.Helpers
{
    public interface IJwtToken
    {
        string GenerateToken(string issuer, string audience, string secret, IEnumerable<Claim> claims);
    }
    public class JwtToken : IJwtToken
    {
        public string GenerateToken(string issuer, string audience, string secret, IEnumerable<Claim> claims, int expires, )
        {
            // generate token that is valid for 7 hours
            var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(_jwtSettings.Expires),
                    signingCredentials: new SigningCredentials(
                                            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                                            SecurityAlgorithms.HmacSha256Signature));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    public static class AuthenticationInstaller
    {
        public static void AddJwtAuthentication(this IServiceCollection services, string issuer, string audience, string secret)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer("Bearer", x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = string.IsNullOrWhiteSpace(issuer),
                    ValidIssuer = issuer ?? "",
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                    ValidAudience = audience ?? "",
                    ValidateAudience = string.IsNullOrWhiteSpace(audience),
                };
            });
        }
    }
}
