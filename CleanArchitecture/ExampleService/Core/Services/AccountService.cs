using Core.Extensions;
using ExampleService.Core.Application.Commands.AccountCommand;
using ExampleService.Core.DTOs;
using ExampleService.Core.Helpers;
using ExampleService.Infrastructure;
using ExampleService.Infrastructure.Entities;
using ExampleService.Infrastructure.Interface.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Core.Helpers.Enums;

namespace ExampleService.Core.Services
{
    public interface IAccountService
    {
        Task<string> Login(LoginCommand login);
    }

    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _uow;
        private readonly IJwtToken _jwtToken;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _configuration;
        public AccountService(IConfiguration configuration, IUnitOfWork uow, IJwtToken jwtToken, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _configuration = configuration;
            _uow = uow;
            _jwtToken = jwtToken;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<string> Login(LoginCommand login)
        {
            var user = await _uow.UserRepository.GetBy(x => x.UserName.ToLower().Equals(login.UserName.ToLower()));
            if (user == null)
            {
                throw new Exception("User not exists");
            }
            var isCorrectPassword = await _userManager.CheckPasswordAsync(user, login.Password);
            if (!isCorrectPassword)
            {
                throw new Exception("Incorrect password");
            }
            var roleNamies = await _userManager.GetRolesAsync(user);
            var roleDTOes = new List<RoleDto>();
            foreach (var roleName in roleNamies)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var claims = await _roleManager.GetClaimsAsync(role);
                    var roleDto = new RoleDto(roleName, claims.Select(x => new PermissionDto(x.Type, x.Value)).ToList());
                    roleDTOes.Add(roleDto);
                }
            }
            var jsonClaims = JsonConvert.SerializeObject(roleDTOes);

            var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();
            var jwtClaims = new List<Claim>() {
                new Claim(Constants.UserId, user.Id.ToString()),
                new Claim(Constants.UserName, user.UserName),
                new Claim(Constants.IsAdmin, user.IsAdmin.ToString()),
                new Claim(Constants.Permissions, jsonClaims)};
            return _jwtToken.GenerateToken(jwtSettings.Issuer, jwtSettings.Audience, jwtSettings.SecretKey, jwtClaims, 8, ExpirationType.Hour);
        }
    }
}
