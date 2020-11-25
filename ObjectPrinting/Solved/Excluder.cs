using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectPrinting.Solved
{
    internal class Excluder
    {
        private readonly HashSet<Type> excludingTypes = new HashSet<Type>();
        private readonly HashSet<string> excludingFields = new HashSet<string>();

        private void Exclude<TPropType>() => excludingTypes.Add(typeof(TPropType));

        internal void Exclude<TOwner, TPropType>(Expression<Func<TOwner, TPropType>> memberSelector = null)
        {
            var fullName = memberSelector?.GetFullNameProperty();
            if (fullName == null)
                Exclude<TPropType>();
            else
                excludingFields.Add(fullName);
        }

        internal bool IsExclude(Type type, string fullName) =>
            (fullName != null && excludingFields.Contains(fullName)) ||
            (type != null && excludingTypes.Contains(type));
    }
}
