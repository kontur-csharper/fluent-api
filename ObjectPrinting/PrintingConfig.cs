using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        private readonly Type[] simpleTypes =
        {
            typeof(int), typeof(double), typeof(float), typeof(string),
            typeof(DateTime), typeof(TimeSpan), typeof(Guid)
        };

        private readonly string endOfLine = Environment.NewLine;
        private readonly List<object> printedObjects = new List<object>();
        private readonly List<MemberVariable> excludingMembers = new List<MemberVariable>();
        private readonly List<Type> excludingTypes = new List<Type>();

        private readonly Dictionary<MemberVariable, Func<object, string>> customPrintForMember =
            new Dictionary<MemberVariable, Func<object, string>>();

        private readonly Dictionary<Type, Func<object, string>> customPrintForType =
            new Dictionary<Type, Func<object, string>>();

        public MemberPrintingConfig<TOwner, TMemberType> Printing<TMemberType>()
        {
            var memberConfig = new MemberPrintingConfig<TOwner, TMemberType>(this);
            customPrintForType[typeof(TMemberType)] = GetPrintMemberFrom(memberConfig);

            return memberConfig;
        }

        public MemberPrintingConfig<TOwner, TMemberType> Printing<TMemberType>(
            Expression<Func<TOwner, TMemberType>> memberSelector)
        {
            var memberVar = MemberVariable.FromExpression(memberSelector);
            var memberConfig = new MemberPrintingConfig<TOwner, TMemberType>(this);
            customPrintForMember[memberVar] = GetPrintMemberFrom(memberConfig);

            return memberConfig;
        }

        public PrintingConfig<TOwner> Excluding<TMemberType>(Expression<Func<TOwner, TMemberType>> memberSelector)
        {
            excludingMembers.Add(MemberVariable.FromExpression(memberSelector));
            return this;
        }

        public PrintingConfig<TOwner> Excluding<TMemberType>()
        {
            excludingTypes.Add(typeof(TMemberType));
            return this;
        }

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        private string PrintToString(object obj, int nestingLevel)
        {
            if (obj == null)
                return "null" + endOfLine;

            if (simpleTypes.Contains(obj.GetType()))
            {
                var printedValue = obj is IFormattable formattable ?
                    formattable.ToString(null, CultureInfo.InvariantCulture) :
                    obj.ToString();
                return printedValue + endOfLine;
            }

            if (printedObjects.Contains(obj))
                return "This object already printed" + endOfLine;

            if (obj is ICollection collection)
                return PrintCollection(collection, nestingLevel);

            return PrintComplexType(obj, nestingLevel);
        }

        private string PrintComplexType(object obj, int nestingLevel)
        {
            printedObjects.Add(obj);
            var indentation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();

            sb.AppendLine(type.Name);
            foreach (var memberVar in MemberVariable.GetMemberVariables(type))
            {
                if (!MustBePrinted(memberVar))
                    continue;

                var printingFunc = GetPrintingFunc(memberVar, nestingLevel);
                sb.Append(indentation + memberVar.Name + " = " + printingFunc(memberVar.GetValue(obj)));
            }

            if (nestingLevel == 0)
                printedObjects.Clear();

            return sb.ToString();
        }

        private string PrintCollection(ICollection collection, int nestingLevel)
        {
            var sb = new StringBuilder();
            var indentation = new string('\t', nestingLevel + 1);

            sb.AppendLine(collection.GetType().Name);
            int i = 0;
            foreach (var element in collection)
            {
                customPrintForType.TryGetValue(element.GetType(), out var printingFunc);
                printingFunc ??= obj => PrintToString(obj, nestingLevel + 1);
                sb.Append($"{indentation}[{i}] => {printingFunc(element)}");
                i++;
            }

            return sb.ToString();
        }

        private bool MustBePrinted(MemberVariable member)
        {
            return !excludingTypes.Contains(member.MemberVarType) &&
                   !excludingMembers.Contains(member);
        }

        private Func<object, string> GetPrintingFunc(MemberVariable memberVar, int nestingLevel)
        {
            if (customPrintForMember.TryGetValue(memberVar, out var printingFunc) ||
                customPrintForType.TryGetValue(memberVar.MemberVarType, out printingFunc))
                return printingFunc;

            return obj => PrintToString(obj, nestingLevel + 1);
        }

        private Func<object, string> GetPrintMemberFrom<TOwner, TMemberType>(
            MemberPrintingConfig<TOwner, TMemberType> memberConfig)
        {
            return obj =>
                ((IMemberPrintingConfig) memberConfig).PrintMember(obj) + endOfLine;
        }
    }
}