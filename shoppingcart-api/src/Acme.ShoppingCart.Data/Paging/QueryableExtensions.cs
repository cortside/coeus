using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Acme.ShoppingCart.Data.Paging;

namespace Acme.ShoppingCart.Data.Paging {
    public static class QueryableExtensions {
        public static IQueryable<T> ToPagedQuery<T>(this IQueryable<T> query, int page, int pageSize) {
            return query.Skip(pageSize * (page - 1)).Take(pageSize);
        }

        public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, bool orderByDescending) {
            return orderByDescending ? source.OrderByDescending(keySelector) : source.OrderBy(keySelector);
        }

        /// <summary>
        /// the sortParameters must have the next format:
        /// if want to sort by descending use '-', for example: -ColumnName, if want to sort ascending use just the name of the column, for example: ColumnName
        /// if want to sot by multiple parameters separarate them with a comma, for example ColumnName1,ColumnName2
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">the IQUERABLE send from the persistence class</param>
        /// <param name="sortParameters">the name of the sort parameters</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ToSortedQuery<T>(this IQueryable<T> query, string sortParameters) {
            sortParameters = sortParameters.Replace("+", "");
            string[] parameters = sortParameters.Split(',');

            var expression = query.Expression;
            for (var index = 0; index < parameters.Length; index++) {
                var sortParameter = parameters[index];
                if (string.IsNullOrEmpty(sortParameter)) {
                    continue;
                }
                string orderType = sortParameter[..1];

                var method = GetOrderMethod(index, orderType);

                string propertyName = string.Equals(orderType, "-", StringComparison.OrdinalIgnoreCase)
                    ? sortParameter[1..]
                    : sortParameter;

                var propertyLambda = GetPropertyLambda<T>(propertyName);

                if (propertyLambda.Item2.Name.Contains("nullable", StringComparison.CurrentCultureIgnoreCase)) {
                    // first add an expression where we order by the property.HasValue
                    var nullCheckProperty = GetPropertyLambda<T>($"{propertyName}.HasValue");
                    expression = Expression.Call(typeof(Queryable), method, new Type[] { query.ElementType, nullCheckProperty.Item2 },
                        expression,
                        Expression.Quote(nullCheckProperty.Item1));

                    // since we may have taken the first order position by inserting this new one
                    // update the order method
                    method = GetOrderMethod(index + 1, orderType);
                }

                expression = Expression.Call(typeof(Queryable), method,
                    new Type[] { query.ElementType, propertyLambda.Item2 },
                    expression, Expression.Quote(propertyLambda.Item1));
            }
            return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(expression);
        }

        private static string GetOrderMethod(int index, string orderType) {
            if (string.Equals(orderType, "-", StringComparison.OrdinalIgnoreCase)) {
                if (index == 0) {
                    return "OrderByDescending";
                } else {
                    return "ThenByDescending";
                }
            } else if (index == 0) {
                return "OrderBy";
            } else {
                return "ThenBy";
            }
        }

        private static (Expression, Type) GetPropertyLambda<T>(string propertyName) {
            var arg = Expression.Parameter(typeof(T), "x");
            if (propertyName.Contains('.')) {
                Expression body = arg;
                var members = propertyName.Split('.');

                Type propertyType = typeof(T);
                foreach (var subMember in members) {
                    body = Expression.PropertyOrField(body, subMember);

                    var propertyInfo = propertyType.GetProperty(subMember);
                    if (propertyInfo == null) {
                        throw new ArgumentException("Sort property does not exist!");
                    }

                    propertyType = propertyInfo.PropertyType;
                }

                var conv = Expression.Convert(body, propertyType);
                var exp = Expression.Lambda(conv, new ParameterExpression[] { arg });
                return (exp, propertyType);
            } else {
                var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null) {
                    throw new ArgumentException("Sort property does not exist!");
                }

                var propertyType = propertyInfo.PropertyType;
                var property = Expression.Property(arg, propertyName);

                //return the property as object
                var conv = Expression.Convert(property, propertyType);
                var exp = Expression.Lambda(conv, new ParameterExpression[] { arg });
                return (exp, propertyType);
            }
        }
    }
}
