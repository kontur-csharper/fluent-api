using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        [Test]
        public void Should_BeAbleTo_ExcludeTypes()
        {
            var person = new Person {Name = "Test", Age = 10};
            var printer = ObjectPrinter.For<Person>()
                .Excluding<double>()
                .Excluding<string>();

            printer.PrintToString(person)
                .Should().NotContain($"{nameof(person.Name)}")
                .And.NotContain($"{nameof(person.LastName)}")
                .And.NotContain($"{nameof(person.Height)}");
        }

        [Test]
        public void Should_BeAbleTo_SerializeTypesAlternatively()
        {
            var person = new Person { Name = "Test", Age = 10 };
            var printer = ObjectPrinter.For<Person>()
                .Printing<string>().Using(x => x.Length.ToString());

            printer.PrintToString(person)
                .Should().Contain($"{nameof(person.Name)} = 4");
        }

        [Test]
        public void Should_BeAbleTo_SetCultureForTypes()
        {
            var person = new Person { Name = "Test", DateOfBirth = new DateTime(2004, 02, 17)};
            var printer = ObjectPrinter.For<Person>()
                .Printing<DateTime>().UsingCulture(CultureInfo.InvariantCulture);

            printer.PrintToString(person)
                .Should().Contain($"{nameof(person.DateOfBirth)} = {person.DateOfBirth.ToString(CultureInfo.InvariantCulture)}");
        }

        [Test]
        public void Should_BeAbleTo_SerializeSpecialPropertyAlternatively()
        {
            var person = new Person { Name = "Test", LastName = "Test", Age = 10 };
            var printer = ObjectPrinter.For<Person>()
                .Printing(x => x.Name).Using(x => x.ToUpper());

            printer.PrintToString(person)
                .Should().Contain($"{nameof(person.Name)} = {person.Name.ToUpper()}");
        }

        [Test]
        public void Should_BeAbleTo_TrimStringMembers()
        {
            var person = new Person { Name = "TestToTrim", LastName = "TestToTrim", Age = 10 };
            var printer = ObjectPrinter.For<Person>()
                .Printing<string>().TrimmedToLength(4);

            printer.PrintToString(person)
                .Should().NotContain("TestToTrim")
                .And.Contain("Test");
        }

        [Test]
        public void Should_BeAbleTo_ExcludeSpecialMember()
        {
            var person = new Person { Name = "Test", LastName = "Test", Age = 10, Height = 140.37};
            var printer = ObjectPrinter.For<Person>()
                .Excluding(x => x.LastName);

            printer.PrintToString(person)
                .Should().NotContain($"{nameof(person.LastName)} = {person.LastName}");

        }

        [Test]
        public void Should_MarkCyclicReferencesCorrectly()
        {
            var person = new Person {Name = "Tester", Father = new Person {Name = "Batya"}};
            var printer = ObjectPrinter.For<Person>();
            printer.PrintToString(person)
                .Should().NotContain("Cyclic reference detected")
                .And.Contain($"{person.Father.Name}");
        }

        [Test]
        public void Should_MarkReferencesToSelf_AsCyclic()
        {
            var person = new Person { Name = "Tester"};
            person.Father = person;
            var printer = ObjectPrinter.For<Person>();

            printer.PrintToString(person)
                .Should().Contain("Cyclic reference detected");
        }

        [Test]
        public void Should_MarkDeepReferenceToSelf_AsCyclic()
        {
            var person = new Person { Name = "Tester", Father = new Person{Name = "Father"}};
            person.Father.Father = person;
            var printer = ObjectPrinter.For<Person>();
            var result = printer.PrintToString(person);
            
            result
                .Should().Contain("Cyclic reference detected")
                .And.Contain($"{nameof(person.Name)} = {person.Father.Name}");

            Console.WriteLine(result);
        }

        [Test]
        public void Should_MarkReferenceToSelf_AsCyclic_OnEachLevel()
        {
            var person = new Person
                {Name = "Tester", Father = new Person {Name = "Father"}, Mother = new Person {Name = "Father"}};
            person.Father.Father = person;
            person.Mother = person;
            var printer = ObjectPrinter.For<Person>();
            var result = printer.PrintToString(person);

            result
                .Should().Contain($"{nameof(person.Father)} = [Cyclic reference detected]")
                .And.Contain($"{nameof(person.Mother)} = [Cyclic reference detected]");

            Console.WriteLine(result);
        }

        [Test]
        public void Should_SupportFields()
        {
            var person = new Person { Name = "Test", LastName = "Test", Age = 10, Height = 140.37 };
            person.Field = 2304;

            var printer = ObjectPrinter.For<Person>()
                .Printing(x => x.Field).Using(x => x.ToString("X"));

            printer.PrintToString(person)
                .Should().Contain($"{nameof(person.Field)} = {person.Field:X}");
        }

        [Test]
        public void Should_SupportArray()
        {
            var arr = new[] {1, 2, 3, 4, 5};
            var printer = ObjectPrinter.For<int[]>();
            var result = printer.PrintToString(arr);
            result.Should()
                .ContainAll(arr.Select(x => x.ToString()));
            Console.WriteLine(result);
        }

        [Test]
        public void Should_SupportList()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var printer = ObjectPrinter.For<List<int>>();
            var result = printer.PrintToString(list);
            result
                .Should().ContainAll(list.Select(x => x.ToString()));
            Console.WriteLine(result);
        }

        [Test]
        public void Should_SupportDictionary()
        {
            var dict = new Dictionary<int, string> {{1, "One"}, {2, "Two"}, {3, "Three"}, {4, "Four"}, {5, "Five"}};
            var printer = ObjectPrinter.For<Dictionary<int, string>>();
            var result = printer.PrintToString(dict);
            result.Should()
                .ContainAll(dict.Keys.Select(x => x.ToString()))
                .And.ContainAll(dict.Values.Select(x => x.ToString()));
            Console.WriteLine(result);
        }

        [Test]
        public void Should_SerializeEmptyList_AsEmpty()
        {
            var list = new List<int>();
            var printer = ObjectPrinter.For<List<int>>();
            printer.PrintToString(list)
                .Should().Contain("Empty");
        }

        [Test]
        public void Should_SerializeEmptyDictionary_AsEmpty()
        {
            var dict = new Dictionary<int, string>();
            var printer = ObjectPrinter.For<Dictionary<int, string>>();
            printer.PrintToString(dict)
                .Should().Contain("Empty");
        }

        [Test]
        public void Should_SerializeEmptyArray_AsEmpty()
        {
            var arr = new int[0];
            var printer = ObjectPrinter.For<int[]>();
            printer.PrintToString(arr)
                .Should().Contain("Empty");
        }
    }
}