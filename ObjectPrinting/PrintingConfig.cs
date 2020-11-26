using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner> : IPrintingConfig<TOwner>
    {
        private readonly HashSet<Type> _excludedTypes = new HashSet<Type>();

        private readonly HashSet<MemberInfo> _excludedMembers = new HashSet<MemberInfo>();

        HashSet<Type> IPrintingConfig<TOwner>.ExcludedTypes => _excludedTypes;

        HashSet<MemberInfo> IPrintingConfig<TOwner>.ExcludedMembers => _excludedMembers;

        Dictionary<Type, Delegate> IPrintingConfig<TOwner>.SpecialSerializationTypes { get; } =
            new Dictionary<Type, Delegate>();

        Dictionary<MemberInfo, Delegate> IPrintingConfig<TOwner>.SpecialSerializationMembers { get; } =
            new Dictionary<MemberInfo, Delegate>();

        public MemberPrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new MemberPrintingConfig<TOwner, TPropType>(this);
        }

        public MemberPrintingConfig<TOwner, TPropType> Printing<TPropType>(
            Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var member = ((MemberExpression) memberSelector.Body).Member;
            return new MemberPrintingConfig<TOwner, TPropType>(this, member);
        }

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var member = ((MemberExpression) memberSelector.Body).Member;
            _excludedMembers.Add(member);
            return this;
        }

        public PrintingConfig<TOwner> Excluding<TPropType>()
        {
            _excludedTypes.Add(typeof(TPropType));
            return this;
        }

        public string PrintToString(TOwner obj)
        {
            var s = new Serializer<TOwner>(this);
            return s.Serialize(obj);
        }
    }
}