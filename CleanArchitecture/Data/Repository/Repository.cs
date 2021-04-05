using Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository 
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> table = null;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            table = _context.Set<T>();
        }
        public Task<T> Find(Expression<Func<T, bool>> match)
        {
            return table.Where(match).FirstOrDefaultAsync();
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> match, bool asNoTracking = true)
        {
            if (asNoTracking)
            {
                return table.AsNoTracking();
            }
            else
            {
                return table;
            }
        }
    }
}
