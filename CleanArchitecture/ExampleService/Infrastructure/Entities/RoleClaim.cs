using Microsoft.AspNetCore.Identity;
using System;

namespace ExampleService.Infrastructure.Entities
{
    public class RoleClaim : IdentityRoleClaim<Guid>, IBaseEntity
    {
        public DateTime? CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        //public Role Role { get; set; }
    }
}