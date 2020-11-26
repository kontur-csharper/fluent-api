using System;
using System.Linq.Expressions;

namespace ObjectPrinting
{
    public partial class PrintingConfig<TOwner>
    {
        public class MemberChainPrintingContext<TProperty> : PrintingContext<TProperty>
        {
            private readonly Expression<Func<TOwner, TProperty>> member;

            public MemberChainPrintingContext(PrintingConfig<TOwner> ownerConfig,
                Expression<Func<TOwner, TProperty>> member)
            {
                OwnerConfig = ownerConfig.WithConfigForMemberOrDefault(member, out CurrentConfig);
                this.member = member;
            }

            protected override PrintingConfig<TOwner> Apply(PrintingConfig<TProperty> config)
            {
                return OwnerConfig.WithConfigForMember(member, config);
            }
        }
    }
}