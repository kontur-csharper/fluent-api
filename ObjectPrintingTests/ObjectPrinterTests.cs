using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting;

namespace ObjectPrintingTests
{
    [TestFixture]
    public class ObjectPrinterTests
    {
        private static readonly Person Person = new Person
        {
            Age = 19, Height = 175.5, Id = Guid.NewGuid(), Name = "Alex", Wallet = new[] {"$: 50", "P: 40000"},
            RelativesNames = new Dictionary<string, string> {{"Mother", "Lana"}, {"Father", "John"}}
        };

        [Test]
        public void PrintToString_DoNotPrintProperties_AfterExcludingType()
        {
            var objInString = Printer.PrintToString(Person,
                x => x
                    .Excluding<string>());
            objInString.Should().NotContain($"{nameof(Person.Name)} = {Person.Name}");
        }

        [Test]
        public void PrintToString_PrintByAnotherMethod_AfterUsing()
        {
            var objInString = Printer.PrintToString(Person,
                x => x
                    .Printing<int>().Using(p => p + " y.o."));
            objInString.Should().Contain($"{nameof(Person.Age)} = {Person.Age} y.o.");
        }

        [Test]
        public void PrintToString_Print_WithConfigCulture()
        {
            var objInString = Printer.PrintToString(Person,
                x => x
                    .Printing<double>().Using(CultureInfo.GetCultureInfo("en-US")));
            objInString.Should().Contain($"{nameof(Person.Height)} = 175.5");
        }

        [Test]
        public void PrintToString_PrintPropByAnotherMethod_AfterPrinting()
        {
            const int maxLen = 2;
            var objInString = Printer.PrintToString(Person,
                x => x
                    .Printing(p => p.Name).TrimmedToLength(maxLen));
            objInString.Should().NotContain($"{nameof(Person.Name)} = {Person.Name}");
            objInString.Should().Contain($"{nameof(Person.Name)} = {Person.Name.Substring(0, maxLen)}");
        }

        [Test]
        public void PrintToString_DoNotPrintProperties_AfterExcludingProps()
        {
            var objInString = Printer.PrintToString(Person,
                x => x
                    .Excluding(p => p.Age));
            objInString.Should().NotContain($"{nameof(Person.Age)} = {Person.Age}");
        }

        [Test]
        public void PrintToString_PrintCollections_WhenInProperties()
        {
            var objInString = Person.PrintToString();
            objInString.Should().ContainAll($"{nameof(Person.Wallet)} = ", "$: 50", "P: 40000");
            objInString.Should().ContainAll($"{nameof(Person.RelativesNames)} = ",
                "Key = Mother",
                "Value = Lana",
                "Key = Father",
                "Value = John");
        }
    }
}