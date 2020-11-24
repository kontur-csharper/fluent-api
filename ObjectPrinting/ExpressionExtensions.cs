using System;
using System.Linq.Expressions;

namespace ObjectPrinting
{
    public static class ExpressionExtensions
    {
        public static string GetObjectName<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            if (!(propertyLambda.Body is MemberExpression member))
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");

            var names = member.GetNestedNames<TSource>();
            return $"{names}.{member.Member.Name}";
        }
    }
}