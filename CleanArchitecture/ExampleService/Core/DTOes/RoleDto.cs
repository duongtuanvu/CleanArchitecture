using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExampleService.Core.DTOes
{
    public class RoleDto
    {
        public RoleDto(string name, List<PermissionDto> permissions)
        {
            Name = name;
            Permissions = permissions;
        }
        public string Name { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }
}
