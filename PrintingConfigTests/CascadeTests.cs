using System;
using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting;
using ObjectPrinting.Configuration;
using PrintingConfigTests.TestingModels;

namespace PrintingConfigTests
{
    [TestFixture]
    public class CascadeTests
    {
        private string result;

        [Test]
        public void Default_CascadeSerialize()
        {
            var subject = CreateCascadeSubject();

            result = ObjectPrinter.For<CascadeModel>()
                .Build()
                .PrintToString(subject);

            result.Should()
                .ContainAll(nameof(CascadeModel),
                    $"{nameof(CascadeModel.Int32)} = {subject.Int32}",
                    $"{nameof(CascadeModel.String)} = {subject.String}",
                    $"{nameof(CascadeModel.Int32)} = {subject.Child.Int32}",
                    $"{nameof(CascadeModel.String)} = {subject.Child.String}",
                    $"{nameof(CascadeModel.Child)} = null");
        }

        [Test]
        public void ParentSettingSpecified_ChildInheritSettings()
        {
            var subject = CreateCascadeSubject();

            result = ObjectPrinter.For<CascadeModel>()
                .Choose(x => x.Int32)
                .Exclude()
                .Build()
                .PrintToString(subject);

            result.Should()
                .Contain($"{nameof(CascadeModel.Int32)} = {subject.Child.Int32}")
                .And
                .NotContain(subject.Int32.ToString());
        }

        [Test]
        public void ChildSettingSpecified_ShouldNotAffectParent()
        {
            var subject = CreateCascadeSubject();

            result = ObjectPrinter.For<CascadeModel>()
                .Choose(x => x.Child.Int32)
                .Exclude()
                .Build()
                .PrintToString(subject);

            result.Should()
                .Contain($"{nameof(CascadeModel.Int32)} = {subject.Int32}");
        }

        [Test]
        public void CyclicReference_IgnoreAlreadyPrinted()
        {
            var subject = new CascadeModel {Int32 = 1, String = "a"};
            subject.Child = subject;

            Action test = () => ObjectPrinter.For<CascadeModel>()
                .Build()
                .PrintToString(subject);

            test.Should()
                .Throw<InvalidOperationException>()
                .WithMessage($"Cyclic reference in {typeof(CascadeModel)}");
        }

        [Test]
        public void SameLinkInDifferentFields_NotThrow()
        {
            var subject = new SeveralNestedContainingModel
            {
                String = "A1",
                M1 = new SeveralNestedContainingModel {String = "A2"}
            };
            subject.M2 = subject.M1;

            Action test = () => ObjectPrinter.For<SeveralNestedContainingModel>()
                .Build()
                .PrintToString(subject);

            test.Should().NotThrow();
        }

        [TearDown]
        public void TearDown()
        {
            TestContext.Out.WriteLine(result);
        }

        private static CascadeModel CreateCascadeSubject() => new CascadeModel
        {
            Int32 = 1,
            String = "a",
            Child = new CascadeModel
            {
                Int32 = 2,
                String = "b"
            }
        };
    }
}