using System;
using System.Reflection;

namespace ObjectPrinting
{
    public class MemberPrintingConfig<TOwner, TPropType> : IMemberPrintingConfig<TOwner, TPropType>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        private readonly MemberInfo _member;

        public MemberPrintingConfig(PrintingConfig<TOwner> printingConfig, MemberInfo member = null)
        {
            this.printingConfig = printingConfig;
            _member = member;
        }

        public PrintingConfig<TOwner> Using(Func<TPropType, string> print)
        {
            var config = (IPrintingConfig<TOwner>) printingConfig;
            if (_member is null)
                config.SpecialSerializationTypes[typeof(TPropType)] = print;
            else
                config.SpecialSerializationMembers[_member] = print;
            return printingConfig;
        }

        PrintingConfig<TOwner> IMemberPrintingConfig<TOwner, TPropType>.ParentConfig => printingConfig;
    }

    public interface IMemberPrintingConfig<TOwner, TPropType>
    {
        PrintingConfig<TOwner> ParentConfig { get; }
    }
}