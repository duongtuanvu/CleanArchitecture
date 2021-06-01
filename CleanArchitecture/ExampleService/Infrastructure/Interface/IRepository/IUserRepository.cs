using ExampleService.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ExampleService.Infrastructure.Interface.IRepository
{
    public interface IUserRepository
    {
        Task<User> GetBy(Expression<Func<User, bool>> predicate, bool asNoTracking = false);
    }
}
