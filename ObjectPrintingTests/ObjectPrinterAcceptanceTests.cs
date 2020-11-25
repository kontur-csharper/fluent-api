using System;
using System.Globalization;
using NUnit.Framework;
using ObjectPrinting;

namespace ObjectPrintingTests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        [Test]
        public void Demo()
        {
            var person = new Person {Name = "Alex", Age = 19};

            var s1 = Printer.PrintToString(person,
                x => x
                    .Excluding<Guid>()
                    .Printing<int>().Using(i => i.ToString("X"))
                    .Printing<double>().Using(CultureInfo.InvariantCulture)
                    .Printing(p => p.Name).TrimmedToLength(10)
                    .Excluding(p => p.Age));

            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию
            var s2 = person.PrintToString();

            //8. ...с конфигурированием
            var s3 = person.PrintToString(s => s.Excluding(p => p.Age));
            Console.WriteLine(s1);
            Console.WriteLine(s2);
            Console.WriteLine(s3);
        }
    }
}