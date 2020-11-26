using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;

namespace ObjectPrinting.Solved
{
    public class PrintingParameters
    {

        public readonly ImmutableHashSet<Type> typesToExclude;
        public readonly ImmutableDictionary<Type, Func<object, string>> typesToSerialize;
        public readonly ImmutableHashSet<MemberInfo> membersToExclude;
        public readonly ImmutableDictionary<MemberInfo, Func<object, string>> membersToSerialize;

        public static PrintingParameters Default => new PrintingParameters(ImmutableHashSet<Type>.Empty,
            ImmutableDictionary<Type, Func<object, string>>.Empty,
            ImmutableHashSet<MemberInfo>.Empty,
            ImmutableDictionary<MemberInfo, Func<object, string>>.Empty);

        public PrintingParameters(ImmutableHashSet<Type> typesToExclude,
            ImmutableDictionary<Type, Func<object, string>> typesToSerialize,
            ImmutableHashSet<MemberInfo> membersToExclude,
            ImmutableDictionary<MemberInfo, Func<object, string>> membersToSerialize)
        {
            this.typesToExclude = typesToExclude;
            this.typesToSerialize = typesToSerialize;
            this.membersToExclude = membersToExclude;
            this.membersToSerialize = membersToSerialize;
        }

        public PrintingParameters AddTypeToExclude(Type type)
        {
            return new PrintingParameters(typesToExclude.Add(type),
                typesToSerialize, membersToExclude, membersToSerialize);
        }

        public PrintingParameters AddTypeToSerialize(Type type, Func<object, string> serializator)
        {
            return new PrintingParameters(typesToExclude,
                typesToSerialize.SetItem(type, serializator), membersToExclude, membersToSerialize);
        }

        public PrintingParameters AddMemberToExclude(MemberInfo member)
        {
            return new PrintingParameters(typesToExclude,
                typesToSerialize, membersToExclude.Add(member), membersToSerialize);
        }

        public PrintingParameters AddMemberToSerialize(MemberInfo member, Func<object, string> serializator)
        {
            return new PrintingParameters(typesToExclude,
                typesToSerialize, membersToExclude, membersToSerialize.SetItem(member, serializator));
        }
    }
}
