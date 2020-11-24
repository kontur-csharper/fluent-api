using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectPrinting
{
    public static class ExpressionExtensions
    {
        public static PropertyInfo GetPropertyInfo<TOwner, TPropType>(
            this Expression<Func<TOwner, TPropType>> memberSelector)
        {
            return ((MemberExpression) memberSelector.Body).Member as PropertyInfo;
        }
    }
}