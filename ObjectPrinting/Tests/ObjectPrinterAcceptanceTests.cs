using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        private Person person;

        [SetUp]
        public void SetUp()
        {
            person = new Person {Name = "Alex", Age = 30, Height = 180, Id = new Guid(), Weight = 60};
        }


        [Test]
        public void Demo()
        {
            var person = new Person {Name = "Alex", Age = 19};

            var printer = ObjectPrinter.For<Person>()
                //1. Исключить из сериализации свойства определенного типа
                .Excluding<double>()
                //2. Указать альтернативный способ сериализации для определенного типа
                .Printing<int>().Using(prop => prop.ToString())
                //3. Для числовых типов указать культуру
                .Printing<int>().Using(CultureInfo.InvariantCulture)
                //4. Настроить сериализацию конкретного свойства
                .Printing(p => p.Age).Using(prop => prop.ToString())
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                .Printing<string>().TrimmedToLength(20)
                //6. Исключить из сериализации конкретного свойства
                .Excluding(p => p.Name);

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
        public void PrinterShouldntPrint_WhenTypeExcluded()
        {
            var printer = ObjectPrinter.For<Person>().Excluding<Guid>();
            printer.PrintToString(person).Should().NotContain("Id = ");
            Console.WriteLine(printer.PrintToString(person));
        }

        [Test]
        public void PrinterShouldntPrint_WhenPropertyExcluded()
        {
            var printer = ObjectPrinter.For<Person>().Excluding(p => p.Age);
            printer.PrintToString(person).Should().NotContain("Age = 30");
            Console.WriteLine(printer.PrintToString(person));
        }

        [Test]
        public void PrinterShouldntPrint_WhenSeveralTypesExcluded()
        {
            var printer = ObjectPrinter.For<Person>().Excluding<Guid>().Excluding<int>();
            printer.PrintToString(person).Should().NotContain("Id = ");
            printer.PrintToString(person).Should().NotContain("Age = ");
        }

        [Test]
        public void PrinterShouldPrintWithFormatting_WhenAlternateSerializationForType()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<int>().Using(prop => $"{prop} with using");
            printer.PrintToString(person).Should().Contain("Age = 30 with using");
        }

        [Test]
        public void PrinterShouldPrintWithFormatting_WhenAlternateSerializationMember()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing(p => p.Age).Using(prop => $"{prop} with using");
            printer.PrintToString(person).Should().Contain("Age = 30 with using");
        }

        [Test]
        public void PrinterShouldPrintWithFormatting_WhenTrimUsed()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<string>().TrimmedToLength(2);
            printer.PrintToString(person).Should().Contain("Name = Al");
        }

        [Test]
        public void PrinterShouldPrintWithFormatting_WhenTrimGreaterThenStringLength()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<string>().TrimmedToLength(10);
            printer.PrintToString(person).Should().Contain("Name = Alex");
        }

        [Test]
        public void PrinterShouldPrintWithFormatting_WhenCultureSet()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<int>().Using(CultureInfo.InvariantCulture);
            printer.PrintToString(person).Should().Contain("Age = 30");
        }

        [Test]
        public void PrinterShouldWorkCorrect_WhenCircularLinkFound()
        {
            var child = new Person
            {
                Name = "Kate",
                Age = 5,
                Height = 105,
                Id = new Guid(),
                Parent = person
            };
            person.Children.Add(child);

            var printer = ObjectPrinter.For<Person>();
            printer.PrintToString(person).Should().Contain("Parent = Person");
        }

        [Test]
        public void PrinterShouldWork_WhenObjectHaveEmptyCollection()
        {
            person.PrintToString().Should().Contain("CustomDict = Empty");
            person.PrintToString().Should().Contain("Children = Empty");
        }

        [Test]
        public void PrinterShouldWork_WhenObjectHaveNonEmptyCollection()
        {
            person.CustomDict.Add("string", 1234);
            var serialized = person.PrintToString();
            serialized.Should().Contain($"string: 1234{Environment.NewLine}");

        }

        [Test]
        public void PrinterShouldWorkCorrect_WhenMultipleConfigurationUsed()
        {
            var printer = ObjectPrinter.For<Person>()
                .Excluding<Person>()
                .Printing<int>().Using(prop => "value: " + prop)
                .Printing<double>().Using(CultureInfo.InvariantCulture)
                .Printing(p => p.Weight).Using(prop => prop + "kg")
                .Printing<string>().TrimmedToLength(2)
                .Excluding(p => p.Height);

            printer.PrintToString(person).Should().Be(
                $"Person{Environment.NewLine}" +
                $"\tId = Guid{Environment.NewLine}" +
                $"\tName = Al{Environment.NewLine}" +
                $"\tAge = value: 30{Environment.NewLine}" +
                $"\tChildren = Empty{Environment.NewLine}" +
                $"\tCustomDict = Empty{Environment.NewLine}" +
                $"\tWeight = 60kg{Environment.NewLine}");
        }

        [Test]
        public void PrinterShouldWorkCorrect_WhenNestedMemberTrimmed()
        {
            var foo = new Bar {Foo = new Foo {Name = "Hello"}};
            var printer = ObjectPrinter.For<Bar>().Printing(b => b.Foo.Name).TrimmedToLength(2);
            var actual = printer.PrintToString(foo);
            actual.Should().Contain("Name = He");
        }

        [Test]
        public void PrinterShouldntExcludeAll_WhenMembersHaveSameName()
        {
            var foo = new Bar {Foo = new Foo {Name = "Hello"}, Name = "World"};
            var printer = ObjectPrinter.For<Bar>().Excluding(b => b.Name);
            var actual = printer.PrintToString(foo);
            actual.Should().Contain("Name = Hello");
        }

        [Test]
        public void PrinterShouldWorkCorrect_WhenEqualsAndGetHashMethodsOverriden()
        {
            var foo = new FooWithEquals {Name = "Hello", Next = new FooWithEquals {Name = "Hello"}};
            var printer = ObjectPrinter.For<FooWithEquals>();
            var actual = printer.PrintToString(foo);
            actual.Should().Be($"FooWithEquals{Environment.NewLine}" +
                               $"\tName = Hello{Environment.NewLine}" +
                               $"\tNext = FooWithEquals{Environment.NewLine}" +
                               $"\t\tName = Hello{Environment.NewLine}" +
                               $"\t\tNext = null{Environment.NewLine}");
        }

        [Test]
        public void PrinterShouldWorkCorrect_WhenSeveralIdenticalMembersOnTheSameLevel()
        {
            var foo1 = new Foo {Name = "Foo1"};
            var foo2 = new Foo {Name = "Foo2", Next = foo1, Friend = foo1};
            var printer = ObjectPrinter.For<Foo>();
            var actual = printer.PrintToString(foo2);
            actual.Should().Be($"Foo{Environment.NewLine}" +
                               $"\tName = Foo2{Environment.NewLine}" +
                               $"\tNext = Foo{Environment.NewLine}" +
                               $"\t\tName = Foo1{Environment.NewLine}" +
                               $"\t\tNext = null{Environment.NewLine}" +
                               $"\t\tFriend = null{Environment.NewLine}" +
                               $"\tFriend = Foo{Environment.NewLine}" +
                               $"\t\tName = Foo1{Environment.NewLine}" +
                               $"\t\tNext = null{Environment.NewLine}" +
                               $"\t\tFriend = null{Environment.NewLine}");
        }
    }
}