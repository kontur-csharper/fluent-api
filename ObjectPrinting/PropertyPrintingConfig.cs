using System;
using System.Reflection;

namespace ObjectPrinting
{
    public class PropertyPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        private readonly Target target;
        private readonly MemberInfo targetProp;

        private PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig, Target target,
            MemberInfo propertyInfo = null)
        {
            this.target = target;
            targetProp = propertyInfo;
            this.printingConfig = printingConfig;
        }

        PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>.ParentConfig => printingConfig;

        public PrintingConfig<TOwner> Using(Func<TPropType, string> print)
        {
            var objPrint = new Func<object, string>(obj =>
            {
                var prop = (TPropType) obj;
                return print(prop);
            });


            switch (target)
            {
                case Target.Type:
                    ((IPrintingConfig<TOwner>) printingConfig).TypesSerialization =
                        ((IPrintingConfig<TOwner>) printingConfig).TypesSerialization
                        .AddOrReplace(typeof(TPropType), objPrint);
                    break;

                case Target.Member:
                    ((IPrintingConfig<TOwner>) printingConfig).PropsSerialization =
                        ((IPrintingConfig<TOwner>) printingConfig).PropsSerialization
                        .AddOrReplace(targetProp, objPrint);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Unknown target");
            }

            return printingConfig;
        }

        public static PropertyPrintingConfig<TOwner, TTargetType> For<TTargetType>(PrintingConfig<TOwner> parentConfig)
        {
            return new PropertyPrintingConfig<TOwner, TTargetType>(parentConfig, Target.Type);
        }

        public static PropertyPrintingConfig<TOwner, TPropType> For(PrintingConfig<TOwner> parentConfig,
            PropertyInfo propertyInfo)
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(parentConfig, Target.Member, propertyInfo);
        }
    }

    public enum Target
    {
        Type,
        Member
    }

    public interface IPropertyPrintingConfig<TOwner, TPropType>
    {
        PrintingConfig<TOwner> ParentConfig { get; }
    }
}