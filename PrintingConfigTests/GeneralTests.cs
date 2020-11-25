using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting;
using ObjectPrinting.Configuration;
using PrintingConfigTests.TestingModels;

namespace PrintingConfigTests
{
    [TestFixture]
    public class GeneralTests
    {
        private string result;

        [Test]
        public void CanPrintProperties()
        {
            var subject = new TestingPropertiesClass
            {
                Int = 18,
                Double = 123.456,
                Guid = Guid.NewGuid(),
                String = "Abc xyz"
            };

            result = ObjectPrinter.For<TestingPropertiesClass>().PrintToString(subject);

            result.Should()
                .ContainAll(
                    nameof(TestingPropertiesClass),
                    $"{nameof(TestingPropertiesClass.Guid)} = {subject.Guid}",
                    $"{nameof(TestingPropertiesClass.String)} = {subject.String}",
                    $"{nameof(TestingPropertiesClass.Double)} = {subject.Double}",
                    $"{nameof(TestingPropertiesClass.Int)} = {subject.Int}");
        }

        [Test]
        public void CanPrintFields()
        {
            var subject = new TestingFieldsClass
            {
                Int32 = 18,
                Double = 123.456,
                Guid = Guid.NewGuid(),
                String = "Abc xyz",
                Bool = true
            };

            result = ObjectPrinter.For<TestingFieldsClass>().PrintToString(subject);

            result.Should()
                .ContainAll(
                    nameof(TestingFieldsClass),
                    $"{nameof(TestingFieldsClass.Guid)} = {subject.Guid}",
                    $"{nameof(TestingFieldsClass.String)} = {subject.String}",
                    $"{nameof(TestingFieldsClass.Double)} = {subject.Double}",
                    $"{nameof(TestingFieldsClass.Bool)} = {subject.Bool}",
                    $"{nameof(TestingFieldsClass.Int32)} = {subject.Int32}");
        }

        [Test]
        public void PropertyGroupExcluded_DoNotPrint()
        {
            var subject = new TestingPropertiesClass
            {
                Int = int.MaxValue,
                Double = 123.456,
                Guid = Guid.NewGuid(),
                String = "Abc xyz"
            };

            result = ObjectPrinter.For<TestingPropertiesClass>()
                .Choose<int>()
                .Exclude()
                .PrintToString(subject);

            result.Should()
                .NotContainAny($"{nameof(TestingPropertiesClass.Int)}", $"{subject.Int}");
        }

        [Test]
        public void PropertyExcluded_DoNotPrint()
        {
            var subject = new TestingPropertiesClass
            {
                Int = 18,
                Double = 123.456,
                Guid = Guid.NewGuid(),
                String = "Abc xyz"
            };

            result = ObjectPrinter.For<TestingPropertiesClass>()
                .Choose(p => p.Guid)
                .Exclude()
                .PrintToString(subject);

            result.Should()
                .NotContainAny($"{nameof(TestingPropertiesClass.Guid)}", $"{subject.Guid}");
        }

        [Test]
        public void CultureSpecified_PrintUsingIt()
        {
            var subject = new TestingPropertiesClass
            {
                Int = 18,
                Double = 123.456,
                Guid = Guid.NewGuid(),
                String = "Abc xyz"
            };

            var selectedCulture = CultureInfo.InvariantCulture;
            result = ObjectPrinter.For<TestingPropertiesClass>()
                .Choose(p => p.Double)
                .SetCulture(selectedCulture)
                .PrintToString(subject);

            result.Should()
                .Contain($"{nameof(TestingPropertiesClass.Double)} = {subject.Double.ToString(null, selectedCulture)}");
        }

        [Test]
        public void FormatAndCultureSpecified_PrintUsingIt()
        {
            var subject = new TestingPropertiesClass
            {
                Int = 18,
                Double = 123.456,
                Guid = Guid.NewGuid(),
                String = "Abc xyz"
            };

            const string format = "e2";
            var selectedCulture = CultureInfo.InvariantCulture;
            result = ObjectPrinter.For<TestingPropertiesClass>()
                .Choose(p => p.Double)
                .SetCulture(format, selectedCulture)
                .PrintToString(subject);

            result.Should()
                .Contain(
                    $"{nameof(TestingPropertiesClass.Double)} = {subject.Double.ToString(format, selectedCulture)}");
        }

        [Test]
        public void StringPropertyLengthLimited_ValueLongerThanMax_TrimToMaxLength()
        {
            const int maxLength = 10;
            var subject = new TestingPropertiesClass
            {
                Int = 18,
                Double = 123.456,
                Guid = Guid.NewGuid(),
                String = new string('.', maxLength) + "!"
            };

            result = ObjectPrinter.For<TestingPropertiesClass>()
                .Choose(p => p.String)
                .Trim(maxLength)
                .PrintToString(subject);

            result.Should()
                .Contain($"{nameof(TestingPropertiesClass.String)} = {new string('.', maxLength)}")
                .And
                .NotContain(subject.String);
        }

        [Test]
        public void StringPropertyLengthLimited_ValueLessThanMax_PrintFull()
        {
            const int maxLength = 10;
            var subject = new TestingPropertiesClass
            {
                Int = 18,
                Double = 123.456,
                Guid = Guid.NewGuid(),
                String = new string('.', maxLength - 1)
            };

            result = ObjectPrinter.For<TestingPropertiesClass>()
                .Choose(p => p.String)
                .Trim(maxLength)
                .PrintToString(subject);

            result.Should()
                .Contain($"{nameof(TestingPropertiesClass.String)} = {subject.String}");
        }

        [Test]
        public void SerializingDelegateSpecified_PrintUsingIt()
        {
            var subject = new TestingPropertiesClass
            {
                Int = 18,
                Double = 123.456,
                Guid = Guid.NewGuid(),
                String = "Abc xyz"
            };

            result = ObjectPrinter.For<TestingPropertiesClass>()
                .Choose(p => p.Double)
                .UseSerializer(_ => "C2H5OH")
                .PrintToString(subject);

            result.Should()
                .Contain($"{nameof(TestingPropertiesClass.Double)} = C2H5OH");
        }

        [Test]
        public void ConflictingSettings_ChooseDirectlySpecified()
        {
            var subject = new TestingPropertiesClass
            {
                Int = 18,
                Double = 123.456,
                Guid = Guid.NewGuid(),
                String = "Abc xyz"
            };

            result = ObjectPrinter.For<TestingPropertiesClass>()
                .Choose(p => p.Double)
                .UseSerializer(_ => "direct")
                .Choose<double>()
                .UseSerializer(_ => "group")
                .PrintToString(subject);

            result.Should()
                .Contain($"{nameof(TestingPropertiesClass.Double)} = direct");
        }

        [Test]
        public void NullProperty_PrintNull()
        {
            var subject = new TestingPropertiesClass
            {
                String = null
            };

            result = ObjectPrinter.For<TestingPropertiesClass>().PrintToString(subject);

            result.Should()
                .Contain($"{nameof(TestingPropertiesClass.String)} = null");
        }

        [TearDown]
        public void TearDown()
        {
            TestContext.Out.WriteLine(result);
        }
    }
}