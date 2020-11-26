using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    public class ObjectPrinterAcceptanceTests
    {
        private Person person;
        private Person person2;
        private Person person3;
        private PrintingConfig<Person> printer;

        [SetUp]
        public void SetUp()
        {
            person = new Person
            {
                Name = "Alex",
                Age = 19,
                Height = 1.85,
                Surname = "Brown"
            };
            person2 = new Person
            {
                Name = "Jame",
                Age = 20,
                Height = 1.92,
                Surname = "Smith"
            };
            person3 = new Person
            {
                Name = "John",
                Age = 14,
                Height = 1.77,
                Surname = "Muller"
            };
            printer = ObjectPrinter.For<Person>();
        }

        [Test]
        public void PrintToString_PrintingAll_IfHasNoConfig()
        {
            var heightToString = person.Height.ToString(CultureInfo.CurrentCulture);
            printer.PrintToString(person).Should().Be("Person\r\n" +
                                                      "\tId = 00000000-0000-0000-0000-000000000000\r\n" +
                                                      "\tName = Alex\r\n" +
                                                      $"\tHeight = {heightToString}\r\n" +
                                                      "\tAge = 19\r\n" +
                                                      "\tFriend = null\r\n" +
                                                      "\tFriend2 = null\r\n" +
                                                      "\tSurname = Brown\r\n" +
                                                      "\tCodes = null\r\n" +
                                                      "\tPasswords = null\r\n");
        }

        [Test]
        public void PrintToString_NotPrintingType_IfTypeExcluded()
        {
            printer = printer.Excluding<int>();
            printer.PrintToString(person).Should().NotContain(nameof(person.Age));
        }

        [Test]
        public void PrintToString_NotPrintingProperty_IfPropertyExcluded()
        {
            printer = printer.Excluding(x => x.Height);
            printer.PrintToString(person).Should().NotContain(nameof(person.Height));
        }

        [Test]
        public void PrintToString_NotPrintingField_IfFieldExcluded()
        {
            printer = printer.Excluding(x => x.Surname);
            printer.PrintToString(person).Should().NotContain(nameof(person.Surname));
        }

        [Test]
        public void PrintToString_PrintingWithCulture_IfCultureWasSet()
        {
            printer = printer.SetCulture<double>(CultureInfo.InvariantCulture);
            printer.PrintToString(person).Should().Contain($"{nameof(person.Height)} = 1.85");
        }

        [Test]
        public void PrintToString_PrintingPropertyByAlternateSerializing()
        {
            printer = printer.SetAlternateSerialize(x => x.Name, x => x + "!!");
            printer.PrintToString(person).Should().Contain($"{nameof(person.Name)} = Alex!!");
        }

        [Test]
        public void PrintToString_PrintingFieldByAlternateSerializing()
        {
            printer = printer.SetAlternateSerialize(x => x.Surname, x => x + "!!");
            printer.PrintToString(person).Should().Contain($"{nameof(person.Surname)} = Brown!!");
        }

        [Test]
        public void PrintToString_PrintingTypeByAlternateSerializing()
        {
            printer = printer.SetAlternateSerialize<int>(x => x + "!!!");
            printer.PrintToString(person).Should().Contain($"{nameof(person.Age)} = 19!!!");
        }

        [Test]
        public void PrintToString_PrintingTrimmedProperty()
        {
            person.Name = "Alexander";
            printer = printer.SetTrimming(x => x.Name, 4);
            printer.PrintToString(person).Should().Contain($"{nameof(person.Name)} = Alex\r\n");
        }

        [Test]
        public void PrintToString_NotThrowsNullReferenceException_IfPropertyIsNull()
        {
            printer = printer
                .SetAlternateSerialize(x => x.Name, x => null)
                .SetTrimming(x => x.Name, 4);
            Action act = () => printer.PrintToString(person);
            act.Should().NotThrow<NullReferenceException>();
        }

        [Test]
        public void PrintToString_PrintingTrimmedField()
        {
            person.Surname = "Brown";
            printer = printer.SetTrimming(x => x.Surname, 1);
            printer.PrintToString(person).Should().Contain($"{nameof(person.Surname)} = B\r\n");
        }

        [Test]
        public void PrintToString_ThrowsSerializationException_IfCircularReference()
        {
            person.Friend = person;
            Action act = () => printer.PrintToString(person);
            act.Should().Throw<SerializationException>();
        }

        [Test]
        public void PrintToString_ThrowsSerializationException_IfCircularReferenceBetween3objects()
        {
            person.Friend = person2;
            person2.Friend = person3;
            person3.Friend = person;
            Action act = () => printer.PrintToString(person);
            act.Should().Throw<SerializationException>();
        }

        [Test]
        public void PrintToString_NotThrowsSerializationException_IfObjectHasSameProperties()
        {
            person.Friend = person2;
            person.Friend2 = person2;
            Action act = () => printer.PrintToString(person);
            act.Should().NotThrow<SerializationException>();
        }

        [Test]
        public void PrintToString_NotThrowsSerializationException_IfListHasSameObjects()
        {
            var list = new List<Person> {person, person};
            Action act = () => ObjectPrinter.For<List<Person>>().PrintToString(list);
            act.Should().NotThrow<SerializationException>();
        }

        [Test]
        public void PrintToString_NotThrowsSerializationException_IfDictHasSameObjects()
        {
            var dictionary = new Dictionary<Person, Person> {[person] = person};
            Action act = () => ObjectPrinter.For<Dictionary<Person, Person>>().PrintToString(dictionary);
            act.Should().NotThrow<SerializationException>();
        }

        [Test]
        public void PrintToString_PrintingArray()
        {
            var array = new[] {0, 1};
            var printer = ObjectPrinter.For<int[]>();
            printer.PrintToString(array).Should().Be("Int32[]\r\n" +
                                                     "[\r\n" +
                                                     "\t0\r\n" +
                                                     "\t1\r\n" +
                                                     "]\r\n");
        }

        [Test]
        public void PrintToString_PrintingList()
        {
            var list = new List<int> {0, 1};
            var printer = ObjectPrinter.For<List<int>>();
            printer.PrintToString(list).Should().Be("List`1\r\n" +
                                                    "[\r\n" +
                                                    "\t0\r\n" +
                                                    "\t1\r\n" +
                                                    "]\r\n");
        }

        [Test]
        public void PrintToString_PrintingObjectWithCollection()
        {
            person.Codes = new List<int> {0, 1};
            printer.PrintToString(person).Should().Contain("\r\n" +
                                                           "\tCodes = List`1\r\n" +
                                                           "\t[\r\n" +
                                                           "\t\t0\r\n" +
                                                           "\t\t1\r\n" +
                                                           "\t]\r\n");
        }

        [Test]
        public void PrintToString_PrintingObjectWithDictionary()
        {
            person.Passwords = new Dictionary<int, int> {[0] = 1};
            printer.PrintToString(person).Should().Contain("\r\n" +
                                                           "\tPasswords = Dictionary`2\r\n" +
                                                           "\t[\r\n" +
                                                           "\t\t{\r\n" +
                                                           "\t\t\tKey = 0\r\n" +
                                                           "\t\t\tValue = 1\r\n" +
                                                           "\t\t}\r\n" +
                                                           "\t]\r\n");
        }

        [Test]
        public void PrintToString_PrintingDict()
        {
            var dictionary = new Dictionary<int, int>
            {
                [0] = 1,
                [2] = 3
            };
            var printer = ObjectPrinter.For<Dictionary<int, int>>();
            printer.PrintToString(dictionary).Should().Be("Dictionary`2\r\n" +
                                                          "[\r\n" +
                                                          "\t{\r\n" +
                                                          "\t\tKey = 0\r\n" +
                                                          "\t\tValue = 1\r\n" +
                                                          "\t}\r\n" +
                                                          "\t{\r\n" +
                                                          "\t\tKey = 2\r\n" +
                                                          "\t\tValue = 3\r\n" +
                                                          "\t}\r\n" +
                                                          "]\r\n");
        }

        [Test]
        public void PrintToString_PrintingDictWithNotFinalType()
        {
            var dictionary = new Dictionary<Person, Person>
            {
                [person] = person2
            };
            var printer = ObjectPrinter.For<Dictionary<Person, Person>>();
            printer.PrintToString(dictionary).Should().Contain("\r\n" +
                                                               "\t\t\tAge = 19\r\n" +
                                                               "\t\t\tFriend = null\r\n");
        }
    }
}