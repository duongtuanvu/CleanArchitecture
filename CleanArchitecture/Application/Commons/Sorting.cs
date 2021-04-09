using Application.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Application.Common
{
    public static class Sorting
    {
        public static List<T> Sort<T>(string orderBy, bool orderByDesc, IQueryable<T> data)
        {
            data.OrderByDynamic("Id DESC");
            //data.OrderByDescending(GetPropertyGetter<T>("Id")).ToList();
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                var lstProperties = typeof(T).GetProperties();
                foreach (var prop in lstProperties)
                {
                    if (prop.Name.ToLower().Equals(orderBy.ToLower()))
                    {
                        if (orderByDesc)
                        {
                            //data.OrderByDescending(CreateExpression<T>(orderBy)).ToList();
                            //data.OrderByPropertyDescending<T>(prop.Name).ToList();
                            data.OrderByDescending(CreateExpression<T>(prop.Name));
                            var sql = data.ToString();
                        }
                        else
                        {
                            data.OrderBy(x => x.GetReflectedPropertyValue(orderBy)).ToList();
                        }
                    }
                }
            }
            return data.ToList();
        }

        //public static Expression<Func<TEntity, object>> GetPropertyGetter<TEntity>(string property)
        //{
        //    if (property == null)
        //        throw new ArgumentNullException(nameof(property));

        //    var param = Expression.Parameter(typeof(TEntity));
        //    var prop = Expression.PropertyOrField(param, "Id");
        //    var convertedProp = Expression.Convert(prop, typeof(object));
        //    return Expression.Lambda<Func<TEntity, object>>(convertedProp, param);
        //}

        private static readonly MethodInfo OrderByDescendingMethod = typeof(Queryable).GetMethods().Single(method => method.Name == "OrderByDescending" && method.GetParameters().Length == 2);

        private static IQueryable<T> OrderByPropertyDescending<T>(
        this IQueryable<T> source, string propertyName)
        {
            if (typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase |
                BindingFlags.Public | BindingFlags.Instance) == null)
            {
                return null;
            }
            ParameterExpression paramterExpression = Expression.Parameter(typeof(T));
            Expression orderByProperty = Expression.Property(paramterExpression, propertyName);
            LambdaExpression lambda = Expression.Lambda(orderByProperty, paramterExpression);
            MethodInfo genericMethod =
              OrderByDescendingMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
            object ret = genericMethod.Invoke(null, new object[] { source, lambda });
            return (IQueryable<T>)ret;
        }

        private static Expression<Func<T, int>> CreateExpression<T>(string field)
        {
            var t1 = Expression.Parameter(typeof(T), "t1");
            var idProp = Expression.PropertyOrField(t1, field);
            var lamda = Expression.Lambda<Func<T, int>>(idProp, t1);
            return lamda;
        }

        private static object GetReflectedPropertyValue(this object subject, string field)
        {
            return subject.GetType().GetProperty(field).GetValue(subject, null);
        }

        public static Expression<Func<TSource, object>> GetExpression<TSource>(string propertyName)
        {
            var param = Expression.Parameter(typeof(TSource), "x");
            Expression conversion = Expression.Convert(Expression.Property
            (param, propertyName), typeof(object));   //important to use the Expression.Convert
            return Expression.Lambda<Func<TSource, object>>(conversion, param);
        }

        //makes deleget for specific prop
        public static Func<TSource, object> GetFunc<TSource>(string propertyName)
        {
            return GetExpression<TSource>(propertyName).Compile();  //only need compiled expression
        }

        //OrderBy overload
        //public static IOrderedEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> source, string propertyName)
        //{
        //    return source.OrderBy(GetFunc<TSource>(propertyName));
        //}

        //OrderBy overload
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
