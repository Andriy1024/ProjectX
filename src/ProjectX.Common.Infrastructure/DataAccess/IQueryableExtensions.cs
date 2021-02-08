using ProjectX.Common.SeedWork;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ProjectX.Common.Infrastructure.DataAccess
{
    public static class IQueryableExtensions
    {
        public static IQueryable<TEntity> WithPagination<TEntity>(this IQueryable<TEntity> source, IPaginationOptions paginable)
            => paginable == default ? source : source.Skip(paginable.Skip).Take(paginable.Take);

        public static IQueryable<TEntity> WithOrdering<TEntity>(this IQueryable<TEntity> source, IOrderingOptions orderable)
            => source.OrderBy(orderable.OrderBy, orderable.Descending);

        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string propertyName, bool reverse = false)
        {
            if (string.IsNullOrEmpty(propertyName) || typeof(TEntity).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public) == null)
                return source;

            return reverse ? source.OrderByDescending(propertyName) : source.OrderBy(propertyName);
        }

        private static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
            => source.OrderBy(CreateExpression<T>(propertyName));

        private static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
            => source.OrderByDescending(CreateExpression<T>(propertyName));

        private static Expression<Func<T, object>> CreateExpression<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }
    }
}
