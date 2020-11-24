using System;
using System.Text;

namespace ObjectPrintingTests
{
    public class PrintedCollectionBuilder
    {
        private StringBuilder stringBuilder = new StringBuilder();
        private readonly int nestingLevel;
        private int counter = -1;

        private string Indentation => new string('\t', nestingLevel);
        
        public PrintedCollectionBuilder(StringBuilder stringBuilder, int nestingLevel)
        {
            this.stringBuilder = stringBuilder;
            this.nestingLevel = nestingLevel;
        }

        public PrintedCollectionBuilder AddSimpleElement(string value)
        {
            stringBuilder.AppendLine($"{Indentation}\t[{++counter}] => {value}");
            return this;
        }

        public PrintedCollectionBuilder AddComplexElement(string typeName,
            Func<PrintedObjectBuilder, PrintedObjectBuilder> configComplexElement)
        {
            AddSimpleElement(typeName);
            var complexElementBuilder = configComplexElement(
                new PrintedObjectBuilder(stringBuilder, nestingLevel + 1));
            return this;
        }

        public string Build()
        {
            return stringBuilder.ToString();
        }
    }
}