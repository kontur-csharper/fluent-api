using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Solved.Tests
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
                .Excluding<Guid>()
                //2. Указать альтернативный способ сериализации для определенного типа
                .Printing<int>().Using(i => i.ToString("X"))
                //3. Для числовых типов указать культуру
                .Printing<double>().Using(CultureInfo.InvariantCulture)
                //4. Настроить сериализацию конкретного свойства
                .Printing(p => p.Name).Using(name => name.ToLower())
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                .Printing(p => p.Name).TrimmedToLength(10)
                //6. Исключить из сериализации конкретного свойства
                .Excluding(p => p.Age);

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
            var person = new Person { Name = "Aphanasiy", Age = 321, Height = 1.83 };
            var serializedPerson = printer.PrintToString(person);
            serializedPerson.Should().Be("Person Id = 0\r\n\tName = Aphanasiy\r\n\tHeight = 1,83\r\n\tAge = 321\r\n");
        }

        [Test]
        public void ObjectPrinter_ExcludeType()
        {
            var printer = ObjectPrinter.For<Person>()
                .Excluding<int>();
            var person = new Person { Name = "Aphanasiy", Age = 321, Height = 1.83 };
            var serializedPerson = printer.PrintToString(person);
            serializedPerson.Should().Be("Person Id = 0\r\n\tName = Aphanasiy\r\n\tHeight = 1,83\r\n");
        }


        [Test]
        public void ObjectPrinter_ShouldSerializeSomeTypesDifferent()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<string>().Using(str => "abc");
            var person = new Person { Name = "Aphanasiy", Age = 321, Height = 1.83 };
            var serializedPerson = printer.PrintToString(person);
            serializedPerson.Should().Be("Person Id = 0\r\n\tName = abc\r\n\tHeight = 1,83\r\n\tAge = 321\r\n");
        }

        [Test]
        public void ObjectPrinter_ShouldSetCulture()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<double>().Using(CultureInfo.InvariantCulture);
            var person = new Person { Name = "Aphanasiy", Age = 321, Height = 1.83 };
            var serializedPerson = printer.PrintToString(person);
            serializedPerson.Should().Be("Person Id = 0\r\n\tName = Aphanasiy\r\n\tHeight = 1.83\r\n\tAge = 321\r\n");

            printer = printer
                .Printing<double>().Using(CultureInfo.GetCultureInfo(3));
            serializedPerson = printer.PrintToString(person);
            serializedPerson.Should().Be("Person Id = 0\r\n\tName = Aphanasiy\r\n\tHeight = 1,83\r\n\tAge = 321\r\n");
        }

        [Test]
        public void ObjectPrinter_ShouldTrimStrings()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<string>().TrimmedToLength(5);
            var person = new Person { Name = "Aphanasiy", Age = 321, Height = 1.83 };
            var serializedPerson = printer.PrintToString(person);
            serializedPerson.Should().Be("Person Id = 0\r\n\tName = Aphan\r\n\tHeight = 1,83\r\n\tAge = 321\r\n");
        }

        [Test]
        public void ObjectPrinter_ShouldExcludeSomeProperties()
        {
            var printer = ObjectPrinter.For<Person>()
                .Excluding(p => p.Name);
            var person = new Person { Name = "Aphanasiy", Age = 321, Height = 1.83 };
            var serializedPerson = printer.PrintToString(person);
            serializedPerson.Should().Be("Person Id = 0\r\n\tHeight = 1,83\r\n\tAge = 321\r\n");
        }

        [Test]
        public void ObjectPrinter_ShouldThrowExceptionWithWrongExpression()
        {
            Action action = () => ObjectPrinter.For<Person>()
                .Excluding(person => 5);
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void ObjectPrinter_ShouldSerializeSomePropertiesDifferent()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing(p => p.Age).Using(age => "785");
            var person = new Person { Name = "Aphanasiy", Age = 321, Height = 1.83 };
            var serializedPerson = printer.PrintToString(person);
            serializedPerson.Should().Be("Person Id = 0\r\n\tName = Aphanasiy\r\n\tHeight = 1,83\r\n\tAge = 785\r\n");
        }

        [Test]
        public void ObjectPrinter_ShouldHandleRecurtionByIds()
        {
            var printer = ObjectPrinter.For<A>();
            var a = new A();
            a.bField = new B {aField = a};
            var serializedClass = printer.PrintToString(a);
            serializedClass.Should().Be($"A Id = 0\r\n\tbField = B Id = 1\r\n\t\taField = A Id = 0\r\n");
        }

        [Test]
        public void ObjectPrinter_ShouldSerializeArraysByElements()
        {
            var printer = ObjectPrinter.For<int[]>();
            var array = new []{1, 2, 3, 4};
            var serializedClass = printer.PrintToString(array);
            serializedClass.Should().Be("Int32[] Id = 0\r\n\t1\r\n\t2\r\n\t3\r\n\t4\r\n");
        }

        [Test]
        public void ObjectPrinter_ShouldSerializeListsByElements()
        {
            var printer = ObjectPrinter.For<List<int>>();
            var list = new List<int>{ 1, 2, 3, 4 };
            var serializedClass = printer.PrintToString(list);
            serializedClass.Should().Be("List`1 Id = 0\r\n\t1\r\n\t2\r\n\t3\r\n\t4\r\n");
        }

        [Test]
        public void ObjectPrinter_ShouldSerializeDictionaryByElements()
        {
            var printer = ObjectPrinter.For<Dictionary<string, int>>()
                .Printing<KeyValuePair<string, int>>().Using(pair => $"{pair.Key} : {pair.Value}");
            var dictionary = new Dictionary<string, int>
            {
                { "abc", 256 },
                { "po", 4 },
                { "f", 9 }
            };
            var serializedClass = printer.PrintToString(dictionary);
            serializedClass.Should().Be("Dictionary`2 Id = 0\r\n\tabc : 256\r\n\tpo : 4\r\n\tf : 9\r\n");
        }
    }
}