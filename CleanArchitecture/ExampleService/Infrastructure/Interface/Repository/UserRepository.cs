using ExampleService.Infrastructure.Entities;
using ExampleService.Infrastructure.Interface.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ExampleService.Infrastructure.Interface.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetBy(Expression<Func<User, bool>> predicate, bool asNoTracking = false)
        {
            var query = _context.Users.Where(predicate);
            if (asNoTracking)
            {
                return await query.AsNoTracking().FirstOrDefaultAsync();
            }
            return await query.FirstOrDefaultAsync();
        }
    }
}
