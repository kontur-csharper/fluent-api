using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting;

namespace ObjectPrintingTests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        [SetUp]
        public void SetUp()
        {
            person = new Person {Name = "Alex", Age = 19, Height = 195.5};
            personPrintingConfig = ObjectPrinter.For<Person>();
        }

        private Person person;
        private PrintingConfig<Person> personPrintingConfig;

        [Test]
        public void Demo()
        {
            person.Parent = new Person {Name = "Anna"};
            personPrintingConfig = personPrintingConfig
                //1. Исключить из сериализации свойства определенного типа
                .Excluding<Guid>()
                //2. Указать альтернативный способ сериализации для определенного типа
                .Printing<int>().Using(i => i.ToString("X"))
                //3. Для числовых типов указать культуру
                .Printing<double>().Using(CultureInfo.InvariantCulture)
                //4. Настроить сериализацию конкретного свойства
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                .Printing(p => p.Name).TrimmedToLength(10)
                //6. Исключить из сериализации конкретного свойства
                .Excluding(p => p.Age);

            var s1 = personPrintingConfig.PrintToString(person);

            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию
            var s2 = person.PrintToString();

            //8. ...с конфигурированием
            var s3 = person.PrintToString(s => s.Excluding(p => p.Age));

            Console.WriteLine(s1);
            Console.WriteLine(s2);
            Console.WriteLine(s3);

            Console.WriteLine(new object[] {person, 3, 4, null}.PrintToString());
        }

        [Test]
        public void ObjectPrinter_ShouldReturnCorrectString_WithoutConfiguration()
        {
            person.PrintToString().Should().ContainAll("Person 1", "Id = 00000000-0000-0000-0000-000000000000",
                "Parent = null", "Name = Alex", "Height = 195,5", "Age = 19");
        }

        [Test]
        public void ObjectPrinter_ShouldIgnorePropertyType_IfPropertyTypeIsExcluding()
        {
            personPrintingConfig
                .Excluding<Guid>()
                .PrintToString(person)
                .Should()
                .NotContain(nameof(person.Id));
        }

        [Test]
        public void ObjectPrinter_ShouldIgnoreProperty_IfPropertyIsExcluding()
        {
            personPrintingConfig
                .Excluding(p => p.Age)
                .PrintToString(person)
                .Should()
                .NotContain(nameof(person.Age));
        }

        [Test]
        public void ObjectPrinter_ShouldUseCustomTypePrinter()
        {
            personPrintingConfig
                .Printing<string>()
                .Using(str => "name")
                .PrintToString(person)
                .Should()
                .NotContain(person.Name).And.Contain("name");
        }

        [Test]
        public void ObjectPrinter_ShouldUseCulture()
        {
            person
                .PrintToString()
                .Should()
                .Contain(person.Height.ToString());

            personPrintingConfig
                .Printing<double>()
                .Using(CultureInfo.InvariantCulture)
                .PrintToString(person)
                .Should()
                .Contain(person.Height.ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public void ObjectPrinter_ShouldUseCustomPropertyPrinter()
        {
            personPrintingConfig
                .Printing(p => p.Age)
                .Using(age => "100")
                .PrintToString(person)
                .Should()
                .NotContain($"{nameof(person.Age)} = {person.Age}")
                .And
                .Contain($"{nameof(person.Age)} = 100");
        }

        [Test]
        public void ObjectPrinter_ShouldTrimStringProperties()
        {
            personPrintingConfig
                .Printing(p => p.Name)
                .TrimmedToLength(1)
                .PrintToString(person)
                .Should()
                .NotContain("Al").And.Contain("A");
        }

        [Test]
        public void ObjectPrinter_ShouldPrintObjectWithCircularReferences()
        {
            person.Parent = person;
            Action action = () => person.PrintToString();
            action.Should().NotThrow<StackOverflowException>();
            person.PrintToString().Should().ContainAll("Person 1", "Id = 00000000-0000-0000-0000-000000000000",
                "Name = Alex", "Height = 195,5", "Age = 19", "Parent = Person 1");
        }

        [Test]
        public void ObjectPrinter_ShouldPrintHighlyNestedObject()
        {
            var firstPerson = person;
            var currentPerson = firstPerson;
            for (var i = 0; i < 100; i++)
            {
                currentPerson.Parent = new Person();
                currentPerson = currentPerson.Parent;
            }

            firstPerson.PrintToString()
                .Should()
                .Contain($"{nameof(Person)} {personPrintingConfig.maxSerializationDepth}")
                .And
                .NotContain($"{nameof(Person)} {personPrintingConfig.maxSerializationDepth + 1}");
        }

        [Test]
        [TestCaseSource(nameof(GetCollections))]
        public void ObjectPrinter_ShouldPrintCollection(ICollection collection)
        {
            var collectionStr = collection.PrintToString();
            Console.WriteLine(collectionStr);
            foreach (var obj in collection)
            {
                var objStrTokens = obj.PrintToString()
                    .Split('\n', '\r', '\t')
                    .Where(str => str != "");
                foreach (var strToken in objStrTokens)
                    collectionStr.Should().Contain(strToken);
            }
        }

        private static IEnumerable<TestCaseData> GetCollections()
        {
            var person = new Person {Name = "Alex", Age = 19, Height = 195.5};
            yield return new TestCaseData(new[] {3, 4});
            yield return new TestCaseData(new List<object> {person, 3, 4, null});
            yield return new TestCaseData(new Dictionary<object, object>
            {
                [person] = null,
                [3] = 1,
                [4] = person
            });
        }
    }
}