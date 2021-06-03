﻿using Application.Extensions;
using ExampleService.Core.Helpers;
using ExampleService.Core.DTOes;
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
                    if (!user.IsAdmin)
                    {
                        return;
                    }
                    else
                    {
                        var permissions = JsonConvert.DeserializeObject<List<RoleDto>>(jsonToken.Claims.Where(x => x.Type.Equals(Constant.Permissions)).FirstOrDefault()?.Value);
                        if (permissions.Any(x => Roles.Contains(x.Name) && x.Permissions.Any(p => Permissions.Any(p1 => p1.Contains(p.Type)))))
                        {
                            return;
                        }
                        context.Result = new ForbidResult();
                        return;
                    }
                }
            }
        }
    }
}