using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework.Internal;

namespace ObjectPrinting
{
    public class PropertyPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        internal readonly string propertyName;

        public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig)
        {
            this.printingConfig = printingConfig;
        }
        public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig, string propertyName)
        {
            this.propertyName = propertyName ?? throw new AggregateException();
            this.printingConfig = printingConfig;
        }


        public PrintingConfig<TOwner> Using(Func<TPropType, string> print)
        {
            var func = print;
            if (propertyName != default)
            {
                if(!printingConfig.configuration.SpecialSerializeProperties.ContainsKey(typeof(TOwner)))
                    printingConfig.configuration.SpecialSerializeProperties[typeof(TOwner)] = new Dictionary<string, Delegate>();

                printingConfig.configuration.SpecialSerializeProperties[typeof(TOwner)][propertyName] = func;
            }
            else
            {
                if(!printingConfig.configuration.SpecialSerializeTypes.ContainsKey(typeof(TOwner)))
                    printingConfig.configuration.SpecialSerializeTypes[typeof(TOwner)] = new Dictionary<Type, Delegate>();
                
                printingConfig.configuration.SpecialSerializeTypes[typeof(TOwner)][typeof(TPropType)] = func;
            }
            return printingConfig;  
        }

        public PrintingConfig<TOwner> Using(CultureInfo culture)
        {
            if(!printingConfig.configuration.SpecialSerializeCulture.ContainsKey(typeof(TOwner)))
                printingConfig.configuration.SpecialSerializeCulture[typeof(TOwner)] = new Dictionary<Type, CultureInfo>();

            printingConfig.configuration.SpecialSerializeCulture[typeof(TOwner)][typeof(TPropType)] = culture;
            return printingConfig;
        }

        PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>.ParentConfig => printingConfig;
    }

    public interface IPropertyPrintingConfig<TOwner, TPropType>
    {
        PrintingConfig<TOwner> ParentConfig { get; }
    }
}