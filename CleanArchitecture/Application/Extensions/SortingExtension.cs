using Application.Extensions;
using Application.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Application.Extensions
{
    public static class SortingExtension
    {
        public static ResponseExtension Sort<T>(this IQueryable<T> query, SearchExtension search) where T : class
        {
            if (!string.IsNullOrWhiteSpace(search.OrderBy))
            {
                var lstProperties = typeof(T).GetProperties();
                foreach (var prop in lstProperties)
                {
                    if (prop.Name.ToLower().Equals(search.OrderBy.ToLower()))
                    {
                        if (search.OrderByDesc)
                        {
                            query = query.OrderByDescending(prop.Name);
                        }
                        else
                        {
                            query = query.OrderBy(prop.Name);
                        }
                        break;
                    }
                }
            }
            var totalRecords = query.Count();
            var totalPages = Convert.ToInt32(Math.Ceiling((double)totalRecords / (double)search.PageSize));
            var paging = new Paging(search.PageNumber, search.PageSize, totalPages, totalRecords);
            query = query.Skip((search.PageNumber - 1) * search.PageSize).Take(search.PageSize);
            return new ResponseExtension(data: query, paging: paging);
        }

        private static IOrderedQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> query, string propertyName, string keyword = null) where TEntity : class
        {
            return CallOrderedQueryable(query, "Where", propertyName, keyword, new List<string> { "Name" });
        }

        private static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, string propertyName) where TEntity : class
        {
            return CallOrderedQueryable(query, "OrderBy", propertyName);
        }

        private static IOrderedQueryable<TEntity> OrderByDescending<TEntity>(this IQueryable<TEntity> query, string propertyName) where TEntity : class
        {
            return CallOrderedQueryable(query, "OrderByDescending", propertyName);
        }
        public static IOrderedQueryable<TEntity> CallOrderedQueryable<TEntity>(this IQueryable<TEntity> query, string methodName, string propertyName, string keyword = null, List<string> propertyNameToSearch = null) where TEntity : class
        {
            var param = Expression.Parameter(typeof(TEntity), "x");
            MemberExpression body;
            if (keyword != null && propertyNameToSearch != null)
            {
                foreach (var prop in propertyNameToSearch)
                {
                    var nameProperty = Expression.PropertyOrField(param, prop);
                    var val1 = Expression.Constant(keyword);
                }
                body = Expression.PropertyOrField(param, propertyName); // propertyName.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);
            }
            else
            {
                body = Expression.PropertyOrField(param, propertyName); // propertyName.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);
            }
            return (IOrderedQueryable<TEntity>)query.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new[] { typeof(TEntity), body.Type },
                        query.Expression,
                        Expression.Lambda(body, param)
                    )
                );
        }

        private static Expression<Func<T, int>> CreateExpression<T>(string prop)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var idProp = Expression.PropertyOrField(param, prop);
            var lamda = Expression.Lambda<Func<T, int>>(idProp, param);
            return lamda;
        }

        //private static object GetReflectedPropertyValue(this object subject, string field)
        //{
        //    return subject.GetType().GetProperty(field).GetValue(subject, null);
        //}

        //public static Expression<Func<TSource, object>> GetExpression<TSource>(string propertyName)
        //{
        //    var param = Expression.Parameter(typeof(TSource), "x");
        //    Expression conversion = Expression.Convert(Expression.Property
        //    (param, propertyName), typeof(object));   //important to use the Expression.Convert
        //    return Expression.Lambda<Func<TSource, object>>(conversion, param);
        //}

        ////makes deleget for specific prop
        //public static Func<TSource, object> GetFunc<TSource>(string propertyName)
        //{
        //    return GetExpression<TSource>(propertyName).Compile();  //only need compiled expression
        //}

        //public static IOrderedEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> source, string propertyName)
        //{
        //    return source.OrderBy(GetFunc<TSource>(propertyName));
        //}

        //public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string propertyName)
        //{
        //    return source.OrderBy(GetExpression<TSource>(propertyName));
        //}

        //public static IOrderedEnumerable<TSource> OrderByDescending<TSource>(this IEnumerable<TSource> source, string propertyName)
        //{
        //    return source.OrderByDescending(GetFunc<TSource>(propertyName));
        //}

        ////OrderBy overload
        //public static IOrderedQueryable<TSource> OrderByDescending<TSource>(this IQueryable<TSource> source, string propertyName)
        //{
        //    return source.OrderByDescending(GetExpression<TSource>(propertyName));
        //}
    }
}
