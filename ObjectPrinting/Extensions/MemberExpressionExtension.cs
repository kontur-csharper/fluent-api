using System.Linq.Expressions;

namespace ObjectPrinting.Extensions
{
    public static class MemberExpressionExtension
    {
        public static string GetFullNestedName(this MemberExpression memberExpression)
        {
            if (memberExpression.Expression is MemberExpression parentMemberExpression)
                return $"{GetFullNestedName(parentMemberExpression)}.{parentMemberExpression.Member.Name}";
            return memberExpression.Member.DeclaringType!.Name;
        }
    }
}