using System.Globalization;
using NUnit.Framework;
using ObjectPrinterTests.ForSerialization;
using ObjectPrinting.Core;
using ObjectPrinting.Extensions;

namespace ObjectPrinterTests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        [Test]
        public void Demo()
        {
            var person = new Person {Name = "Alex", Age = 19};
            var printer = ObjectPrinter.For<Person>();
            //1. Исключить из сериализации свойства определенного типа
            //2. Указать альтернативный способ сериализации для определенного типа
            //3. Для числовых типов указать культуру
            //4. Настроить сериализацию конкретного свойства
            //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)\
            //6. Исключить из сериализации конкретного свойства
            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию        
            //8. ...с конфигурированием
            var result = printer
                .Excluding<int>().Printing<int>()
                .Using(val => val + "***")
                .Printing<double>()
                .SpecifyCulture(CultureInfo.InvariantCulture)
                .Printing(instance => instance.Name)
                .Using(val => "1" + val + "1")
                .Printing(p => p.Name)
                .TrimmedToLength(3)
                .Excluding(p => p.Id)
                .PrintToString(person);
            var second = person.PrintToString();
        }
    }
}