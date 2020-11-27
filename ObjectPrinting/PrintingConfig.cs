using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        private readonly HashSet<Type> typesToDefaultSerialize = new HashSet<Type>
        {
            typeof(int), typeof(double), typeof(float), typeof(string), typeof(bool),
            typeof(DateTime), typeof(TimeSpan), typeof(Guid)
        };

        private readonly HashSet<Type> typesToExclude = new HashSet<Type>();
        private readonly HashSet<MemberInfo> membersToExclude = new HashSet<MemberInfo>();
        private readonly HashSet<object> serializingMembersCache = new HashSet<object>();

        internal Dictionary<Type, Delegate> AlternativeSerializersForTypes = new Dictionary<Type, Delegate>();
        internal Dictionary<MemberInfo, Delegate> AlternativeSerializersForMembers =
            new Dictionary<MemberInfo, Delegate>(); 

        private string PrintToString(object obj, int nestingLevel, MemberInfo info)
        {
            if (obj == null)
                return "null" + Environment.NewLine;

            if (typesToDefaultSerialize.Contains(obj.GetType()))
                return SerializeValue(obj, info);

            if (obj is IEnumerable enumerable)
                return SerializeEnumerable(enumerable, nestingLevel);

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);

            foreach (var member in GetMembersToSerialize(obj))
            {
                if (serializingMembersCache.Contains(member.value))
                {
                    sb.Append($"{identation}{member.info.Name} = [Cyclic reference detected]{Environment.NewLine}"); 
                    continue;
                }

                serializingMembersCache.Add(member.value);
                var serializedValue = $"{PrintToString(member.value, nestingLevel + 1, member.info)}";

                sb.Append($"{identation}{member.info.Name} = {serializedValue}");
                serializingMembersCache.Remove(member.value);
            }

            return sb.ToString();
        }

        private string SerializeValue(object obj, MemberInfo info)
        {
            var type = obj.GetType();

            if (info != null && AlternativeSerializersForMembers.TryGetValue(info, out var serializer))
                return serializer.DynamicInvoke(obj) + Environment.NewLine;

            if (AlternativeSerializersForTypes.TryGetValue(type, out var typeSerializer))
                return typeSerializer.DynamicInvoke(obj) + Environment.NewLine;

            return obj + Environment.NewLine;
        }

        private string SerializeEnumerable(IEnumerable enumerable, int nestingLevel)
        {
            var sb = new StringBuilder($"{enumerable.GetType().Name}{Environment.NewLine}");
            var identation = new string('\t', nestingLevel + 1);
            sb.Append($"{new string('\t', nestingLevel)}{{{Environment.NewLine}");
            var startLength = sb.Length;

            foreach (var obj in enumerable)
                sb.Append($"{identation}{PrintToString(obj, nestingLevel + 1, null)}");

            if (sb.Length == startLength)
                sb.Append("Empty");
            sb.Append($"{new string('\t', nestingLevel)}}}{Environment.NewLine}");
            
            return sb.ToString();
        }

        private IEnumerable<(MemberInfo info, object value)> GetMembersToSerialize(object obj)
        {
            var type = obj.GetType();

            foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Instance))
            {
                if (membersToExclude.Contains(member))
                    continue;

                if (member is FieldInfo fi)
                    if (!typesToExclude.Contains(fi.FieldType))
                        yield return (fi, fi.GetValue(obj));

                if(member is PropertyInfo pi)
                    if (!typesToExclude.Contains(pi.PropertyType))
                        yield return (pi, pi.GetValue(obj));
            }
        }

        public string PrintToString(TOwner obj)
        {
            serializingMembersCache.Add(obj);
            return PrintToString(obj, 0, null);
        }

        public MemberPrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new MemberPrintingConfig<TOwner, TPropType>(this);
        }

        public MemberPrintingConfig<TOwner, TPropType> Printing<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            return new MemberPrintingConfig<TOwner, TPropType>(this, memberSelector);
        }

        internal PrintingConfig<TOwner> Excluding<TPropType>()
        {
            typesToExclude.Add(typeof(TPropType));
            return this;
        }

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            membersToExclude.Add(((MemberExpression) memberSelector.Body).Member);
            return this;
        }
    }
}