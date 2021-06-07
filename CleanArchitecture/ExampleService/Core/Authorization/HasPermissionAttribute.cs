using Core.Extensions;
using ExampleService.Core.Helpers;
using ExampleService.Core.DTOs;
using ExampleService.Infrastructure;
using ExampleService.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ExampleService.Core.Authorization
{
    public class HasPermissionAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly List<string> Roles;
        private readonly List<string> Permissions;
        public HasPermissionAttribute(string[] roles, string[] permissions = null)
        {
            if (permissions != null)
            {
                Roles = roles.ToList();
            }
            if (permissions != null)
            {
                Permissions = permissions.ToList();
            }
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //var dbContext = context.HttpContext.RequestServices.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            //var userManager = context.HttpContext.RequestServices.GetService(typeof(UserManager<User>)) as UserManager<User>;
            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var jsonToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            var isAdmin = jsonToken.Claims.Where(x => x.Type.Equals(Constants.IsAdmin)).FirstOrDefault()?.Value;
            if (!bool.Parse(isAdmin))
            {
                var permissions = JsonConvert.DeserializeObject<List<RoleDto>>(jsonToken.Claims.Where(x => x.Type.Equals(Constants.Permissions)).FirstOrDefault()?.Value);
                if (permissions.Any(x => Roles.Contains(x.Name) && x.Permissions.Any(p => Permissions.Any(p1 => p1.Contains(p.Type)))))
                {
                    return;
                }
                context.Result = new ForbidResult();
                return;
            }
            return;
        }
    }
}
