using System.Linq.Expressions;

namespace ObjectPrinting
{
    public static class MemberExpressionExtensions
    {
        public static string GetNestedNames<TSource>(this MemberExpression expression)
        {
            if (!(expression.Expression is MemberExpression member))
                return typeof(TSource).Name;
            return $"{member.GetNestedNames<TSource>()}.{member.Member.Name}";
        }
    }
}