using System;
using System.Globalization;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        [Test]
        public void Demo()
        {
            var person = new Person
            {
                Name = "Alex", //ммя ему - Алексей...
                Age = 19, //...и возраст его - 19 долларов...
                Father = new Person
                {
                    Name = "John",
                    Mother = new Person
                    {
                        Name = "Jessieieieieieieieieieieieieie",
                        Height = 1.1,
                        Mother = new Person
                        {
                            Name = "someone",
                            Height = 336.333,
                            Father = new Person
                            {
                                Name = "???",
                                Age = 2000,
                                Height = 875.375
                            }
                        },
                    }
                }
            };

            var printer = ObjectPrinter.For<Person>()
                //1. Исключить из сериализации свойства определенного типа
                .Excluding<Guid>()
                //2. Указать альтернативный способ сериализации для определенного типа
                .Printing<string>().As(s => $"<<{s}>>")
                .Printing<DateTime>().As(d => $"year: {d.Year}, the rest is mistery...")
                //3. Для числовых типов указать культуру
                .Printing<double>().Using(CultureInfo.InvariantCulture)
                .Printing(p => p.Father.Mother.Mother.Father.Height).Using("f10")
                .Printing(p => p.Age).Using("C", CultureInfo.CreateSpecificCulture("en-US"))
                //4. Настроить сериализацию конкретного свойства
                .Printing(p => p.Name).As(p => "Алексей")
                .Printing(p => p.Father.Mother.Mother.Father.Name).As(name => $"who is that?{name}")
                .Printing(p => p.Father.Mother).Using(config => config
                    .Printing<DateTime>().As(d => $"...but not here: {d}"))
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                .Printing<string>().Truncated(5)
                .Printing(p => p.Father.Mother.Name).Truncated(10)
                //6. Исключить из сериализации конкретного свойства
                .Excluding(p => p.Mother)
                .Excluding(p => p.Father.Father);

            var s1 = printer.PrintToString(person);
            Console.WriteLine(s1);
            Console.WriteLine("--------------------------------");

            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию        
            //8. ...с конфигурированием
            Console.WriteLine(person.PrintToString(printer.Excluding(p => p.Father.Mother)));
            Console.WriteLine("--------------------------------");
            Console.WriteLine(person.PrintToString(c => c
                .Excluding<Guid>()
                .Printing<Person>().As(p => "...no! Enough recursive printing!")
                .Printing(p => p.SomeArray).As(a => $"int[{a.Length}] [...]")
                .Excluding(p => p.SomeDict)
                .Printing<DateTime>().Using(CultureInfo.InvariantCulture)));
        }
    }
}