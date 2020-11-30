using System;

namespace ObjectPrinting
{
    public class PropertyPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        private readonly string memberName;
        private readonly Type declaringType;

        public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig)
        {
            this.printingConfig = printingConfig;
        }

        public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig, string memberName, Type declaringType)
        {
            this.memberName = memberName;
            this.printingConfig = printingConfig;
            this.declaringType = declaringType;
        }

        public PrintingConfig<TOwner> Using(Func<TPropType, string> print)
        {
            if (memberName is null)
            {
                printingConfig.AddSerializationForType(typeof(TPropType), print);
            }
            else
            {
                printingConfig.AddSerializationForProperty(memberName, print, declaringType);
            }
            return printingConfig;
        }

        PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>.ParentConfig => printingConfig;
        string  IPropertyPrintingConfig<TOwner, TPropType>.MemberName => memberName;
        Type IPropertyPrintingConfig<TOwner, TPropType>.DeclaringType => declaringType;
    }
    
    public interface IPropertyPrintingConfig<TOwner, TPropType>
    {
        PrintingConfig<TOwner> ParentConfig { get; }
        string MemberName { get; }
        Type DeclaringType { get; }
    }
}