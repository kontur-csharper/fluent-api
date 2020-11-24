using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting.Solved;

namespace ObjectPrinting.Tests
{
    public class ObjectPrinterAcceptanceTests
    {
        private Person person;

        [SetUp]
        public void SetUp()
        {
            person = new Person
            {
                Name = "Alex",
                AgeProperty = 19,
                Relatives = new[]
                {
                    new Person() {Name = "AS"},
                    new Person()
                },
                Persons = new Dictionary<Person, Person>()
                {
                    [new Person()] = new Person(),
                    [new Person()] = new Person()
                },
                Height = 0.1
            };
        }

        [Test]
        public void Excluding_Type_FromSerialization()
        {
            var serialized = person.PrintToString(config => config.Excluding<Guid>());
            serialized.Should().NotContain(nameof(person.Id));
        }

        [Test]
        public void Excluding_Property_FromSerialization()
        {
            var serialized = person.PrintToString(config => config.Excluding(x => x.AgeProperty));
            serialized.Should().NotContain(nameof(person.AgeProperty)).And
                .Contain($"{nameof(person.AgeField)} = {person.AgeField}");
        }

        [Test]
        public void Excluding_Field_FromSerialization()
        {
            var serialized = person.PrintToString(config => config.Excluding(x => x.AgeField));
            serialized.Should().NotContain(nameof(person.AgeField)).And
                .Contain($"{nameof(person.AgeProperty)} = {person.AgeProperty}");
        }

        [Test]
        public void ChangeSerialization_ForType()
        {
            var serialized = person.PrintToString(config => config.Printing<int>().Using(x => x + "q"));
            serialized.Should().Contain($"{nameof(person.AgeProperty)} = {person.AgeProperty}q").And
                .Contain($"{nameof(person.AgeField)} = {person.AgeField}q");
        }

        [Test]
        public void ChangeSerialization_ForProperty()
        {
            var serialized = person.PrintToString(config => config.Printing(x => x.AgeProperty).Using(x => x + "q"));
            serialized.Should().Contain($"{nameof(person.AgeProperty)} = {person.AgeProperty}q").And
                .Contain($"{nameof(person.AgeField)} = {person.AgeField}");
        }

        [Test]
        public void ChangeSerialization_ForField()
        {
            var serialized = person.PrintToString(config => config.Printing(x => x.AgeField).Using(x => x + "q"));
            serialized.Should().Contain($"{nameof(person.AgeProperty)} = {person.AgeProperty}").And
                .Contain($"{nameof(person.AgeField)} = {person.AgeField}q");
        }

        [Test]
        public void SetMaxLength_ForStringField()
        {
            var serialized = person.PrintToString(config => config.Printing(x => x.FamilyName).SetMaxLength(1));
            serialized.Should().Contain($"{nameof(person.FamilyName)} = {person.FamilyName[0]}")
                .And.NotContain($"{nameof(person.FamilyName)} = {person.FamilyName}")
                .And.Contain($"{nameof(person.Name)} = {person.Name}");
        }

        [Test]
        public void SetMaxLength_ForAllStrings()
        {
            var serialized = person.PrintToString(config => config.Printing<string>().SetMaxLength(1));
            serialized.Should().Contain($"{nameof(person.Name)} = {person.Name[0]}")
                .And.NotContain($"{nameof(person.Name)} = {person.Name}")
                .And.Contain($"{nameof(person.FamilyName)} = {person.FamilyName[0]}")
                .And.NotContain($"{nameof(person.FamilyName)} = {person.FamilyName}");
        }

        [Test]
        public void SetMaxLength_ForStringProperty()
        {
            var serialized = person.PrintToString(config => config.Printing(x => x.Name).SetMaxLength(1));
            serialized.Should().Contain($"{nameof(person.Name)} = {person.Name[0]}")
                .And.NotContain($"{nameof(person.Name)} = {person.Name}")
                .And.Contain($"{nameof(person.FamilyName)} = {person.FamilyName}");
        }

        [Test]
        public void CircularReference_ShouldPass()
        {
            var circularReference = new CircularReference();

            var serialized = circularReference.PrintToString();
            serialized.Should()
                .Be(
                    $"{nameof(CircularReference)}{Environment.NewLine}\t{nameof(circularReference.Self)} = {nameof(CircularReference)}");
        }

        [Test]
        public void NoThrow_WhenDeepRecursion()
        {
            var circularReference = new CircularReference();
            var secondCircularReference = new CircularReference();
            circularReference.Self = secondCircularReference;
            secondCircularReference.Self = circularReference;

            Assert.DoesNotThrow(() => circularReference.PrintToString());
        }

        [Test]
        public void SetCulture_ForProperty()
        {
            var englishFormat = person.PrintToString(x =>
                x.Printing(y => y.Height).Using(CultureInfo.CreateSpecificCulture("en")));
            var russianFormat = person.PrintToString(x =>
                x.Printing(y => y.Height).Using(CultureInfo.CreateSpecificCulture("ru")));

            englishFormat.Should()
                .Contain(
                    $"{nameof(person.Height)} = {person.Height.ToString(CultureInfo.CreateSpecificCulture("en"))}");
            russianFormat.Should()
                .Contain(
                    $"{nameof(person.Height)} = {person.Height.ToString(CultureInfo.CreateSpecificCulture("ru"))}");
        }

        [Test]
        public void SetCulture_ForType()
        {
            var englishFormat =
                person.PrintToString(x => x.Printing<double>().Using(CultureInfo.CreateSpecificCulture("en")));
            var russianFormat =
                person.PrintToString(x => x.Printing<double>().Using(CultureInfo.CreateSpecificCulture("ru")));

            englishFormat.Should()
                .Contain($"{nameof(person.Height)} = {person.Height.ToString(CultureInfo.CreateSpecificCulture("en"))}")
                .And.Contain(
                    $"{nameof(person.HeightField)} = {person.HeightField.ToString(CultureInfo.CreateSpecificCulture("en"))}");
            russianFormat.Should()
                .Contain($"{nameof(person.Height)} = {person.Height.ToString(CultureInfo.CreateSpecificCulture("ru"))}")
                .And.Contain(
                    $"{nameof(person.HeightField)} = {person.HeightField.ToString(CultureInfo.CreateSpecificCulture("ru"))}");
        }

        [Test]
        public void SetCulture_ForField()
        {
            var englishFormat = person.PrintToString(x =>
                x.Printing(y => y.HeightField).Using(CultureInfo.CreateSpecificCulture("en")));
            var russianFormat = person.PrintToString(x =>
                x.Printing(y => y.HeightField).Using(CultureInfo.CreateSpecificCulture("ru")));

            englishFormat.Should()
                .Contain(
                    $"{nameof(person.HeightField)} = {person.HeightField.ToString(CultureInfo.CreateSpecificCulture("en"))}");
            russianFormat.Should()
                .Contain(
                    $"{nameof(person.HeightField)} = {person.HeightField.ToString(CultureInfo.CreateSpecificCulture("ru"))}");
        }
    }
}