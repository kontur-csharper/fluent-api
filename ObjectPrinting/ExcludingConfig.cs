using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectPrinting
{
    public class ExcludingConfig<TOwner>
    {
        private readonly HashSet<Type> typesToExclude = new HashSet<Type>();
        private readonly HashSet<MemberInfo> propertiesToExclude = new HashSet<MemberInfo>();
        
        public void Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var body = (MemberExpression) memberSelector.Body;
            propertiesToExclude.Add(body.Member);
        }

        internal void Excluding<TPropType>()
        {
            typesToExclude.Add(typeof(TPropType));
        }

        public bool IsExcluded(MemberInfo property) => typesToExclude.Contains(property.GetMemberType()) ||
                                                       propertiesToExclude.Contains(property);
    }
}