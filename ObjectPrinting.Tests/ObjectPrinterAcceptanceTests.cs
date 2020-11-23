using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    public class ObjectPrinterAcceptanceTests
    {
        private Person person;

        [SetUp]
        public void SetUp()
        {
            person = new Person {Name = "Alex"};
        }

        [Test]
        public void PrintToString_SerializedWithNewIndentation_WhenChangeIndentation()
        {
            var actual = ObjectPrinter.PrintToString(person, x => x.ChangeIndentation(" "));
            var expected = "Person\r\n Id = 00000000-0000-0000-0000-000000000000\r\n Name = Alex\r\n";

            actual.Should().Be(expected);
        }

        [Test]
        public void PrintToString_SerializedWithNewSeparator_WhenChangeSeparator()
        {
            var actual = ObjectPrinter.PrintToString(person, x => x.ChangeSeparatorBetweenNameAndValue(":"));
            var expected = "Person\r\n\tId : 00000000-0000-0000-0000-000000000000\r\n\tName : Alex\r\n";

            actual.Should().Be(expected);
        }

        [Test]
        public void PrintToString_SerializedWithoutInt_WhenExcludingGuid()
        {
            var actual = ObjectPrinter.PrintToString(person, x => x.Excluding<Guid>());
            var expected = "Person\r\n\tName = Alex\r\n";

            actual.Should().Be(expected);
        }

        [Test]
        public void PrintToString_SerializedWithChangedGuid_WhenPrintingGuid()
        {
            var actual = ObjectPrinter.PrintToString(person, x => x.Printing<Guid>().Using(i => "Guid"));
            var expected = "Person\r\n\tId = Guid\tName = Alex\r\n";

            actual.Should().Be(expected);
        }

        [Test]
        public void PrintToString_SerializedWithCulturedDateTime_WhenPrintingDateTimeWithCulture()
        {
            var time = new ClassWithDateTime {Date = new DateTime(2001, 2, 3)};

            var actual =
                ObjectPrinter.PrintToString(time, x => x.Printing<DateTime>().Using(CultureInfo.InvariantCulture));
            var expected = "ClassWithDateTime\r\n\tDate = 02/03/2001 00:00:00\r\n";

            actual.Should().Be(expected);
        }

        [Test]
        public void PrintToString_SerializedWithChangedMemberId_WhenPrintingId()
        {
            var actual = ObjectPrinter.PrintToString(person, x => x.Printing(p => p.Id).Using(p => "!Guid!"));
            var expected = "Person\r\n\tId = !Guid!\tName = Alex\r\n";

            actual.Should().Be(expected);
        }

        [Test]
        public void PrintToString_SerializedWithCutString_WhenPrintingWithCutting()
        {
            var actual = ObjectPrinter.PrintToString(person, x => x.Printing(p => p.Name).TrimmedToLength(3));
            var expected = "Person\r\n\tId = 00000000-0000-0000-0000-000000000000\r\n\tName = Ale";

            actual.Should().Be(expected);
        }

        [Test]
        public void PrintToString_SerializedWithoutMemberId_WhenExcludingId()
        {
            var actual = ObjectPrinter.PrintToString(person, x => x.Excluding(p => p.Id));
            var expected = "Person\r\n\tName = Alex\r\n";

            actual.Should().Be(expected);
        }

        [Test]
        public void PrintToString_SerializedWithFoundCycle_WhenCycleBetweenObjectsDifferentTypes()
        {
            var human = new Human();
            human.Parent = new Parent {Child = human};

            var actual = ObjectPrinter.PrintToString(human);
            var expected = "Human\r\n\tParent = Parent\r\n\t\tParent = null\r\n\t\tChild = cycle\r\n";

            actual.Should().Be(expected);
        }

        [Test]
        public void PrintToString_SerializedWithFoundCycle_WhenCycleBetweenObjectsSameTypes()
        {
            var parent = new Parent();
            parent.Parent = parent;

            var actual = ObjectPrinter.PrintToString(parent);
            var expected = "Parent\r\n\tParent = cycle\r\n\tChild = null\r\n";

            actual.Should().Be(expected);
        }

        [Test]
        public void PrintToString_SerializedWithArray_WhenPrintingArray()
        {
            var library = new Library();

            var actual = ObjectPrinter.PrintToString(library, x => x.Excluding<List<Book>>()
                .Excluding<Dictionary<string, Book>>());
            var expected = "Library\r\n\tBooksArray = Book[]\r\n\t\tBook\r\n\t\t\t" +
                           "Name = MyBook\r\n\t\t\tAuthor = Alex\r\n";

            actual.Should().Be(expected);
        }

        [Test]
        public void PrintToString_SerializedWithList_WhenPrintingList()
        {
            var library = new Library();

            var actual = ObjectPrinter.PrintToString(library,
                x => x.Excluding<Book[]>().Excluding<Dictionary<string, Book>>());
            var expected = "Library\r\n\tBooksList = List`1\r\n\t\tBook\r\n\t\t\t" +
                           "Name = MyBook\r\n\t\t\tAuthor = Alex\r\n";

            actual.Should().Be(expected);
        }

        [Test]
        public void PrintToString_SerializedWithDictionary_WhenPrintingDictionary()
        {
            var library = new Library();

            var actual = ObjectPrinter.PrintToString(library,
                x => x.Excluding<Book[]>().Excluding<List<Book>>());
            var result = "Library\r\n\tBooksDictionary = Dictionary`2\r\n\t\t" +
                         "KeyValuePair`2\r\n\t\t\tKey = Alex\r\n\t\t\tValue = Book\r\n\t\t\t\t" +
                         "Name = MyBook\r\n\t\t\t\tAuthor = Alex\r\n";

            actual.Should().Be(result);
        }


        [Test]
        public void PrintToString_ThrowException_WhenPrintingIncorrectExpression()
        {
            Assert.Throws<InvalidCastException>(() => ObjectPrinter.PrintToString(person,
                x => x.Printing(y => y.Name + new Guid() + 25).Using(CultureInfo.InvariantCulture)));
        }
    }
}