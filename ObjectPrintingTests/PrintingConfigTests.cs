using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using FluentAssertions;
using ObjectPrinting;

namespace ObjectPrintingTests
{
    [TestFixture]
    public class PrintingConfigTests
    {
        [Test]
        public void PrintToString_FieldsArePrinted()
        {
            var onlyFields = new OnlyFieldClass {DoubleField = 10.1};
            var expectedValue = new PrintedObjectBuilder(nameof(OnlyFieldClass))
                .AddSimpleMember(
                    nameof(onlyFields.DoubleField),
                    onlyFields.DoubleField.ToString(CultureInfo.InvariantCulture))
                .Build();

            var printer = ObjectPrinter.For<OnlyFieldClass>();
            printer.PrintToString(onlyFields).Should().Be(expectedValue);
        }

        [Test]
        public void PrintToString_PropertiesArePrinted()
        {
            var onlyProperty = new OnlyPropertyClass {StrProperty = "str"};
            var expectedValue = new PrintedObjectBuilder(nameof(OnlyPropertyClass))
                .AddSimpleMember(nameof(onlyProperty.StrProperty), onlyProperty.StrProperty)
                .Build();

            var printer = ObjectPrinter.For<OnlyPropertyClass>();
            printer.PrintToString(onlyProperty).Should().Be(expectedValue);
        }

        [Test]
        public void PrintToString_IfReferenceTypeMemberIsNull_PrintedValueIsNullString()
        {
            var onlyReference = new OnlyReferenceTypeClass();
            var expectedValue = new PrintedObjectBuilder(nameof(OnlyReferenceTypeClass))
                .AddNullMember(nameof(onlyReference.Reference))
                .Build();

            var printer = ObjectPrinter.For<OnlyReferenceTypeClass>();
            printer.PrintToString(onlyReference).Should().Be(expectedValue);
        }

        [Test]
        public void PrintToString_AlsoPrintInnerObjects()
        {
            var simpleClass = new SimpleClass
            {
                DoubleField = 2.5, DoubleProperty = 1.5,
                ReferenceField = new OnlyPropertyClass {StrProperty = "str"}
            };
            var expectedValue = new PrintedObjectBuilder(nameof(SimpleClass))
                .AddComplexMember(nameof(simpleClass.ReferenceField), nameof(OnlyPropertyClass),
                    complexField => complexField.AddSimpleMember(
                        nameof(OnlyPropertyClass.StrProperty), simpleClass.ReferenceField.StrProperty))
                .AddSimpleMember(
                    nameof(simpleClass.DoubleField),
                    simpleClass.DoubleField.ToString(CultureInfo.InvariantCulture))
                .AddSimpleMember(
                    nameof(simpleClass.DoubleProperty),
                    simpleClass.DoubleProperty.ToString(CultureInfo.InvariantCulture))
                .Build();

            var printer = ObjectPrinter.For<SimpleClass>();
            printer.PrintToString(simpleClass).Should().Be(expectedValue);
        }

        [Test]
        public void PrintToString_IfObjectsHaveCircularReference_PrintMessageAboutItAsValue()
        {
            var firstObj = new OnlyReferenceTypeClass();
            var secondObj = new OnlyReferenceTypeClass {Reference = firstObj};
            firstObj.Reference = secondObj;
            var expectedValue = new PrintedObjectBuilder(nameof(OnlyReferenceTypeClass))
                .AddComplexMember(nameof(firstObj.Reference), nameof(OnlyReferenceTypeClass),
                    complexMember => complexMember.AddSimpleMember(
                        nameof(secondObj.Reference), "This object already printed"))
                .Build();

            var printer = ObjectPrinter.For<OnlyReferenceTypeClass>();
            printer.PrintToString(firstObj).Should().Be(expectedValue);
        }

        [TestCaseSource(nameof(PrintToString_WorkWithCollectionsCases))]
        public void PrintToString_CorrectWorkWithCollections(
            ClassWithCollections obj, string expectedValue)
        {
            ObjectPrinter.For<ClassWithCollections>().PrintToString(obj).Should()
                .Be(expectedValue);
        }

        public static IEnumerable PrintToString_WorkWithCollectionsCases
        {
            get
            {
                yield return new TestCaseData(
                        new ClassWithCollections {Dict = new Dictionary<string, int> {{"key1", 10}}},
                        new PrintedObjectBuilder(nameof(ClassWithCollections))
                            .AddNullMember(nameof(ClassWithCollections.List))
                            .AddNullMember(nameof(ClassWithCollections.Array))
                            .AddCollectionMember(
                                nameof(ClassWithCollections.Dict),
                                "Dictionary`2",
                                collection => collection
                                    .AddComplexElement(
                                        "KeyValuePair`2",
                                        element => element
                                            .AddSimpleMember("Key", "key1")
                                            .AddSimpleMember("Value", "10")))
                            .Build())
                    .SetName("WithDict");

                yield return new TestCaseData(
                        new ClassWithCollections {List = new List<string> {"str", "rts"}},
                        new PrintedObjectBuilder(nameof(ClassWithCollections))
                            .AddCollectionMember(
                                nameof(ClassWithCollections.List),
                                $"{nameof(List<string>)}`1",
                                config =>
                                    config.AddSimpleElement("str").AddSimpleElement("rts"))
                            .AddNullMember(nameof(ClassWithCollections.Array))
                            .AddNullMember(nameof(ClassWithCollections.Dict))
                            .Build())
                    .SetName("WithList");

                yield return new TestCaseData(
                        new ClassWithCollections {Array = new[] {2.5}},
                        new PrintedObjectBuilder(nameof(ClassWithCollections))
                            .AddNullMember(nameof(ClassWithCollections.List))
                            .AddCollectionMember(
                                nameof(ClassWithCollections.Array),
                                "Double[]",
                                config =>
                                    config.AddSimpleElement(2.5.ToString(CultureInfo.InvariantCulture)))
                            .AddNullMember(nameof(ClassWithCollections.Dict))
                            .Build())
                    .SetName("WithArray");
            }
        }

        [Test]
        public void Excluding_IfTypeIsSpecified_ExcludeAllMembersOfThisType()
        {
            var simpleClass = new SimpleClass();
            var expectedValue = new PrintedObjectBuilder(nameof(SimpleClass))
                .AddNullMember(nameof(simpleClass.ReferenceField))
                .Build();

            var printer = ObjectPrinter.For<SimpleClass>().Excluding<double>();
            printer.PrintToString(simpleClass).Should().Be(expectedValue);
        }

        [Test]
        public void
            Excluding_IfConcreteMemberSpecified_ExcludeOnlyThisMember()
        {
            var simpleClass = new SimpleClass {DoubleField = 1.5, DoubleProperty = 2.5};
            var expectedValue = new PrintedObjectBuilder(nameof(SimpleClass))
                .AddSimpleMember(
                    nameof(simpleClass.DoubleField),
                    simpleClass.DoubleField.ToString(CultureInfo.InvariantCulture))
                .AddSimpleMember(
                    nameof(simpleClass.DoubleProperty),
                    simpleClass.DoubleProperty.ToString(CultureInfo.InvariantCulture))
                .Build();

            var printer = ObjectPrinter.For<SimpleClass>()
                .Excluding(x => x.ReferenceField);
            printer.PrintToString(simpleClass).Should().Be(expectedValue);
        }

        [Test]
        public void Printing_IfConfigureType_AllInstanceOfTypeArePrintedWithThisConfig()
        {
            var simpleClass = new SimpleClass {DoubleField = 1.5, DoubleProperty = 2.5};
            var expectedValue = new PrintedObjectBuilder(nameof(SimpleClass))
                .AddNullMember(nameof(simpleClass.ReferenceField))
                .AddSimpleMember(nameof(simpleClass.DoubleField), $"It's double {simpleClass.DoubleField}")
                .AddSimpleMember(nameof(simpleClass.DoubleProperty), $"It's double {simpleClass.DoubleProperty}")
                .Build();

            var printer = ObjectPrinter.For<SimpleClass>()
                .Printing<double>()
                .Using(d => $"It's double {d}");
            printer.PrintToString(simpleClass).Should().Be(expectedValue);
        }

        [Test]
        public void Printing_IfConfigConcreteMember_PrintWithThisConfigOnlyThisMember()
        {
            var simpleClass = new SimpleClass {DoubleField = 1.5, DoubleProperty = 2.5};
            var expectedValue = new PrintedObjectBuilder(nameof(SimpleClass))
                .AddNullMember(nameof(simpleClass.ReferenceField))
                .AddSimpleMember(nameof(simpleClass.DoubleField), $"With member config {simpleClass.DoubleField}")
                .AddSimpleMember(
                    nameof(simpleClass.DoubleProperty),
                    simpleClass.DoubleProperty.ToString(CultureInfo.InvariantCulture))
                .Build();

            var printer = ObjectPrinter.For<SimpleClass>()
                .Printing(x => x.DoubleField)
                .Using(doubleField => $"With member config {doubleField}");
            printer.PrintToString(simpleClass).Should().Be(expectedValue);
        }

        [Test]
        public void Using_IfCultureInfoGiven_UseItForThisMemberPrintingConfigOfIFormattableType()
        {
            var onlyFieldObj = new OnlyFieldClass {DoubleField = 2.5};
            var expectedValue = new PrintedObjectBuilder(nameof(OnlyFieldClass))
                .AddSimpleMember(nameof(onlyFieldObj.DoubleField), onlyFieldObj.DoubleField.ToString())
                .Build();

            var printer = ObjectPrinter.For<OnlyFieldClass>()
                .Printing<double>()
                .Using(CultureInfo.CurrentCulture);
            printer.PrintToString(onlyFieldObj).Should().Be(expectedValue);
        }

        [Test]
        public void TrimmedToLength_IfLengthOfStringGreaterOrEqualThanMaxLength_TrimIt()
        {
            var onlyFieldObj = new OnlyPropertyClass {StrProperty = "string"};
            var expectedValue = new PrintedObjectBuilder(nameof(OnlyPropertyClass))
                .AddSimpleMember(nameof(onlyFieldObj.StrProperty), "str")
                .Build();

            var printer = ObjectPrinter.For<OnlyPropertyClass>()
                .Printing<string>()
                .TrimmedToLength(3);
            printer.PrintToString(onlyFieldObj).Should().Be(expectedValue);
        }

        [Test]
        public void TrimmedToLength_IfLengthOfStringLessThanMaxLength_PrintAllString()
        {
            var onlyFieldObj = new OnlyPropertyClass {StrProperty = "string"};
            var expectedValue = new PrintedObjectBuilder(nameof(OnlyPropertyClass))
                .AddSimpleMember(nameof(onlyFieldObj.StrProperty), "string")
                .Build();

            var printer = ObjectPrinter.For<OnlyPropertyClass>()
                .Printing<string>()
                .TrimmedToLength(100);
            printer.PrintToString(onlyFieldObj).Should().Be(expectedValue);
        }
    }
}