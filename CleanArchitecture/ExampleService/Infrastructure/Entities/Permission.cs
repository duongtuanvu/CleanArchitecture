using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Infrastructure.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Controller { get; set; }
        public string Value { get; set; }
    }
}
