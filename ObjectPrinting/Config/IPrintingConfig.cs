using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectPrinting.Config
{
    public interface IPrintingConfig<TOwner>
    {
        HashSet<Type> ExcludedTypes { get; }
        HashSet<PropertyInfo> ExcludedProperties { get; }
        HashSet<FieldInfo> ExcludedFields { get; }
        Dictionary<Type, Delegate> TypeToSerializer { get; }
        Dictionary<PropertyInfo, Delegate> PropertyToSerializer { get; }
        Dictionary<FieldInfo, Delegate> FieldToSerializer { get; }

        public string PrintToString(TOwner obj);
        public IConfig<TOwner, TPropType> Printing<TPropType>();
        public IConfig<TOwner, TPropType> Printing<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector);
        public IPrintingConfig<TOwner> Excluding<TPropType>();
        public IPrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector);
    }
}
