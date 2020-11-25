using System;
using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting;
using ObjectPrinting.Configuration;
using PrintingConfigTests.TestingModels;

namespace PrintingConfigTests
{
    [TestFixture]
    public class NestingTests
    {
        private string result;

        [Test]
        public void Default_SerializeNestedProperties()
        {
            var subject = CreateNestingSubject();

            result = ObjectPrinter.For<NestedContainingModel>()
                .Build()
                .PrintToString(subject);

            result.Should()
                .ContainAll(nameof(TestingPropertiesClass),
                    $"{nameof(NestedContainingModel.Int)} = {subject.Int}",
                    $"{nameof(NestedContainingModel.String)} = {subject.String}",
                    $"{nameof(NestedContainingModel.Nested)} = {nameof(TestingPropertiesClass)}",
                    $"{nameof(TestingPropertiesClass.Double)} = {subject.Nested.Double}",
                    $"{nameof(TestingPropertiesClass.Int)} = {subject.Nested.Int}",
                    $"{nameof(TestingPropertiesClass.Guid)} = {subject.Nested.Guid}",
                    $"{nameof(TestingPropertiesClass.String)} = {subject.Nested.String}");
        }

        [Test]
        public void ChildPropertySerializerSpecified_UseIt()
        {
            var subject = CreateNestingSubject();

            result = ObjectPrinter.For<NestedContainingModel>()
                .Choose(x => x.Nested.Guid)
                .UseSerializer(g => $"ggg {g.ToString()}")
                .Build()
                .PrintToString(subject);

            result.Should()
                .Contain($"{nameof(TestingPropertiesClass.Guid)} = ggg {subject.Nested.Guid}");
        }

        [Test]
        public void PropertyGroupSerializerSpecified_ApplyToAllLevels()
        {
            var subject = CreateNestingSubject();

            result = ObjectPrinter.For<NestedContainingModel>()
                .Choose<int>()
                .Exclude()
                .Build()
                .PrintToString(subject);

            result.Should().NotContainAny(
                nameof(TestingPropertiesClass.Int),
                nameof(NestedContainingModel.Int),
                subject.Int.ToString(),
                subject.Nested.Int.ToString());
        }

        [Test]
        public void NestedPropertySerializerSpecified_DoesntAffectedParent()
        {
            var subject = CreateNestingSubject();

            result = ObjectPrinter.For<NestedContainingModel>()
                .Choose(x => x.Nested.Int)
                .UseSerializer(_ => "uip")
                .Build()
                .PrintToString(subject);

            result.Should()
                .Contain($"{nameof(NestedContainingModel.Int)} = {subject.Int}");
        }

        [Test]
        public void NestedObjectSerializerSpecified_UseItInsteadState()
        {
            var subject = CreateNestingSubject();

            result = ObjectPrinter.For<NestedContainingModel>()
                .Choose(x => x.Nested)
                .UseSerializer(n => "q1w2e3")
                .Build()
                .PrintToString(subject);

            result.Should()
                .Contain($"{nameof(NestedContainingModel.Nested)} = q1w2e3")
                .And
                .NotContainAny(subject.Nested.String, 
                    subject.Nested.Double.ToString(), 
                    subject.Nested.Guid.ToString(),
                    subject.Nested.Int.ToString());
        }

        [TearDown]
        public void TearDown()
        {
            TestContext.Out.WriteLine(result);
        }

        private static NestedContainingModel CreateNestingSubject() => new NestedContainingModel
        {
            Int = 123,
            String = "abc",
            Nested = new TestingPropertiesClass
            {
                Double = 45.67,
                Int = 890,
                Guid = Guid.NewGuid(),
                String = "xyz"
            }
        };
    }
}