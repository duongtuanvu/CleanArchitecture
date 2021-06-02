using ExampleService.Application.Commands.AccountCommand;
using ExampleService.DTOes;
using ExampleService.Extensions;
using ExampleService.Infrastructure;
using ExampleService.Infrastructure.Entities;
using ExampleService.Infrastructure.Interface.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Services
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
        private readonly ApplicationDbContext _context;
        public AccountService(ApplicationDbContext context,IUnitOfWork uow, IJwtToken jwtToken, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _context = context;
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
            var roleDtoes = new List<RoleDto>();
            foreach (var roleName in roleNamies)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var claims = await _roleManager.GetClaimsAsync(role);
                    var roleDto = new RoleDto(roleName, claims.Select(x => new PermissionDto(x.Type, x.Value)).ToList());
                    roleDtoes.Add(roleDto);
                }
            }var jsonClaims = JsonConvert.SerializeObject(roleDtoes);
            return _jwtToken.GenerateToken(user, jsonClaims); ;
        }
    }
}
