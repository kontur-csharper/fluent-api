using System;
using System.Text;

namespace ObjectPrintingTests
{
    public class PrintedObjectBuilder
    {
        private StringBuilder stringBuilder = new StringBuilder();
        private readonly int nestingLevel;
        private string Indentation => new string('\t', nestingLevel);

        public PrintedObjectBuilder(string className)
        {
            stringBuilder.AppendLine(className);
        }

        public PrintedObjectBuilder(StringBuilder stringBuilder, int nestingLevel)
        {
            this.stringBuilder = stringBuilder;
            this.nestingLevel = nestingLevel;
        }

        public PrintedObjectBuilder AddSimpleMember(string name, string value)
        {
            stringBuilder.AppendLine(Indentation + $"\t{name} = {value}");
            return this;
        }

        public PrintedObjectBuilder AddNullMember(string name)
        {
            return AddSimpleMember(name, "null");
        }

        public PrintedObjectBuilder AddComplexMember(string memberName, string typeName,
            Func<PrintedObjectBuilder, PrintedObjectBuilder> configComplexMember)
        {
            AddSimpleMember(memberName, typeName);
            var complexMemberBuilder = configComplexMember(
                new PrintedObjectBuilder(stringBuilder, nestingLevel + 1));
            return this;
        }

        public PrintedObjectBuilder AddCollectionMember(string memberName, string typeName,
            Func<PrintedCollectionBuilder, PrintedCollectionBuilder> collectionConfig)
        {
            AddSimpleMember(memberName, typeName);
            var collectionBuilder = collectionConfig(
                new PrintedCollectionBuilder(stringBuilder, nestingLevel + 1));
            return this;
        }

        public string Build()
        {
            return stringBuilder.ToString();
        }
    }
}