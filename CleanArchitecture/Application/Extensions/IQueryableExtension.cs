using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Extensions
{
    public static class IQueryableExtension
    {
        /// <summary>
        /// PagingAndSorting
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="searchRequest"></param>
        /// <returns></returns>
        public static async Task<ResponseExtension> PagingAndSorting<TEntity>(this IQueryable<TEntity> query, SearchExtension searchRequest = null) where TEntity : class
        {
            var totalRecords = await query.CountAsync();
            List<TEntity> data;
            if (searchRequest == null)
            {
                data = await query.AsNoTracking().ToListAsync();
                return new ResponseExtension(data: data, paging: null);
            }

            var sortField = GetFieldMapping(searchRequest, searchRequest.OrderBy);

            if (sortField != null)
            {
                if (searchRequest.OrderByDesc)
                {
                    query = query.OrderByDescending(sortField);
                }
                else
                {
                    query = query.OrderBy(sortField);
                }
            }
            query = query.SortDefaultField(searchRequest);

            if (searchRequest.PageNumber > 0)
            {
                query = query.Skip((searchRequest.PageNumber - 1) * searchRequest.PageSize).Take(searchRequest.PageSize);
            }
            data = await query.AsNoTracking().ToListAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / searchRequest.PageSize);
            var paging = new Paging(searchRequest.PageNumber, searchRequest.PageSize, totalPages, totalRecords);
            return new ResponseExtension(data: data, paging: paging);
        }

        private static IQueryable<TEntity> SortDefaultField<TEntity>(this IQueryable<TEntity> query, SearchExtension searchRequest = null) where TEntity : class
        {
            if (searchRequest == null)
            {
                return query;
            }

            if (searchRequest.GetDefaultSortField() == null || !searchRequest.GetDefaultSortField().Any())
            {
                return query;
            }
            foreach (var item in searchRequest.GetDefaultSortField())
            {
                if (!string.IsNullOrEmpty(searchRequest.OrderBy)
                    && item.Key.ToLower() == searchRequest.OrderBy.ToLower())
                {
                    continue;
                }

                var sortField = GetFieldMapping(searchRequest, item.Key);
                if (sortField == null) throw new KeyNotFoundException($"Không tìm thấy key {item.Key}");

                if (item.Value.ToLower().Equals("DESC".ToLower()))
                {
                    if (query.IsOrdered())
                    {
                        query = ((IOrderedQueryable<TEntity>)query).ThenByDescending(sortField);
                    }
                    else
                    {
                        query = query.OrderByDescending(sortField);
                    }
                }
                else
                {
                    if (query.IsOrdered())
                    {
                        query = (query as IOrderedQueryable<TEntity>).ThenBy(sortField);
                    }
                    else
                    {
                        query = query.OrderBy(sortField);
                    }
                }

            }

            return query;
        }

        private static string GetFieldMapping(SearchExtension searchRequest, string key)
        {
            if (searchRequest == null) return null;

            if (!string.IsNullOrEmpty(key)
               && searchRequest.GetFieldMapping().ContainsKey(key.ToLower()))
            {
                return searchRequest.GetFieldMapping()[key.ToLower()];
            }
            return null;
        }

        private static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, string propertyName, IComparer<object> comparer = null) where TEntity : class
        {
            return CallOrderedQueryable(query, "OrderBy", propertyName, comparer);
        }

        private static IOrderedQueryable<TEntity> OrderByDescending<TEntity>(this IQueryable<TEntity> query, string propertyName, IComparer<object> comparer = null) where TEntity : class
        {
            return CallOrderedQueryable(query, "OrderByDescending", propertyName, comparer);
        }

        private static IOrderedQueryable<TEntity> ThenBy<TEntity>(this IOrderedQueryable<TEntity> query, string propertyName, IComparer<object> comparer = null) where TEntity : class
        {
            return CallOrderedQueryable(query, "ThenBy", propertyName, comparer);
        }

        private static IOrderedQueryable<TEntity> ThenByDescending<TEntity>(this IOrderedQueryable<TEntity> query, string propertyName, IComparer<object> comparer = null) where TEntity : class
        {
            return CallOrderedQueryable(query, "ThenByDescending", propertyName, comparer);
        }

        /// <summary>
        /// Is Ordered
        /// </summary>
        /// <param name="queryable"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsOrdered<TEntity>(this IQueryable<TEntity> queryable) where TEntity : class
        {
            if (queryable == null)
            {
                throw new ArgumentNullException("queryable");
            }

            return queryable.Expression.Type == typeof(IOrderedQueryable<TEntity>);
        }

        /// <summary>
        /// Builds the Queryable functions using a TSource property name.
        /// </summary>
        public static IOrderedQueryable<TEntity> CallOrderedQueryable<TEntity>(this IQueryable<TEntity> query, string methodName, string propertyName,
                IComparer<object> comparer = null) where TEntity : class
        {
            var param = Expression.Parameter(typeof(TEntity), "x");

            var body = propertyName.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);

            return comparer != null
                ? (IOrderedQueryable<TEntity>)query.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new[] { typeof(TEntity), body.Type },
                        query.Expression,
                        Expression.Lambda(body, param),
                        Expression.Constant(comparer)
                    )
                )
                : (IOrderedQueryable<TEntity>)query.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new[] { typeof(TEntity), body.Type },
                        query.Expression,
                        Expression.Lambda(body, param)
                    )
                );
        }

        ///// <summary>
        ///// Lấy câu truy vấn SQL của Queryable
        ///// </summary>
        ///// <typeparam name="TEntity"></typeparam>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        //{
        //    using var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
        //    var relationalCommandCache = enumerator.Private("_relationalCommandCache");
        //    var selectExpression = relationalCommandCache.Private<SelectExpression>("_selectExpression");
        //    var factory = relationalCommandCache.Private<IQuerySqlGeneratorFactory>("_querySqlGeneratorFactory");

        //    var sqlGenerator = factory.Create();
        //    var command = sqlGenerator.GetCommand(selectExpression);

        //    string sql = command.CommandText;
        //    return sql;
        //}

        //private static object Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
        //private static T Private<T>(this object obj, string privateField) => (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
    }
}
