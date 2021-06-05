using Application.Extensions;
using ExampleService.Infrastructure;
using ExampleService.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Core.Application.Queries.AccountQuery
{
    public class AccountQuery
    {
        private readonly ApplicationDbContext _context;
        public AccountQuery(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ResponseExtension> List(AccountSearch search)
        {
            var query = _context.Users.AsQueryable();
            return await query.Sort<User>(search);
        }
    }
}
