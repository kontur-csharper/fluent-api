using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectPrinting
{
    public interface IPrintingConfig<TOwner>
    {
        public HashSet<Type> ExcludedTypes { get; }
        public HashSet<MemberInfo> ExcludedMembers { get; }
        public Dictionary<Type, Delegate> SpecialSerializationTypes { get; }
        public Dictionary<MemberInfo, Delegate> SpecialSerializationMembers { get; }
    }
}
