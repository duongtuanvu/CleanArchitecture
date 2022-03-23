using Dapper;
using Data.Context;
using Domain.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }
        public void AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
        }
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }
        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }
        public IQueryable<T> GetBy(Expression<Func<T, bool>> expression, bool asNoTracking = false)
        {
            var query = _context.Set<T>().Where(expression);
            if (asNoTracking)
            {
                return query.AsNoTracking();
            }
            return query;
        }
        public T Find(object key)
        {
            return _context.Set<T>().Find(key);
        }
        public async Task<T> FindAsync(object key)
        {
            return await _context.Set<T>().FindAsync(key);
        }
        public void Update(T entitie)
        {
            _context.Set<T>().Update(entitie);
        }
        public void UpdateRange(List<T> entities)
        {
            _context.Set<T>().UpdateRange(entities);
        }
        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
        public void DeleteRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }
        public IQueryable<T> Include(params Expression<Func<T, object>>[] includes)
        {
            var dbSet = _context.Set<T>();
            IQueryable<T> query = null;
            foreach (var include in includes)
            {
                query = dbSet.Include(include);
            }
            return query ?? dbSet;
        }

        //public async Task Add(T entity)
        //{
        //    await table.AddAsync(entity);
        //}

        //public Task<T> Find(Expression<Func<T, bool>> match)
        //{
        //    return table.Where(match).FirstOrDefaultAsync();
        //}

        //public IEnumerable<T> FindAll(Expression<Func<T, bool>> match, bool asNoTracking = true)
        //{
        //    if (asNoTracking)
        //    {
        //        return table.AsNoTracking();
        //    }
        //    else
        //    {
        //        return table;
        //    }
        //}

        //public async Task<IQueryable<T>> QueryAsync<T>(string sql, object param = null, CommandType commandType = CommandType.Text)
        //{
        //    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        connection.Open();
        //        return (await connection.QueryAsync<T>(sql, param, commandType: commandType)).AsQueryable();
        //    }
        //}

        //public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, CommandType commandType = CommandType.Text)
        //{
        //    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        connection.Open();
        //        return await connection.QueryFirstOrDefaultAsync<T>(sql, param, commandType: commandType);
        //    }
        //}

        //public async Task<T> QuerySingleAsync<T>(string sql, object param = null, CommandType commandType = CommandType.Text)
        //{
        //    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        connection.Open();
        //        return await connection.QuerySingleAsync<T>(sql, param, commandType: commandType);
        //    }
        //}
    }
}
