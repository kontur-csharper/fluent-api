using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    public class ObjectPrinterTests
    {
        private PrintingConfig<Person> printer;
        private Person person;

        [SetUp]
        public void SetUp()
        {
            printer = ObjectPrinter.For<Person>();
            person = new Person();
        }

        [Test]
        public void PrintToString_Correct_WhenExcludeProperty()
        {
            printer.Excluding(x => x.Age);

            printer.PrintToString(person).Should().NotContain("Age =");
        }

        [Test]
        public void PrintToString_Correct_WhenExcludeField()
        {
            person.Nickname = "Abcde";
            printer.Excluding(x => x.Nickname);
            printer.PrintToString(person).Should().NotContain("Nickname = ");
        }

        [Test]
        public void PrintToString_Correct_WhenExcludeType()
        {
            printer.Excluding<string>();

            var stringMembersNames = person.GetType().GetFieldsAndProperties()
                .Where(x => x.GetValueType() == typeof(string))
                .Select(x => x.Name);

            printer.PrintToString(person).Should().NotContainAll(stringMembersNames);
        }

        [Test]
        public void PrintToString_Correct_WhenSpecialSerializeForType()
        {
            person.Age = 5;
            var serializeResult = "its integer!";

            printer.Printing<int>().Using(x => serializeResult);

            printer.PrintToString(person).Should().Contain(serializeResult).And.NotContain("5");
        }

        [Test]
        public void PrintToString_Correct_WhenSpecialSerializeForProperty()
        {
            person.Name = "Hulio";
            var serializeResult = "Hello!";

            printer.Printing(x => x.Name).Using(x => serializeResult);

            printer.PrintToString(person).Should().Contain(serializeResult).And.NotContain("Hulio");
        }

        [Test]
        public void PrintToString_Correct_WhenSpecialSerializeForField()
        {
            person.Nickname = "Hulio";
            var serializeResult = "Hello!";

            printer.Printing(x => x.Nickname).Using(x => serializeResult);

            printer.PrintToString(person).Should().Contain(serializeResult).And.NotContain("Hulio");
        }

        [Test]
        public void PrintToString_Correct_WhenUseCultureForDouble()
        {
            person.Height = 2.15;

            printer.Printing<double>().Using(new CultureInfo("eu-ES"));

            printer.PrintToString(person).Should().Contain("2,15").And.NotContain("2.15");
        }

        [Test]
        public void PringToString_Correct_WhenUseCultureForDateTime()
        {
            person.Birthday = new DateTime(2001, 11, 27);

            printer.Printing<DateTime>().Using(new CultureInfo("ru-RU"));

            printer.PrintToString(person).Should().Contain("27.11.2001 0:00:00");
        }

        [Test]
        public void PrintingToString_Correct_WhenTrimString()
        {
            person.Name = "aAaaAAAAAAAA";
            person.Nickname = "bBbb";

            printer.Printing<string>().TrimmedToLength(5);

            printer.PrintToString(person).Should()
                .ContainAll($"Name = aAaaA{Environment.NewLine}",
                    $"Nickname = bBbb{Environment.NewLine}");
        }

        [Test]
        public void PrintingToString_NotThrow_WhenCуcle()
        {
            var friend = new Person() {Friend = person};
            person.Friend = friend;

            Action act = () => printer.PrintToString(person);

            act.Should().NotThrow<StackOverflowException>();
        }

        [Test]
        public void PrintingToString_DoesNotExclude_WhenPropertyOfAnotherObject()
        {
            person.Name = "Alex";
            person.Dog = new Pet() {Name = "Spotty"};

            printer.Excluding<string>();

            printer.PrintToString(person).Should().Contain("Spotty");
        }

        [Test]
        public void PrintingToString_Correct_WhenPropertyIsArray()
        {
            person.PhoneNumbers = new string[] {"Hello", "From", "Array!"};

            printer.PrintToString(person).Should()
                .ContainAll("String[]", "Hello", "From", "Array!");
        }

        [Test]
        public void PrintingToString_Correct_WhenPropertyIsDictionary()
        {
            person.PhoneNumbers = new Dictionary<string, string>()
            {
                {"Mom", "911"},
                {"Dad", "02"}
            };

            printer.PrintToString(person).Should()
                .ContainAll("Dictionary`2", "Key = Mom", "Key = Dad", "Value = 911", "Value = 02");
        }
    }
}
