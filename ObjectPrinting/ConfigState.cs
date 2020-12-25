using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public class ConfigState
    {
        public ImmutableHashSet<Type> ExcludedTypes { get; set; }
        public ImmutableHashSet<MemberInfo> ExcludedMembers { get; set; }
        public ImmutableDictionary<Type, Delegate> AltSerializerForType { get; set; }
        public ImmutableDictionary<MemberInfo, Delegate> AltSerializerForMember { get; set; }
        public ImmutableDictionary<Type, CultureInfo> CultureForType { get; set; }

        public ConfigState()
        {
            ExcludedTypes = ImmutableHashSet.Create<Type>();
            ExcludedMembers = ImmutableHashSet.Create<MemberInfo>();
            AltSerializerForType = ImmutableDictionary.Create<Type, Delegate>();
            AltSerializerForMember = ImmutableDictionary.Create<MemberInfo, Delegate>();
            CultureForType = ImmutableDictionary.Create<Type, CultureInfo>();
        }

        public ConfigState(ConfigState previousState)
        {
            ExcludedTypes = previousState.ExcludedTypes;
            ExcludedMembers = previousState.ExcludedMembers;
            AltSerializerForType = previousState.AltSerializerForType;
            AltSerializerForMember = previousState.AltSerializerForMember;
            CultureForType = previousState.CultureForType;
        }
    }
}
