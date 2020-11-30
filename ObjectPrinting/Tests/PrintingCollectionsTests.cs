using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    public class PrintingCollectionsTests
    {
        [Test]
        public void PrintArray()
        {
            var intArrayPrinter = ObjectPrinter.For<int[]>();
            intArrayPrinter.PrintToString(new[] {1, 2, 3, 4, 5}).Should()
                .Contain("0: 1")
                .And.Contain("1: 2")
                .And.Contain("2: 3")
                .And.Contain("3: 4")
                .And.Contain("4: 5");
        }

        [Test]
        public void PrintDictionary()
        {
            var dictionaryPrinter = ObjectPrinter.For<Dictionary<string, int>>();
            var dict = new Dictionary<string, int>() {{"sugar", 3}, {"milk", 2}, {"potato", 1}};
            dictionaryPrinter.PrintToString(dict).Should()
                .Contain("0: ")
                .And.Contain("1: ")
                .And.Contain("Key = sugar")
                .And.Contain("Value = 3")
                .And.Contain("Key = milk")
                .And.Contain("Value = 2")
                .And.Contain("Key = potato")
                .And.Contain("Value = 1");

        }
    }
}