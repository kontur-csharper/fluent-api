using System;
using System.Globalization;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using NUnit.Framework;
using ObjectPrinting.Extensions;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    [UseReporter(typeof(DiffReporter), typeof(FileLauncherReporter))]
    [UseApprovalSubdirectory("Approval")]
    public class PrintingConfig_Should
    {
        [Test]
        public void PrintToString_Null_ReturnsNull() =>
            Approvals.Verify(ObjectPrinter.For<string>().PrintToString(null));

        [Test]
        public void PrintToString_PersonWithoutExcludingAnything_ContainsAllPublicProperties() =>
            Approvals.Verify(new Person().PrintToString());

        [Test]
        public void PrintToString_PersonExcludingGuidValues_NotContainsGuid() =>
            VerifyWithCustomConfig(config => config.Excluding<Guid>());

        [Test]
        public void PrintToString_PersonExcludingProperty_NotContainsProperty() =>
            VerifyWithCustomConfig(config => config.Excluding(p => p.Spouse));

        [Test]
        public void PrintToString_PersonExcludingField_NotContainsProperty() =>
            VerifyWithCustomConfig(config => config.Excluding(p => p.Id));

        [Test]
        public void PrintToString_PersonHasAlternativePrintForDouble_ContainsAlternativeStringInsteadDouble() =>
            VerifyWithCustomConfig(config => config.Printing<double>().Using(d => "this is double"));

        [Test]
        public void PrintToString_PersonHasAlternativePrintForHeightProperty_ContainsAlternativeStringForHeight() =>
            VerifyWithCustomConfig(config => config.Printing(person => person.Height).Using(d => "this is height"));

        [Test]
        public void PrintToString_PersonHasAlternativePrintForIdField_ContainsAlternativeStringForHeight() =>
            VerifyWithCustomConfig(config => config.Printing(person => person.Id).Using(d => "this is personal ID"));

        [Test]
        public void PrintToString_PersonHasInvariantCultureForDateTime_ContainsInvariantDateTime() =>
            VerifyWithCustomConfig(config => config.Printing<DateTime>().Using(CultureInfo.InvariantCulture));

        [Test]
        public void PrintToString_PersonHasCurrentCultureForDouble_ContainsCorrespondingDouble() =>
            Approvals.Verify(new Person {Height = 1.78}.PrintToString(
                config => config.Printing<double>().Using(CultureInfo.CurrentCulture))
            );

        [Test]
        public void PrintToString_TrimmedPersonStrings_ContainsOnlyTrimmedStrings() =>
            Approvals.Verify(new Person {Name = "Humpty", Surname = "Dumpty"}.PrintToString(
                config => config.Printing<string>().TrimmedToLength(3)
            ));

        [Test]
        public void PrintToString_IEnumerable_ReturnsCorrectly() =>
            Approvals.Verify(Enumerable.Range(0, 10).PrintToString());

        [Test]
        public void PrintToString_Array_ReturnsCorrectly() =>
            Approvals.Verify(Enumerable.Range(0, 10).ToArray().PrintToString());

        [Test]
        public void PrintToString_List_ReturnsCorrectly() =>
            Approvals.Verify(Enumerable.Range(0, 10).ToList().PrintToString());

        [Test]
        public void PrintToString_Dictionary_ReturnsCorrectly()
        {
            var count = 0;
            var dictionary = Enumerable.Range(0, 3)
                .Select(_ => new Person())
                .ToDictionary(person => count++, person => person);
            Approvals.Verify(dictionary.PrintToString());
        }

        [Test]
        public void PrintToString_Cycle_ReturnsCorrectResult() => Approvals.Verify(long.MaxValue.PrintToString());

        private static void VerifyWithCustomConfig(Func<PrintingConfig<Person>, PrintingConfig<Person>> conf) =>
            Approvals.Verify(new Person().PrintToString(conf));
    }
}