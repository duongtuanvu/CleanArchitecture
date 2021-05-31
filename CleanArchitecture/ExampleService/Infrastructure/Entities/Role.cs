using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ExampleService.Infrastructure.Entities
{
    public class Role : IdentityRole<Guid>, IBaseEntity
    {
        public DateTime? CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}