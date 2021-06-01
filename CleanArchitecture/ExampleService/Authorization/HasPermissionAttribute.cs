using Application.Extensions;
using ExampleService.Infrastructure;
using ExampleService.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Authorization
{
    public class HasPermissionAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly string Role;
        private readonly List<string> Claims;
        public HasPermissionAttribute(string role, string[] claims = null)
        {
            Role = role;
            if (claims != null)
            {
                Claims = claims.ToList();
            }
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var dbContext = context.HttpContext.RequestServices.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            var userManager = context.HttpContext.RequestServices.GetService(typeof(UserManager<User>)) as UserManager<User>;
            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var userId = jsonToken.Claims.Where(x => x.Type.Equals("UserId")).FirstOrDefault()?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                context.Result = new ForbidResult();
                return;
            }
            else
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    context.Result = new ForbidResult();
                    return;
                }
                else
                {
                    if (user.IsAdmin)
                    {
                        return;
                    }
                    else
                    {

                    }
                }
            }
        }
    }
}
