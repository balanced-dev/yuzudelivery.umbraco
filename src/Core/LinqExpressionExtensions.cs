using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;

namespace YuzuDelivery.Umbraco.Core
{
    public static class LinqExpressionExtensions
    {
        public static string GetMemberName<Type, Member>(this Expression<Func<Type, Member>> lambda)
        {
            var methodExpression = lambda.Body as MethodCallExpression;
            var memberExpression = lambda.Body as MemberExpression;

            if (methodExpression != null)
            {
                var propInfo = methodExpression.Method as MethodInfo;
                if (propInfo == null)
                    throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", lambda.ToString()));
                else
                    return propInfo.Name;
            }
            else if (memberExpression != null)
            {
                PropertyInfo propInfo = memberExpression.Member as PropertyInfo;
                if (propInfo == null)
                    throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", lambda.ToString()));
                else
                    return propInfo.Name;

            }
            throw new ArgumentException(string.Format("Expression '{0}' not supported.", lambda.ToString()));
        }
    }
}
