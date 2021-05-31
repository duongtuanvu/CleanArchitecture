using ExampleService.Infrastructure;
using ExampleService.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Behaviour
{
    public class AuthorizeBehaviour : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string Role;
        private readonly List<string> Claims;
        private readonly UserManager<User> _userManager;
        public AuthorizeBehaviour(string role, string[] claims)
        {
            Role = role;
            Claims = claims.ToList();
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var dbContext = context.HttpContext.RequestServices.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            var userManager = context.HttpContext.RequestServices.GetService(typeof(UserManager<User>)) as UserManager<User>;
            var token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = (JwtSecurityToken)handler.ReadToken(token);
            //var tokenS = jsonToken as JwtSecurityToken;
            var userId = jsonToken.Claims.First(x => x.Type.Equals("UserId"))?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                context.Result = "";
                return;
            }
            if (string.IsNullOrEmpty()
            {

            }
        }
    }
}
