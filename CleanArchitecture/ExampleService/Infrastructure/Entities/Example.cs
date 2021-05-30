using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Infrastructure.Entities
{
    public class Example : BaseEntity
    {
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
