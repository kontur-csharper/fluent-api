using System;
using System.Reflection;

namespace ObjectPrinting
{
    public class MemberPrintingConfig<TOwner, TProperty> : IMemberPrintingConfig<TOwner, TProperty>
    {
        private readonly PrintingConfig<TOwner> parentPrintingConfig;
        private readonly MemberInfo member;
        PrintingConfig<TOwner> IMemberPrintingConfig<TOwner, TProperty>.ParentPrintingConfig => parentPrintingConfig;

        public MemberPrintingConfig(MemberInfo member, PrintingConfig<TOwner> parentPrintingConfig)
        {
            this.member = member;
            this.parentPrintingConfig = parentPrintingConfig;
        }

        public PrintingConfig<TOwner> SetSerializer(Func<TProperty, string> serializer)
        {
            var newConfig = new PrintingConfig<TOwner>(parentPrintingConfig);
            ((IPrintingConfig)newConfig).State.AltSerializerForMember = ((IPrintingConfig)parentPrintingConfig).
                State.AltSerializerForMember.AddOrUpdate(member, serializer);
            return newConfig;
        }
    }

    public interface IMemberPrintingConfig<TOwner, TProperty>
    {
        PrintingConfig<TOwner> ParentPrintingConfig { get; }
    }
}
