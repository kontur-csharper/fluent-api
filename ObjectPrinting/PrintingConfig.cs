using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner> : IPrintingConfig
    {
        private ConfigState state;
        ConfigState IPrintingConfig.State => state;
        public ObjectPrinter<TOwner> Build()
        {
            return new ObjectPrinter<TOwner>(this);
        }

        public PrintingConfig()
        {
            state = new ConfigState();
        }

        public PrintingConfig(PrintingConfig<TOwner> previousConfig)
        {
            state = new ConfigState(previousConfig.state);
        }

        public PrintingConfig<TOwner> Exclude<TExcluded>()
        {
            var newConfig = new PrintingConfig<TOwner>(this);
            newConfig.state.ExcludedTypes = state.ExcludedTypes.Add(typeof(TExcluded));
            return newConfig;
        }

        public PrintingConfig<TOwner> Exclude<TMember>(Expression<Func<TOwner, TMember>> member)
        {
            var newConfig = new PrintingConfig<TOwner>(this);
            var memberInfo = ((MemberExpression)member.Body).Member;
            newConfig.state.ExcludedMembers = state.ExcludedMembers.Add(memberInfo);
            return newConfig;
        }

        public PrintingConfig<TOwner> SetSerializerFor<T>(Func<T, string> serializer)
        {
            var newConfig = new PrintingConfig<TOwner>(this);
            newConfig.state.AltSerializerForType = state.AltSerializerForType.AddOrUpdate(typeof(T), serializer);
            return newConfig;
        }

        public PrintingConfig<TOwner> SetCultureFor<T>(CultureInfo culture)
            where T : IFormattable
        {
            if (typeof(IFormattable).IsAssignableFrom(typeof(T)))
            {
                var newConfig = new PrintingConfig<TOwner>(this);
                newConfig.state.CultureForType = state.CultureForType.AddOrUpdate(typeof(T), culture);
                return newConfig;
            }

            throw new ArgumentException("Невозможно установить культуру для данного типа");
        }

        public MemberPrintingConfig<TOwner, TMember> ForMember<TMember>(Expression<Func<TOwner, TMember>> member)
        {
            var memberInfo = ((MemberExpression)member.Body).Member;
            return new MemberPrintingConfig<TOwner, TMember>(memberInfo, this);
        }
    }

    public interface IPrintingConfig
    {
        ConfigState State { get; }
    }
}
