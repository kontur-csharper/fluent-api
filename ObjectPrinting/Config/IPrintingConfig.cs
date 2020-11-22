using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectPrinting
{
    public interface IPrintingConfig<TOwner>
    {
        HashSet<Type> ExcludedTypes { get; }
        Dictionary<Type, Delegate> TypeSerialization { get; }
        Dictionary<PropertyInfo, Delegate> PropertySerialization { get; }
        Dictionary<FieldInfo, Delegate> FieldSerialization { get; }
        HashSet<PropertyInfo> ExcludedProperties { get; }
        HashSet<FieldInfo> ExcludedFields { get; }

        public string PrintToString(TOwner obj);
        public IConfig<TOwner, TPropType> Printing<TPropType>();
        public IConfig<TOwner, TPropType> Printing<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector);
        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector);
        public PrintingConfig<TOwner> Excluding<TPropType>();
    }
}
