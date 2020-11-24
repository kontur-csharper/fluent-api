using System;

namespace ObjectPrinting
{
    public class MemberPrintingConfig<TOwner, TMemberType> : IMemberPrintingConfig
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        private Func<object, string> printMember = obj => obj.ToString();

        public MemberPrintingConfig(PrintingConfig<TOwner> printingConfig)
        {
            this.printingConfig = printingConfig;
        }

        public PrintingConfig<TOwner> Using(Func<TMemberType, string> print)
        {
            printMember = obj => print((TMemberType) obj);
            return printingConfig;
        }

        Func<object, string> IMemberPrintingConfig.PrintMember => printMember;
    }

    public interface IMemberPrintingConfig
    {
        Func<object, string> PrintMember { get; }
    }
}