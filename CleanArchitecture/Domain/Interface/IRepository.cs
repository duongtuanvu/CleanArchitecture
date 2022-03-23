using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        Task AddAsync(T entity);
        void AddRange(IEnumerable<T> entities);
        Task AddRangeAsync(IEnumerable<T> entities);
        IQueryable<T> GetAll();
        IQueryable<T> GetBy(Expression<Func<T, bool>> expression, bool asNoTracking = false);
        T Find(object key);
        Task<T> FindAsync(object key);
        void Update(T entitie);
        void UpdateRange(List<T> entities);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        IQueryable<T> Include(params Expression<Func<T, object>>[] includes);

        //Task<T> Find(Expression<Func<T, bool>> match);
        //IEnumerable<T> FindAll(Expression<Func<T, bool>> match, bool asNoTracking = true);
        //Task<IQueryable<T>> QueryAsync<T>(string sql, object param = null, CommandType commandType = CommandType.Text);
        //Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, CommandType commandType = CommandType.Text);
        //Task<T> QuerySingleAsync<T>(string sql, object param = null, CommandType commandType = CommandType.Text);
    }
}
