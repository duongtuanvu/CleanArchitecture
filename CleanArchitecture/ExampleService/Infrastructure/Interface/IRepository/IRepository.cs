using Application.Extensions;
using ExampleService.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ExampleService.Infrastructure.Interface.IRepository
{
    public interface IRepository<T> where T : BaseEntity, IBaseEntity
    {
        Task<ResponseExtension> ListASync(Expression<Func<T, bool>> predicate, SearchBase search, bool asNoTracking = false);
        Task<ResponseExtension> GetByAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false);
        Task AddAsync(T entity);
        Task AddRangeAsync(List<T> entities);
        void Update(T entity);
        void UpdateRange(List<T> entities);
        void Delete(T entity);
        void DeleteRange(List<T> entities);
    }
}
