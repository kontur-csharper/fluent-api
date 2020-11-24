using System;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public class PrintingConfigBase
    {
        protected Func<object, string> PrintFunc;

        protected ImmutableDictionary<MemberInfo, PrintingConfigBase> MemberConfigs =
            ImmutableDictionary<MemberInfo, PrintingConfigBase>.Empty;

        protected ImmutableDictionary<Type, PrintingConfigBase> TypeConfigs =
            ImmutableDictionary<Type, PrintingConfigBase>.Empty;

        protected virtual PrintingConfigBase Copy() => new PrintingConfigBase
        {
            PrintFunc = PrintFunc,
            MemberConfigs = MemberConfigs,
            TypeConfigs = TypeConfigs
        };

        private PrintingConfigBase WithParentTypeConfigs(
            ImmutableDictionary<Type, PrintingConfigBase> parentTypeConfigs)
        {
            var newItems = parentTypeConfigs.Where(p => !TypeConfigs.ContainsKey(p.Key));
            var result = Copy();
            result.TypeConfigs = TypeConfigs.AddRange(newItems);
            return result;
        }

        protected PrintingConfigBase WithConfigForMember(MemberInfo[] memberChain, PrintingConfigBase config)
            => Copy().WithConfigForMember(memberChain, config, 0);

        private PrintingConfigBase WithConfigForMember(MemberInfo[] memberChain, PrintingConfigBase config,
            int indexInChain)
        {
            if (indexInChain == memberChain.Length - 1)
                return WithConfigForMember(memberChain[indexInChain], config);

            if (!MemberConfigs.TryGetValue(memberChain[indexInChain], out var currentConfig))
                currentConfig = new PrintingConfigBase();

            var result = Copy();
            result.MemberConfigs = result.MemberConfigs.SetItem(memberChain[indexInChain],
                currentConfig.WithConfigForMember(memberChain, config, indexInChain + 1));
            return result;
        }

        protected PrintingConfigBase WithConfigForMemberOrDefault(
            MemberInfo[] memberChain,
            PrintingConfigBase defaultConfig,
            out PrintingConfigBase config)
        {
            config = this;
            for (var i = 0; i < memberChain.Length; i++)
            {
                var hasConfig = config.MemberConfigs.TryGetValue(memberChain[i], out config);
                if (!hasConfig || config == null)
                {
                    if (i < memberChain.Length - 1 || !hasConfig || defaultConfig != null)
                        return WithConfigForMember(memberChain, config = defaultConfig);
                    config = null;
                    return this;
                }
            }

            return this;
        }

        private PrintingConfigBase WithConfigForMember(MemberInfo memberChain, PrintingConfigBase config)
        {
            var result = Copy();
            result.MemberConfigs = result.MemberConfigs.SetItem(memberChain, config);
            return result;
        }

        protected static PrintingConfig<T> ToGeneric<T>(PrintingConfigBase config)
        {
            if (config is PrintingConfig<T> generic) return generic;
            if (config == null) return null;
            if (config.GetType() == typeof(PrintingConfigBase))
            {
                return new PrintingConfig<T>
                {
                    PrintFunc = config.PrintFunc,
                    MemberConfigs = config.MemberConfigs,
                    TypeConfigs = config.TypeConfigs
                };
            }

            throw new Exception($"{config} is not generic of {typeof(T)}!");
        }

        protected string PrintToString(object obj, int nestingLevel)
        {
            if (nestingLevel > 5) return "...";
            if (obj == null) return "null";

            if (PrintFunc != null) return PrintFunc(obj);
            var finalTypes = new[]
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan)
            };
            if (finalTypes.Contains(obj.GetType())) return obj.ToString();

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.Append(type.Name);
            if (obj is IEnumerable enumerable)
            {
                sb.AppendLine(" [");
                foreach (var o in enumerable)
                    sb.Append(identation).AppendLine(PrintChild(obj, o, nestingLevel));
                sb.Append('\t', nestingLevel).Append("]");
            }
            else
            {
                var members = type.GetProperties();
                if (members.Any()) sb.AppendLine();
                sb.AppendJoin('\n', members.Select(
                    m => PrintMember(obj, m, nestingLevel)).Where(s => s != null));
            }

            return sb.ToString();
        }

        private string PrintChild(object obj, object child, int nestingLevel)
        {
            if (child == null || !TypeConfigs.TryGetValue(child.GetType(), out var config))
                config = new PrintingConfigBase {TypeConfigs = TypeConfigs};
            else if (config == null) return null;
            else config = config.WithParentTypeConfigs(TypeConfigs);
            return config.PrintToString(child, nestingLevel + 1);
        }

        private string PrintMember(object obj, MemberInfo member, int nestingLevel)
        {
            object value;
            Type type;
            if (member is PropertyInfo property)
            {
                if (!property.CanRead) return $"{member.Name} (not readable)";
                value = property.GetValue(obj);
                type = property.PropertyType;
            }
            else if (member is FieldInfo field)
            {
                value = field.GetValue(obj);
                type = field.FieldType;
            }
            else throw new InvalidOperationException($"{member.MemberType} is not supported!");
            
            if (!(MemberConfigs.TryGetValue(member, out var config)
                  || TypeConfigs.TryGetValue(type, out config)))
                config = new PrintingConfigBase {TypeConfigs = TypeConfigs};
            else if (config == null) return null;
            else config = config.WithParentTypeConfigs(TypeConfigs);

            var identation = new string('\t', nestingLevel + 1);
            return $"{identation}{member.Name} = {config.PrintToString(value, nestingLevel + 1)}";
        }
    }
}