using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting.Solved;
using ObjectPrinting.Solved.Tests;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {


        [Test]
        public void Demo()
        {
            var person = new Person { Name = "Alex", Age = 19 };

            var printer = ObjectPrinter.For<Person>()
                //1. Исключить из сериализации свойства определенного типа
                .WithoutType<int>()
                //2. Указать альтернативный способ сериализации для определенного типа
                .SerializeTypeAs<int>(i => i.ToString("X"))
                //3. Для числовых типов указать культуру
                .SetCulture(CultureInfo.InvariantCulture)
                //4. Настроить сериализацию конкретного свойства
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                .TrimStrings(10)
                //6. Исключить из сериализации конкретного свойства
                .WithoutProperty(p => p.Age);

            string s1 = printer.PrintToString(person);

            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию
            string s2 = person.PrintToString();

            //8. ...с конфигурированием
            string s3 = person.PrintToString(s => s.Excluding(p => p.Age));
            Console.WriteLine(s1);
            Console.WriteLine(s2);
            Console.WriteLine(s3);
        }
        
        [Test]
        public void ObjectPrinter_PrintDefaultObject()
        {
            var printer = ObjectPrinter.For<Person>();
            var person = new Person{Name = "Aphanasiy", Age = 321, Height = 1.83 };
            var serializedPerson = printer.PrintToString(person);
            serializedPerson.Should().Be("Person\r\n\tId = 00000000-0000-0000-0000-000000000000\r\n\tName = Aphanasiy\r\n\tHeight = 1,83\r\n\tAge = 321\r\n");
        }

        [Test]
        public void ObjectPrinter_ExcludeType()
        {
            var printer = ObjectPrinter.For<Person>()
                .WithoutType<int>();
            var person = new Person { Name = "Aphanasiy", Age = 321, Height = 1.83 };
            var serializedPerson = printer.PrintToString(person);
            serializedPerson.Should().Be("Person\r\n\tId = 00000000-0000-0000-0000-000000000000\r\n\tName = Aphanasiy\r\n\tHeight = 1,83\r\n");
        }


        [Test]
        public void ObjectPrinter_ShouldSerializeSomeTypesDifferent()
        {
            var printer = ObjectPrinter.For<Person>()
                .SerializeTypeAs<string>(str => "abc");
            var person = new Person { Name = "Aphanasiy", Age = 321, Height = 1.83 };
            var serializedPerson = printer.PrintToString(person);
            serializedPerson.Should().Be("Person\r\n\tId = 00000000-0000-0000-0000-000000000000\r\n\tName = abc\r\n\tHeight = 1,83\r\n\tAge = 321\r\n");
        }

        [Test]
        public void ObjectPrinter_ShouldSetCulture()
        {
            var printer = ObjectPrinter.For<Person>()
                .SetCulture(CultureInfo.InvariantCulture);
            var person = new Person { Name = "Aphanasiy", Age = 321, Height = 1.83 };
            var serializedPerson = printer.PrintToString(person);
            serializedPerson.Should().Be("Person\r\n\tId = 00000000-0000-0000-0000-000000000000\r\n\tName = Aphanasiy\r\n\tHeight = 1.83\r\n\tAge = 321\r\n");

            printer = printer
                .SetCulture(CultureInfo.GetCultureInfo(3));
            serializedPerson = printer.PrintToString(person);
            serializedPerson.Should().Be("Person\r\n\tId = 00000000-0000-0000-0000-000000000000\r\n\tName = Aphanasiy\r\n\tHeight = 1,83\r\n\tAge = 321\r\n");
        }

        [Test]
        public void ObjectPrinter_ShouldTrimStrings()
        {
            var printer = ObjectPrinter.For<Person>()
                .TrimStrings(5);
            var person = new Person { Name = "Aphanasiy", Age = 321, Height = 1.83};
            var serializedPerson = printer.PrintToString(person);
            serializedPerson.Should().Be("Person\r\n\tId = 00000000-0000-0000-0000-000000000000\r\n\tName = Aphan\r\n\tHeight = 1,83\r\n\tAge = 321\r\n");
        }

        [Test]
        public void ObjectPrinter_ShouldExcludeSomeProperties()
        {
            var printer = ObjectPrinter.For<Person>()
                .WithoutProperty(p => p.Name);
            var person = new Person { Name = "Aphanasiy", Age = 321, Height = 1.83};
            var serializedPerson = printer.PrintToString(person);
            serializedPerson.Should().Be("Person\r\n\tId = 00000000-0000-0000-0000-000000000000\r\n\tHeight = 1,83\r\n\tAge = 321\r\n");
        }

        [Test]
        public void ObjectPrinter_ShouldThrowExceptionWithWrongExpression()
        {
            Action action = () => ObjectPrinter.For<Person>()
                .WithoutProperty(person => 5);
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void ObjectPrinter_ShouldSerializeSomePropertiesDifferent()
        {
            var printer = ObjectPrinter.For<Person>()
                .SerializePropertyAs(p => p.Age, age => "785");
            var person = new Person { Name = "Aphanasiy", Age = 321, Height = 1.83 };
            var serializedPerson = printer.PrintToString(person);
            serializedPerson.Should().Be("Person\r\n\tId = 00000000-0000-0000-0000-000000000000\r\n\tName = Aphanasiy\r\n\tHeight = 1,83\r\n\tAge = 785\r\n");
        }
    }
}