using ExampleService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Infrastructure.EntityConfigurations
{
    public static class BaseEntityConfiguration
    {
        public static void Configuration<T>(EntityTypeBuilder<T> builder) where T : class, IBaseEntity
        {
            builder.Property(x => x.CreatedDate);
            builder.Property(x => x.CreatedBy);
            builder.Property(x => x.UpdatedDate);
            builder.Property(x => x.UpdatedBy);
        }
    }
}
