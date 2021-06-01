using Application.Extensions;
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
    public class Repository<T> : IRepository<T> where T : BaseEntity, IBaseEntity
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> table = null;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            table = _context.Set<T>();
        }

        public async Task<ResponseExtension> ListASync(Expression<Func<T, bool>> predicate, SearchBase search, bool asNoTracking = false)
        {
            var query = table.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search.Keyword))
            {
                query = query.Where(predicate);
            }
            return await query.Sort<T>(search);
        }

        public async Task<ResponseExtension> GetByAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false)
        {
            var query = table.Where(predicate);
            if (asNoTracking)
            {
                return new ResponseExtension(data: await query.AsNoTracking().FirstOrDefaultAsync());
            }
            return new ResponseExtension(data: await query.FirstOrDefaultAsync());
        }

        public async Task AddAsync(T entity)
        {
            await table.AddAsync(entity);
        }

        public async Task AddRangeAsync(List<T> entities)
        {
            await table.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            table.Update(entity);
        }

        public void UpdateRange(List<T> entities)
        {
            table.UpdateRange(entities);
        }

        public void Delete(T entity)
        {
            table.Remove(entity);
        }

        public void DeleteRange(List<T> entities)
        {
            table.RemoveRange(entities);
        }
    }
}
