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
        private readonly IDbConnection connection;
        private readonly IConfiguration _configuration;
        private DbSet<T> table = null;
        public Repository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            table = _context.Set<T>();
            _configuration = configuration;
        }

        public async Task Add(T entity)
        {
            await table.AddAsync(entity);
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

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                return (await connection.QueryAsync<T>(sql, param, commandType: commandType));
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<T>(sql, param, commandType: commandType);
            }
        }

        public async Task<T> QuerySingleAsync<T>(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                return await connection.QuerySingleAsync<T>(sql, param, commandType: commandType);
            }
        }
    }
}
