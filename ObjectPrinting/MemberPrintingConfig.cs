using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectPrinting
{
    public class MemberPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        internal readonly MemberInfo Info = null;

        public MemberPrintingConfig(PrintingConfig<TOwner> printingConfig)
        {
            this.printingConfig = printingConfig;
        }

        public MemberPrintingConfig(PrintingConfig<TOwner> printingConfig, Expression<Func<TOwner, TPropType>> memberSelector)
        {
            this.printingConfig = printingConfig;
            Info = ((MemberExpression) memberSelector.Body).Member;
        }

        public PrintingConfig<TOwner> Using(Func<TPropType, string> print)
        {
            if (Info is null)
                printingConfig.AlternativeSerializersForTypes[typeof(TPropType)] = print;
            else
                printingConfig.AlternativeSerializersForMembers[Info] = print;

            return printingConfig;
        }

        PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>.ParentConfig => printingConfig;
    }

    public interface IPropertyPrintingConfig<TOwner, TPropType>
    {
        PrintingConfig<TOwner> ParentConfig { get; }
    }
}