using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectPrinting
{
    public partial class PrintingConfig<TOwner> : PrintingConfigBase
    {
        public string PrintToString(TOwner obj) => PrintToString(obj, 0);

        public PrintingContext<TMember> Printing<TMember>()
            => new TypePrintingContext<TMember>(this, typeof(TMember));

        public PrintingContext<TMember> Printing<TMember>(Expression<Func<TOwner, TMember>> member)
            => new MemberChainPrintingContext<TMember>(this, member);

        public PrintingConfig<TOwner> Excluding<TMember>()
            => Printing<TMember>().Using((PrintingConfig<TMember>) null);

        public PrintingConfig<TOwner> Excluding<TMember>(Expression<Func<TOwner, TMember>> member)
            => Printing(member).Using((PrintingConfig<TMember>) null);

        private PrintingConfig<TOwner> WithPrintFunction(Func<TOwner, string> printFunc)
        {
            var result = (PrintingConfig<TOwner>) Copy();
            result.PrintFunc = (obj) => printFunc((TOwner) obj);
            return result;
        }

        private PrintingConfig<TOwner> WithConfigForMember<TMember>(
            Expression<Func<TOwner, TMember>> member,
            PrintingConfig<TMember> config)
        {
            var members = GetMembersChain(member).ToArray();
            return (PrintingConfig<TOwner>) WithConfigForMember(members, config);
        }

        private PrintingConfig<TOwner> WithConfigForMemberOrDefault<TMember>(
            Expression<Func<TOwner, TMember>> member,
            out PrintingConfig<TMember> config)
        {
            var members = GetMembersChain(member).ToArray();
            var result = WithConfigForMemberOrDefault(members, new PrintingConfig<TMember>(), out var resultConfig);
            config = ToGeneric<TMember>(resultConfig);
            return (PrintingConfig<TOwner>) result;
        }

        private IEnumerable<MemberInfo> GetMembersChain<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
                return GetMembersChain(memberExpression);
            throw new InvalidOperationException("Only member expressions are supported!");
        }

        private IEnumerable<MemberInfo> GetMembersChain(MemberExpression expression) =>
            GetReverseMembersChain(expression).Reverse();

        private IEnumerable<MemberInfo> GetReverseMembersChain(MemberExpression expression)
        {
            while (true)
            {
                yield return expression.Member;
                if (expression.Expression is MemberExpression memberExpression)
                    expression = memberExpression;
                else if (expression.Expression is ParameterExpression) yield break;
                else throw new InvalidOperationException("Only member expressions are supported!");
            }
        }

        protected override PrintingConfigBase Copy()
            => new PrintingConfig<TOwner>
            {
                PrintFunc = PrintFunc,
                MemberConfigs = MemberConfigs,
                TypeConfigs = TypeConfigs
            };
    }
}