using System;
using System.Globalization;
using NUnit.Framework;
using FluentAssertions;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        private Person firstPerson;
        private Person secondPerson;
        private Person thirdPerson;

        [SetUp]
        public void SetUp()
        {
            firstPerson = new Person { Name = "Alexander", Age = 19, Height = 1.84, Job = "Manager" };
            secondPerson = new Person { Name = "Vladimir", Age = 23, Height = 1.76 };
            thirdPerson = new Person { Name = "Petr", Age = 30, Height = 1.79, Job = "Developer" };
            firstPerson.Friends.Add(secondPerson);
            secondPerson.Friends.Add(firstPerson);
            firstPerson.Friends.Add(thirdPerson);
            secondPerson.Friends.Add(thirdPerson);
            secondPerson.SomeDict[1] = "один";
            secondPerson.SomeDict[2] = "два";
            secondPerson.SomeDict[4] = "четыре";
        }

        
        [Test]
        public void Demo()
        {
            var printer = ObjectPrinter.For<Person>()
                //1. Исключить из сериализации свойства определенного типа
                .Exclude<Guid>()
                //2. Указать альтернативный способ сериализации для определенного типа
                .Printing<double>().Using(number => $"double {number.ToString()}")
                //3. Для числовых типов указать культуру
                .Printing<int>().Using(CultureInfo.InvariantCulture)
                //4. Настроить сериализацию конкретного свойства
                .Printing(p => p.Height).Using(number => $"{number} m")
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                .Printing(p => p.Name).Trimmed(4)
                //6. Исключить из сериализации конкретного свойства
                .Exclude(p => p.Age);
            
            string s1 = printer.PrintToString(firstPerson);

            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию  
            string s2 = firstPerson.PrintToString();
            //8. ...с конфигурированием
            string s3 = firstPerson.PrintToString(p => p.Exclude<Guid>());
            Console.WriteLine(s1);
            Console.WriteLine(s2);
            Console.WriteLine(s3);
        }

        [Test]
        public void PrintToString_ShouldNotContainProperty_WhenPropertyExcluded()
        {
            var printer = ObjectPrinter.For<Person>().Exclude(p => p.Height);
            
            printer.PrintToString(firstPerson).Should().NotContain("Height = ");
        }
        
        [Test]
        public void PrintToString_ShouldNotContainProperty_WhenTypeExcluded()
        {
            var printer = ObjectPrinter.For<Person>().Exclude<double>();
            
            printer.PrintToString(firstPerson).Should().NotContain("Height = ");
            printer.PrintToString(firstPerson).Should().NotContain("Weight = ");
        }
        
        [Test]
        public void PrintToString_ShouldCorrectWriteProperty_WhenTypeSerializationChanged()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<double>().Using(p=> $"double {p}");
            
            printer.PrintToString(firstPerson).Should().Contain($"Height = double {firstPerson.Height}");
            printer.PrintToString(firstPerson).Should().Contain($"Weight = double {firstPerson.Weight}");
        }
        
        [Test]
        public void PrintToString_ShouldCorrectWriteProperty_WhenPropertySerializationChanged()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing(p => p.Height).Using(p=> $"double {p}");
            
            printer.PrintToString(firstPerson).Should().Contain($"Height = double {firstPerson.Height}");
            printer.PrintToString(firstPerson).Should().Contain($"Weight = {firstPerson.Weight}");
        }
        
        [Test]
        public void PrintToString_ShouldCorrectWriteProperty_WhenTypeCultureSet()
        {
            var russianCulture = new CultureInfo("ru-RU", false);
            var invariantPrinter = ObjectPrinter.For<Person>()
                .Printing<double>().Using(CultureInfo.InvariantCulture);
            var russianPrinter = ObjectPrinter.For<Person>()
                .Printing<double>().Using(russianCulture);
            firstPerson.Weight = 1.23;
            invariantPrinter.PrintToString(firstPerson).Should().Contain("Weight = 1.23");
            russianPrinter.PrintToString(firstPerson).Should().Contain("Weight = 1,23");
        }
        
        [Test]
        public void PrintToString_ShouldCorrectWriteProperty_WhenTrimmed()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing(p => p.Job).Trimmed(3);
            
            printer.PrintToString(firstPerson).Should().Contain("Job = Dev" + Environment.NewLine);
        }
        
        [Test]
        public void ObjectPrinter_ShouldBeImmutable()
        {
            var firstPrinter = ObjectPrinter.For<Person>();
            var secondPrinter = firstPrinter.Exclude<double>();
            var thirdPrinter = firstPrinter.Exclude<int>();
            
            firstPrinter.PrintToString(firstPerson).Should().Contain("Age = ");
            firstPrinter.PrintToString(firstPerson).Should().Contain("Height = ");
            secondPrinter.PrintToString(firstPerson).Should().Contain("Age = ");
            secondPrinter.PrintToString(firstPerson).Should().NotContain("Height = ");
            thirdPrinter.PrintToString(firstPerson).Should().NotContain("Age = ");
            thirdPrinter.PrintToString(firstPerson).Should().Contain("Height = ");
        }
    }
}