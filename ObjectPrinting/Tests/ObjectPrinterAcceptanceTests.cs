using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        private readonly string nl = Environment.NewLine;

        [Test]
        public void Demo()
        {
            var person = GetPerson();

            var printer = ObjectPrinter<Person>.Should()
                .Exclude(x => x.Parent)
                .ForMember(x => x.Name)
                .SetSerializer(x => x + "...")
                .Build();

            //1. Исключить из сериализации свойства определенного типа
            //2. Указать альтернативный способ сериализации для определенного типа
            //3. Для числовых типов указать культуру
            //4. Настроить сериализацию конкретного свойства
            //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
            //6. Исключить из сериализации конкретного свойства

            string result = printer.PrintToString(person);

            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию        
            //8. ...с конфигурированием
        }

        [Test]
        public void PrintingConfig_SupportsExcludingTypes()
        {
            var cat = new Cat("Boris", 5.123, 10);
            var result = ObjectPrinter<Cat>.Should()
                .Exclude<double>()
                .Build()
                .PrintToString(cat);

            result.Should().Be($"Cat{nl}\tSomeField = SomeValue{nl}\tName = Boris{nl}\tWhiskersCount = 10");
        }

        [Test]
        public void PrintingConfig_SupportsDoubleExcludingTypes()
        {
            var cat = new Cat("Boris", 5.123, 10);
            var result = ObjectPrinter<Cat>.Should()
                .Exclude<double>()
                .Exclude<int>()
                .Build()
                .PrintToString(cat);

            result.Should().Be($"Cat{nl}\tSomeField = SomeValue{nl}\tName = Boris");
        }

        [Test]
        public void PrintingConfig_SupportsExcludingProperties()
        {
            var cat = GetCat();
            var result = ObjectPrinter<Cat>.Should()
                .Exclude(x => x.WhiskersCount)
                .Build()
                .PrintToString(cat);

            result.Should().Be($"Cat{nl}\tSomeField = SomeValue{nl}\tName = Boris{nl}\tWeight = 5,123");
        }

        [Test]
        public void PrintingConfig_SupportsDoubleExcludingProperties()
        {
            var cat = GetCat();
            var result = ObjectPrinter<Cat>.Should()
                .Exclude(x => x.WhiskersCount)
                .Exclude(x => x.Weight)
                .Build()
                .PrintToString(cat);

            result.Should().Be($"Cat{nl}\tSomeField = SomeValue{nl}\tName = Boris");
        }

        [Test]
        public void PrintingConfig_SupportsExcludingFields()
        {
            var cat = GetCat();
            var result = ObjectPrinter<Cat>.Should()
                .Exclude(x => x.SomeField)
                .Build()
                .PrintToString(cat);

            result.Should().Be($"Cat{nl}\tName = Boris{nl}\tWeight = 5,123{nl}\tWhiskersCount = 10");
        }

        [Test]
        public void PrintingConfig_SupportsMultiExcluding()
        {
            var cat = GetCat();
            var result = ObjectPrinter<Cat>.Should()
                .Exclude<int>()
                .Exclude(x => x.Weight)
                .Build()
                .PrintToString(cat);

            result.Should().Be($"Cat{nl}\tSomeField = SomeValue{nl}\tName = Boris");
        }

        [Test]
        public void PrintingConfig_PrintsOnlyType_WhenAllMembersAreExcluded()
        {
            var cat = GetCat();
            var result = ObjectPrinter<Cat>.Should()
                .Exclude<int>()
                .Exclude<string>()
                .Exclude(x => x.Weight)
                .Build()
                .PrintToString(cat);

            result.Should().Be($"Cat");
        }

        [Test]
        public void PrintingConfig_SupportsSettingSerializerForTypes()
        {
            var cat = GetCat();
            var result = ObjectPrinter<Cat>.Should()
                .SetSerializerFor<double>(str => $"{str}kg")
                .Build()
                .PrintToString(cat);

            result.Should().Be($"Cat{nl}\tSomeField = SomeValue{nl}\tName = Boris" +
                               $"{nl}\tWeight = 5,123kg{nl}\tWhiskersCount = 10");
        }

        [Test]
        public void PrintingConfig_SupportsSettingSerializerForProperties()
        {
            var cat = GetCat();
            var result = ObjectPrinter<Cat>.Should()
                .ForMember(x => x.WhiskersCount)
                .SetSerializer(str => $"{str} whiskers")
                .Build()
                .PrintToString(cat);

            result.Should().Be($"Cat{nl}\tSomeField = SomeValue{nl}\tName = Boris" +
                               $"{nl}\tWeight = 5,123{nl}\tWhiskersCount = 10 whiskers");
        }

        [Test]
        public void PrintingConfig_SupportsSettingSerializerForFields()
        {
            var cat = GetCat();
            var result = ObjectPrinter<Cat>.Should()
                .ForMember(x => x.SomeField)
                .SetSerializer(str => $"{str}OfField")
                .Build()
                .PrintToString(cat);

            result.Should().Be($"Cat{nl}\tSomeField = SomeValueOfField{nl}\tName = Boris" +
                               $"{nl}\tWeight = 5,123{nl}\tWhiskersCount = 10");
        }

        [Test]
        public void PrintingConfig_DoesNotPrintType_WhenSetSerializerAfterExcluding()
        {
            var cat = GetCat();
            var result = ObjectPrinter<Cat>.Should()
                .Exclude<int>()
                .SetSerializerFor<int>(str => $"{str}.0")
                .Build()
                .PrintToString(cat);

            result.Should().Be($"Cat{nl}\tSomeField = SomeValue{nl}\tName = Boris{nl}\tWeight = 5,123");
        }

        [Test]
        public void PrintingConfig_SupportsSettingCultureForType()
        {
            var culture = new CultureInfo("us-EN", false);
            culture.NumberFormat.NumberDecimalDigits = 3;

            var cat = GetCat();
            var result = ObjectPrinter<Cat>.Should()
                .SetCultureFor<double>(culture)
                .Build()
                .PrintToString(cat);

            result.Should().Be($"Cat{nl}\tSomeField = SomeValue{nl}\tName = Boris{nl}\tWeight = 5.123{nl}\tWhiskersCount = 10");
        }

        [Test]
        public void PrintingConfig_SupportsTrimmingString()
        {
            var cat = GetCat();
            var result = ObjectPrinter<Cat>.Should()
                .ForMember(x => x.Name)
                .TrimmedToLength(3)
                .Build()
                .PrintToString(cat);

            result.Should().Be($"Cat{nl}\tSomeField = SomeValue{nl}\tName = Bor{nl}\tWeight = 5,123{nl}\tWhiskersCount = 10");
        }

        [Test]
        public void PrintingConfig_SupportsNestedObjects()
        {
            var person = GetPerson();
            var result = ObjectPrinter<Person>.Should()
                .Exclude<Person>()
                .Exclude<List<Person>>()
                .Exclude<Person[]>()
                .Exclude<Dictionary<string, Person>>()
                .Build()
                .PrintToString(person);

            var expexted = $"Person" +
                           $"{nl}\tName = Alex" + 
                           $"{nl}\tCar = Car" +
                           $"{nl}\t\tBrand = Lada" +
                           $"{nl}\t\tColor = Black" +
                           $"{nl}\t\tLicensePlateNumber = Т808ОС" +
                           $"{nl}\t\tReleaseDate = 12.10.2006 0:00:00";
            result.Should().Be(expexted);
        }

        [Test]
        public void PrintingConfig_SupportsArrays()
        {
            var person = GetPerson();
            var result = ObjectPrinter<Person>.Should()
                .Exclude<Person>()
                .Exclude<List<Person>>()
                .Exclude<Car>()
                .Exclude<Dictionary<string, Person>>()
                .Build()
                .PrintToString(person);

            var expexted = $"Person" +
                           $"{nl}\tName = Alex" +
                           $"{nl}\tChildrenArray = Person[]" +
                           $"{nl}\t\tPerson" +
                           $"{nl}\t\t\tName = Tom" +
                           $"{nl}\t\t\tChildrenArray = null" +
                           $"{nl}\t\tPerson" +
                           $"{nl}\t\t\tName = Max" +
                           $"{nl}\t\t\tChildrenArray = null";
            result.Should().Be(expexted);
        }

        [Test]
        public void PrintingConfig_SupportsDictionary()
        {
            var person = GetPerson();
            var result = ObjectPrinter<Person>.Should()
                .Exclude<Person>()
                .Exclude<List<Person>>()
                .Exclude<Car>()
                .Exclude<Person[]>()
                .Build()
                .PrintToString(person);

            var expexted = $"Person" +
                           $"{nl}\tName = Alex" +
                           $"{nl}\tChildrenDict = Dictionary`2" +
                           $"{nl}\t\tKeyValuePair`2" +
                           $"{nl}\t\t\tKey = Tom" +
                           $"{nl}\t\tKeyValuePair`2" +
                           $"{nl}\t\t\tKey = Max";
            result.Should().Be(expexted);
        }

        [Test]
        public void PrintingConfig_SupportsList()
        {
            var person = GetPerson();
            var result = ObjectPrinter<Person>.Should()
                .Exclude<Person>()
                .Exclude<Dictionary<string, Person>>()
                .Exclude<Car>()
                .Exclude<Person[]>()
                .Build()
                .PrintToString(person);

            var expexted = $"Person" +
                           $"{nl}\tName = Alex" +
                           $"{nl}\tChildren = List`1" +
                           $"{nl}\t\tPerson" +
                           $"{nl}\t\t\tName = Tom" +
                           $"{nl}\t\t\tChildren = List`1" +
                           $"{nl}\t\tPerson" +
                           $"{nl}\t\t\tName = Max" +
                           $"{nl}\t\t\tChildren = List`1";
            result.Should().Be(expexted);
        }

        [Test]
        public void PrintingConfig_SupportsLoopedReferences()
        {
            var person = GetPerson();
            var result = ObjectPrinter<Person>.Should()
                .Exclude<Dictionary<string, Person>>()
                .Exclude<Car>()
                .Exclude<Person[]>()
                .Build()
                .PrintToString(person);

            var expexted = $"Person" +
                           $"{nl}\tName = Alex" +
                           $"{nl}\tChildren = List`1" +
                           $"{nl}\t\tPerson" +
                           $"{nl}\t\t\tName = Tom" +
                           $"{nl}\t\t\tChildren = List`1" +
                           $"{nl}\t\t\tParent = Person (Looped)" +
                           $"{nl}\t\tPerson" +
                           $"{nl}\t\t\tName = Max" +
                           $"{nl}\t\t\tChildren = List`1" +
                           $"{nl}\t\t\tParent = Person (Looped)" +
                           $"{nl}\tParent = null";
            result.Should().Be(expexted);
        }

        private Person GetPerson()
        {
            var person = new Person("Alex",
                new Car("Lada", "Black", "Т808ОС", new DateTime(2006, 10, 12)), null);
            person.AddChild("Tom");
            person.AddChild("Max");
            return person;
        }

        private Cat GetCat()
        {
            return new Cat("Boris", 5.123, 10);
        }
    }
}