using System;
using System.Linq;
using System.Linq.Expressions;

namespace ObjectPrinting.Solved
{
    public static class ObjectExtensions
    {
        public static string PrintToString<T>(this T obj)
        {
            return ObjectPrinter.For<T>().PrintToString(obj);
        }

        public static string GetFullNameProperty<TOwner, TPropType>(this Expression<Func<TOwner, TPropType>> memberSelector) =>
            string.Join(string.Empty, memberSelector.Body.ToString().SkipWhile(c => c != '.'));
    }
}