using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Infrastructure.Entities
{
    public class BaseEntity : IBaseEntity
    {
        public Guid Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
