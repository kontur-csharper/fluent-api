﻿using System;
using System.Globalization;
using System.Collections.Generic;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        [Test]
        public void Demo()
        {
            var person = new Person { Name = "Alex", Age = 19, SecondName = "Shmalex" };

            var printer = ObjectPrinter.For<Person>()
                //1. Исключить из сериализации свойства определенного типа
                .Excluding<Guid>()
                //2.Указать альтернативный способ сериализации для определенного типа
                .ChangePrintFor<string>().Using(s => s.Trim())
                //3. Для числовых типов указать культуру
                .ChangePrintFor<int>().Using(CultureInfo.CurrentCulture)
                //4. Настроить сериализацию конкретного свойства
                .ChangePrintFor(p => p.Name).Using(value => value.ToUpper())
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                .ChangePrintFor(p => p.Name).TrimToLength(5)
                //6. Исключить из сериализации конкретного свойства
                .Excluding(p => p.Name);

            string s1 = printer.PrintToString(person);
            Console.Write(s1);

            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию    
            //8. ...с конфигурированием
        }

        [Test]
        public void Excluding_Type()
        {
            var person = new Person { Name = "Alex", Age = 19 };
            var printer = ObjectPrinter.For<Person>()
                .Excluding<Guid>();

            string s1 = printer.PrintToString(person);

            Assert.AreEqual("Person\r\n\tName = Alex\r\n\tHeight = 0\r\n\tAge = 19\r\n\tSecondName = null\r\n", s1);
        }

        [Test]
        public void ChangePrintFor_Type_Using_Function()
        {
            var person = new Person { Name = "Alex", Age = 19 };
            var printer = ObjectPrinter.For<Person>()
                .ChangePrintFor<int>().Using(s => (s * 100).ToString());

            string s1 = printer.PrintToString(person);

            Assert.AreEqual("Person\r\n\tId = Guid\r\n\tName = Alex\r\n\tHeight = 0\r\n\tAge = 1900\tSecondName = null\r\n", s1);
        }

        [Test]
        public void ChangePrintFor_Int_Using_CultureInfo()
        {
            var person = new Person { Name = "Alex", Age = 19 };
            var printer = ObjectPrinter.For<Person>()
                .ChangePrintFor<int>().Using(CultureInfo.CurrentCulture);

            string s1 = printer.PrintToString(person);

            Assert.AreEqual("Person\r\n\tId = Guid\r\n\tName = Alex\r\n\tHeight = 0\r\n\tAge = ru-RU\tSecondName = null\r\n", s1);
        }

        [Test]
        public void ChangePrintFor_Function_Using_Function()
        {
            var person = new Person { Name = "Alex", Age = 19 , SecondName = "Shmalex"};
            var printer = ObjectPrinter.For<Person>()
                .ChangePrintFor(p => p.Name).Using(value => value.ToUpper());

            string s1 = printer.PrintToString(person);

            Assert.AreEqual("Person\r\n\tId = Guid\r\n\tName = ALEX\tHeight = 0\r\n\tAge = 19\r\n\tSecondName = Shmalex\r\n", s1);
        }

        [Test]
        public void ChangePrintFor_String_TrimToLength()
        {
            var person = new Person { Name = "Alex", Age = 19, SecondName = "Shmalex" };
            var printer = ObjectPrinter.For<Person>()
                .ChangePrintFor<string>().TrimToLength(3);

            string s1 = printer.PrintToString(person);

            Assert.AreEqual("Person\r\n\tId = Guid\r\n\tName = Ale\tHeight = 0\r\n\tAge = 19\r\n\tSecondName = Shm", s1);
        }

        [Test]
        public void Excluding_Property()
        {
            var person = new Person { Name = "Alex", Age = 19, SecondName = "Shmalex" };
            var printer = ObjectPrinter.For<Person>().Excluding(p => p.SecondName);

            string s1 = printer.PrintToString(person);

            Assert.AreEqual("Person\r\n\tId = Guid\r\n\tName = Alex\r\n\tHeight = 0\r\n\tAge = 19\r\n", s1);
        }

        [Test]
        public void PrintToString_List()
        {
            var list = new List<int>() { 1, 2, 3 };
            var printer = ObjectPrinter.For<List<int>>();

            string s1 = printer.PrintToString(list);

            Assert.AreEqual("List`1\r\n\t1\r\n\t2\r\n\t3\r\n", s1);
        }

        [Test]
        public void PrintToString_Dictionary()
        {
            var list = new Dictionary<int, string>() {
                { 1 , "a" },
                { 2 , "b" },
                { 3 , "c" }};
            var printer = ObjectPrinter.For<Dictionary<int, string>>();

            string s1 = printer.PrintToString(list);

            Assert.AreEqual("Dictionary`2\r\n\t" +
                "KeyValuePair`2\r\n\t\t\tKey = 1\r\n\t\t\tValue = a\r\n\t" +
                "KeyValuePair`2\r\n\t\t\tKey = 2\r\n\t\t\tValue = b\r\n\t" +
                "KeyValuePair`2\r\n\t\t\tKey = 3\r\n\t\t\tValue = c\r\n", s1);
        }
    }
}