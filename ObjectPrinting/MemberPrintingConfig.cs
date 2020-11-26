using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Reflection;

namespace ObjectPrinting
{
    public class MemberPrintingConfig<TOwner, TPropType> : IMemberPrintingConfig<TOwner, TPropType>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        internal readonly MemberInfo memberInfo;

        public MemberPrintingConfig(PrintingConfig<TOwner> printingConfig, MemberInfo memberInfo = null)
        {
            this.printingConfig = printingConfig;
            this.memberInfo = memberInfo;
        }

        public PrintingConfig<TOwner> Using(Func<TPropType, string> print)
        {
            if (memberInfo == null)
                printingConfig.typeMemberConfigs[typeof(TPropType)] = print;
            else
                printingConfig.nameMemberConfigs[memberInfo] = print;
            return printingConfig;
        }

        PrintingConfig<TOwner> IMemberPrintingConfig<TOwner, TPropType>.ParentConfig => printingConfig;
    }

    public interface IMemberPrintingConfig<TOwner, TPropType>
    {
        PrintingConfig<TOwner> ParentConfig { get; }
    }
}