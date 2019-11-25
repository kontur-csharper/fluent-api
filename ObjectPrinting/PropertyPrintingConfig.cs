﻿using System;
using System.Globalization;

namespace ObjectPrinting
{
    public class PropertyPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        private readonly string propertyName;

        public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig, string propertyName = null)
        {
            this.printingConfig = printingConfig;
            this.propertyName = propertyName;
        }

        public PrintingConfig<TOwner> Using(Func<TPropType, string> printingFunction)
        {
            if (propertyName == null)
            {
                ((IPrintingConfig<TOwner>) printingConfig).TypePrintingFunctions[typeof(TPropType)] = printingFunction;
            }
            else
            {
                ((IPrintingConfig<TOwner>) printingConfig).PropertyPrintingFunctions[propertyName] =
                    printingFunction;
            }

            return printingConfig;
        }

        public PrintingConfig<TOwner> Using(CultureInfo culture)
        {
            ((IPrintingConfig<TOwner>) printingConfig).TypeCultures[typeof(TPropType)] = culture;
            return printingConfig;
        }

        PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>.ParentConfig => printingConfig;
    }
}