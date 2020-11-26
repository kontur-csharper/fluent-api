using System;
using System.Globalization;
using System.Reflection;

namespace ObjectPrinting.Solved
{
    public class PropertyPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        private readonly PrintingParameters parameters;
        private readonly MemberInfo member;
        private readonly Type type;

        public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig, MemberInfo member)
        {
            this.printingConfig = printingConfig;
            this.member = member;
            parameters = printingConfig.Parameters;
        }

        public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig, Type type)
        {
            this.printingConfig = printingConfig;
            this.type = type;
            parameters = printingConfig.Parameters;
        }

        public PrintingConfig<TOwner> Using(Func<TPropType, string> print)
        {
            return GetNewConfig(obj => print((TPropType) obj));
        }

        public PrintingConfig<TOwner> Using(CultureInfo culture)
        {
            return GetNewConfig(obj =>
                obj is IFormattable formattable && culture != null ?
                    formattable.ToString(null, culture) : obj.ToString());
        }

        private PrintingConfig<TOwner> GetNewConfig(Func<object, string> serializator)
        {
            if (member != null)
            {
                return new PrintingConfig<TOwner>
                    (parameters.AddMemberToSerialize(member, serializator));
            }
            if (type != null)
            {
                return new PrintingConfig<TOwner>
                    (parameters.AddTypeToSerialize(typeof(TPropType), serializator));
            }
            throw new InvalidOperationException();
        }

        PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>.ParentConfig => printingConfig;
    }

    public interface IPropertyPrintingConfig<TOwner, TPropType>
    {
        PrintingConfig<TOwner> ParentConfig { get; }
    }
}