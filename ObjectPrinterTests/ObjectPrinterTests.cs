using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting;

namespace ObjectPrinterTests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        [SetUp]
        public void SetUp()
        {
            person = new Person {Id = Guid.Empty, Name = "Alex", Age = 19, Height = 193.2, money = 0};
        }
        
        private Person person;

        [Explicit]
        [Test]
        [SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
        public void Demo()
        {
            var printer = ObjectPrinter.For<Person>()
                //1. Исключить из сериализации свойства определенного типа
                .Excluding<Guid>()
                //2. Указать альтернативный способ сериализации для определенного типа
                .Printing<int>().Using(i => i + "XXXX")
                //3. Для числовых типов указать культуру
                .Printing<double>().Using(CultureInfo.InvariantCulture)
                //4. Настроить сериализацию конкретного свойства
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                //.Printing(p => p.Name).TrimmedToLength(10)
                .Printing<string>().TrimmedToLength(5)
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

        [Explicit]
        [Test]
        public void Collections_Demo()
        {
            var printer = ObjectPrinter.For<Person>()
                .Excluding<Guid>()
                .Printing<int>().Using(i => i + "XXXX")
                .Printing(p => p.Age).Using(i => i + " years old")
                .Printing<double>().Using(CultureInfo.InvariantCulture);

            var pers1 = new Person {Age = 23, Height = 100.5, Name = "Alex"};
            var pers2 = new Person {Age = 13, Height = 150.5, Name = "Misha"};
            var pers3 = new Person {Age = 26, Height = 200.5, Name = "Vasya"};

            var personList = new List<Person> {pers1, pers2, pers3};

            Console.WriteLine(printer.PrintToString(personList));
            var arr = new[] {pers1, pers2, pers3};
            Console.WriteLine(printer.PrintToString(arr));
        }

        [Explicit]
        [Test]
        public void Dictionary_Demo()
        {
            var printer = ObjectPrinter.For<Person>()
                .Excluding<Guid>()
                .Printing(p => p.Age).Using(i => i + " years old")
                .Printing<double>().Using(CultureInfo.InvariantCulture);
            
            var pers1 = new Person {Age = 23, Height = 100.5, Name = "Alex"};
            var pers2 = new Person {Age = 13, Height = 150.5, Name = "Misha"};
            var pers3 = new Person {Age = 26, Height = 200.5, Name = "Vasya"};

            var dic1 = new Dictionary<int, Person> {[1] = pers1, [2] = pers2, [3] = pers3};
            var dic2 = new Dictionary<string, Person> {[pers1.Name] = pers1, [pers2.Name] = pers2, [pers3.Name] = pers3};
            
            Console.WriteLine(printer.PrintToString(dic1));
            Console.WriteLine(printer.PrintToString(dic2));
        }
        
        

        [Test]
        public void PrintToString_PersonIdDoesNotPrint_ExcludingGuid()
        {
            var printer = ObjectPrinter.For<Person>().Excluding<Guid>();

            Assert.That(printer.PrintToString(person), Is.EqualTo("Person\r\n\tName = Alex\r\n\tHeight = 193,2\r\n\tAge = 19\r\n\tmoney = 0\r\n"));
        }

        [Test]
        public void PrintToString_ModifiedInt_PrintingWithXAtTheEnd()
        {
            var printer = ObjectPrinter.For<int>().Printing<int>().Using(i => i + "X");

            Assert.That(printer.PrintToString(5), Is.EqualTo("5X"));
        }

        [Test]
        public void PrintToString_PrintingDoubleInInvariantCulture_PrintingUsingInvariantCulture()
        {
            var printer = ObjectPrinter.For<double>().Printing<double>().Using(CultureInfo.InvariantCulture);

            Assert.That(printer.PrintToString(2.5), Is.EqualTo("2.5"));
        }

        [Test]
        public void PrintToString_PrintedTrimmedStr_UsingTrimmingToLen()
        {
            var printer = ObjectPrinter.For<string>().Printing<string>().TrimmedToLength(3);
            
            Assert.That(printer.PrintToString("abcdef"), Is.EqualTo("abc"));
        }
        
        [Test]
        public void PrintToString_ShouldNotThrow_TrimmedToLenThatIsBiggerThanTextLen()
        {
            var printer = ObjectPrinter.For<string>().Printing<string>().TrimmedToLength(5);
            
            Assert.DoesNotThrow(() => printer.PrintToString("abc"));
            Assert.That(printer.PrintToString("abc"), Is.EqualTo("abc"));
        }

        [Test]
        public void PrintToString_PrintingDateTimeInInvariantCulture_PrintingUsingInvariantCulture()
        {
            var printer = ObjectPrinter.For<DateTime>().Printing<DateTime>().Using(CultureInfo.InvariantCulture);

            Assert.That(printer.PrintToString(DateTime.MinValue), Is.EqualTo(DateTime.MinValue.ToString(CultureInfo.InvariantCulture)));
        }

        [Test]
        public void PrintToString_PersonIdDoesNotPrint_ExcludingPersonId()
        {
            var printer = ObjectPrinter.For<Person>().Excluding(p => p.Id);

            Assert.That(printer.PrintToString(person), Is.EqualTo("Person\r\n\tName = Alex\r\n\tHeight = 193,2\r\n\tAge = 19\r\n\tmoney = 0\r\n"));
        }

        [Test]
        [Timeout(1000)]
        public void PrintToString_ShouldNotThrow_SelfReference()
        {
            var personWithFriend = new PersonWithFriend {Name = "Alex", Age = 19};
            personWithFriend.Friend = personWithFriend;

            Console.WriteLine(personWithFriend.PrintToString());
        }
        
        [Test]
        [Timeout(1000)]
        public void PrintToString_ShouldNotThrow_CyclicReference()
        {
            var personWithFriend1 = new PersonWithFriend {Name = "Alex", Age = 19};
            var personWithFriend2 = new PersonWithFriend {Name = "Petr", Age = 19};
            personWithFriend1.Friend = personWithFriend2;
            personWithFriend2.Friend = personWithFriend1;

            Console.WriteLine(personWithFriend1.PrintToString());
        }

        [Test]
        public void PrintRoString_PrintedEachObjectInCollection_PrintingList()
        {
            var printer = ObjectPrinter.For<int>();
            var list = new List<int> {1, 2, 3};
            
            Assert.That(printer.PrintToString(list), Is.EqualTo("List`1\r\n1\r\n2\r\n3\r\n"));
            Console.WriteLine(printer.PrintToString(list));
        }

        [Test]
        public void PrintRoString_PrintedEachObjectInCollection_PrintingArray()
        {
            var printer = ObjectPrinter.For<int>();
            var arr = new [] {1, 2, 3};
            
            Assert.That(printer.PrintToString(arr), Is.EqualTo("Int32[]\r\n1\r\n2\r\n3\r\n"));
            Console.WriteLine(printer.PrintToString(arr));
        }

        [Test]
        public void PrintRoString_PrintedEachObjectInCollection_PrintingDictionary()
        {
            var printer = ObjectPrinter.For<string>();
            var dic = new Dictionary<int, string>
            {
                {1, "a"},
                {2, "b"},
                {3, "c"},
            };

            Assert.That(printer.PrintToString(dic), Is.EqualTo("Dictionary`2\r\n1 : a\r\n2 : b\r\n3 : c\r\n"));
            Console.WriteLine(printer.PrintToString(dic));
        }
    }
}