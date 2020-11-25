using System;
using System.Collections.Generic;
using System.Globalization;
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
            person = new Person {Age = 19, Height = 175.1, Name = "Alex", Surname = "Ivanov"};
        }

        private Person person;

        [Test]
        public void Demo()
        {
            var printer = ObjectPrinter.For<Person>()
                .Excluding<Guid>()
                .Printing<int>().Using(i => i.ToString("X"))
                .Printing<double>().Using(CultureInfo.InvariantCulture)
                .Printing(person => person.Name).Using(str => str.ToUpper())
                .Printing(p => p.Name).TrimmedToLength(10)
                .Excluding(p => p.Age);

            var s1 = printer.PrintToString(person);

            var s2 = person.PrintToString();

            var s3 = person.PrintToString(s => s.Excluding(p => p.Age));
            Console.WriteLine(s1);
            Console.WriteLine(s2);
            Console.WriteLine(s3);
        }

        [Test]
        public void PrintToString_WhenSomeMembersIsNull()
        {
            person.Surname = null;
            var printer = ObjectPrinter.For<Person>()
                .Printing<string>().Using(str => str.ToUpper());
            var serialization = printer.PrintToString(person);
            serialization.Should().Be(
                $"Person{Environment.NewLine}\tId = Guid{Environment.NewLine}\tName = ALEX{Environment.NewLine}" +
                $"\tHeight = 175,1{Environment.NewLine}\tAge = 19{Environment.NewLine}\tSurname = null{Environment.NewLine}");
        }

        [Test]
        public void Excluding_OneType()
        {
            var printer = ObjectPrinter.For<Person>()
                .Excluding<Guid>();
            var serialization = printer.PrintToString(person);
            serialization.Should()
                .Be(
                    $"Person{Environment.NewLine}\tName = Alex{Environment.NewLine}\tHeight = 175,1{Environment.NewLine}\tAge = 19{Environment.NewLine}\tSurname = Ivanov{Environment.NewLine}");
        }

        [Test]
        public void Excluding_SeveralTypes()
        {
            var printer = ObjectPrinter.For<Person>()
                .Excluding<Guid>().Excluding<int>();
            var serialization = printer.PrintToString(person);
            serialization.Should()
                .Be(
                    $"Person{Environment.NewLine}\tName = Alex{Environment.NewLine}\tHeight = 175,1{Environment.NewLine}\tSurname = Ivanov{Environment.NewLine}");
        }

        [Test]
        public void Using_OwnSerialization_ForOneType()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<string>().Using(str => str.ToUpper());
            var serialization = printer.PrintToString(person);
            serialization.Should().Be(
                $"Person{Environment.NewLine}\tId = Guid{Environment.NewLine}\tName = ALEX{Environment.NewLine}" +
                $"\tHeight = 175,1{Environment.NewLine}\tAge = 19{Environment.NewLine}\tSurname = IVANOV{Environment.NewLine}");
        }

        [Test]
        public void Using_OwnSerialization_ForSeveralTypes()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<string>().Using(str => str.ToUpper())
                .Printing<double>().Using(number => number.ToString("F"));
            var serialization = printer.PrintToString(person);
            serialization.Should().Be(
                $"Person{Environment.NewLine}\tId = Guid{Environment.NewLine}\tName = ALEX{Environment.NewLine}" +
                $"\tHeight = 175,10{Environment.NewLine}\tAge = 19{Environment.NewLine}\tSurname = IVANOV{Environment.NewLine}");
        }

        [Test]
        public void Using_SerializationWithCultureInfo_ForNumbers()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<double>().Using(CultureInfo.CurrentUICulture);
            var serialization = printer.PrintToString(person);
            serialization.Should().Be(
                $"Person{Environment.NewLine}\tId = Guid{Environment.NewLine}\tName = Alex{Environment.NewLine}" +
                $"\tHeight = 175,1{Environment.NewLine}\tAge = 19{Environment.NewLine}\tSurname = Ivanov{Environment.NewLine}");
        }

        [Test]
        public void Excluding_OneProperty()
        {
            var printer = ObjectPrinter.For<Person>()
                .Excluding(person => person.Age);
            var serialization = printer.PrintToString(person);
            serialization.Should().Be(
                $"Person{Environment.NewLine}\tId = Guid{Environment.NewLine}\tName = Alex{Environment.NewLine}" +
                $"\tHeight = 175,1{Environment.NewLine}\tSurname = Ivanov{Environment.NewLine}");
        }

        [Test]
        public void Excluding_OneField()
        {
            var printer = ObjectPrinter.For<Person>()
                .Excluding(person => person.Surname);
            var serialization = printer.PrintToString(person);
            serialization.Should().Be(
                $"Person{Environment.NewLine}\tId = Guid{Environment.NewLine}\tName = Alex{Environment.NewLine}" +
                $"\tHeight = 175,1{Environment.NewLine}\tAge = 19{Environment.NewLine}");
        }

        [Test]
        public void Excluding_SeveralMembers()
        {
            var printer = ObjectPrinter.For<Person>()
                .Excluding(person => person.Surname)
                .Excluding(person => person.Age);
            var serialization = printer.PrintToString(person);
            serialization.Should().Be(
                $"Person{Environment.NewLine}\tId = Guid{Environment.NewLine}\tName = Alex{Environment.NewLine}" +
                $"\tHeight = 175,1{Environment.NewLine}");
        }

        [Test]
        public void Excluding_ThrowsException_WhenExpressionNotCorrect()
        {
            var printer = ObjectPrinter.For<Person>();
            Assert.Throws<Exception>(() => printer.Excluding(person => 5));
        }

        [Test]
        public void Excluding_ThrowsException_WhenMemberNotExistInClass()
        {
            var printer = ObjectPrinter.For<Person>();
            Assert.Throws<Exception>(() => printer.Excluding(person => string.Empty));
        }

        [Test]
        public void Using_OwnSerialization_ForOneProperty()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing(person => person.Name).Using(str => str.ToUpper());
            var serialization = printer.PrintToString(person);
            serialization.Should().Be(
                $"Person{Environment.NewLine}\tId = Guid{Environment.NewLine}\tName = ALEX{Environment.NewLine}" +
                $"\tHeight = 175,1{Environment.NewLine}\tAge = 19{Environment.NewLine}\tSurname = Ivanov{Environment.NewLine}");
        }

        [Test]
        public void Using_OwnSerialization_ForOneField()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing(person => person.Surname).Using(str => str.ToUpper());
            var serialization = printer.PrintToString(person);
            serialization.Should().Be(
                $"Person{Environment.NewLine}\tId = Guid{Environment.NewLine}\tName = Alex{Environment.NewLine}" +
                $"\tHeight = 175,1{Environment.NewLine}\tAge = 19{Environment.NewLine}\tSurname = IVANOV{Environment.NewLine}");
        }

        [Test]
        public void Using_OwnSerialization_ForSeveralMembers()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing(person => person.Surname).Using(str => str.ToUpper())
                .Printing(person => person.Height).Using(CultureInfo.InvariantCulture);
            var serialization = printer.PrintToString(person);
            serialization.Should().Be(
                $"Person{Environment.NewLine}\tId = Guid{Environment.NewLine}\tName = Alex{Environment.NewLine}" +
                $"\tHeight = 175.1{Environment.NewLine}\tAge = 19{Environment.NewLine}\tSurname = IVANOV{Environment.NewLine}");
        }

        [Test]
        public void Printing_ThrowsException_WhenExpressionNotCorrect()
        {
            var printer = ObjectPrinter.For<Person>();
            Assert.Throws<Exception>(() => printer.Printing(person => 5));
        }

        [Test]
        public void Printing_ThrowsException_WhenMemberNotExistInClass()
        {
            var printer = ObjectPrinter.For<Person>();
            Assert.Throws<Exception>(() => printer.Printing(person => string.Empty));
        }

        [Test]
        public void TrimmedToLength_WhenMaxLengthMoreThanLineLength()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<string>().TrimmedToLength(2);
            var serialization = printer.PrintToString(person);
            serialization.Should().Be(
                $"Person{Environment.NewLine}\tId = Guid{Environment.NewLine}\tName = Al{Environment.NewLine}" +
                $"\tHeight = 175,1{Environment.NewLine}\tAge = 19{Environment.NewLine}\tSurname = Iv{Environment.NewLine}");
        }

        [Test]
        public void TrimmedToLength_WhenMaxLengthLessThanLineLength()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<string>().TrimmedToLength(10);
            var serialization = printer.PrintToString(person);
            serialization.Should().Be(
                $"Person{Environment.NewLine}\tId = Guid{Environment.NewLine}\tName = Alex{Environment.NewLine}" +
                $"\tHeight = 175,1{Environment.NewLine}\tAge = 19{Environment.NewLine}\tSurname = Ivanov{Environment.NewLine}");
        }

        [Test]
        public void PrintToString_Dictionary_Demo()
        {
            var dictionary = new Dictionary<string, Person>
            {
                {"a", person},
                {"b", person}
            };
            Console.WriteLine(dictionary.PrintToString());
        }

        [Test]
        public void PrintToString_List_Demo()
        {
            var list = new List<Person>();
            list.Add(person);
            list.Add(person);
            Console.WriteLine(list.PrintToString());
        }

        [Test]
        public void PrintToString_Array_Demo()
        {
            var array = new[] {person, person};
            Console.WriteLine(array.PrintToString());
        }

        [Test]
        public void PrintToString_WhenThereIsCyclicalRef()
        {
            var obj = new ClassForTestingCyclicalRefs();
            obj.Reference = obj;
            obj.TestStringSerialization = "Test";
            var serialization = obj.PrintToString();
            serialization.Should()
                .Be(
                    $"ClassForTestingCyclicalRefs{Environment.NewLine}\tReference = Cyclical reference{Environment.NewLine}" +
                    $"\tTestStringSerialization = Test{Environment.NewLine}" +
                    $"\tTestIntSerialization = 0{Environment.NewLine}");
        }
    }
}