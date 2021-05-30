using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Infrastructure.Entities
{
    public class User : IdentityUser<Guid>, IBaseEntity
    {
        public DateTime? CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<UserClaim> UserClaims { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
