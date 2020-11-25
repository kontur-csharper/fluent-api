using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using ObjectPrinting.Extensions;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        [Test]
        public void Demo()
        {
            var spouse = new Person
            {
                Id = Guid.NewGuid(),
                Name = "Jane",
                Surname = "Merser",
                Age = 38,
                BirthDay = DateTime.Today.Date,
                Spouse = null
            };
            var person = new Person
            {
                Id = Guid.NewGuid(),
                Name = "Alex",
                Surname = "Merser",
                Age = 19,
                Spouse = spouse,
                Height = 1.75,
                Family = new List<Person>
                {
                    new Person {Name = "Rachel", Surname = "Merser", Id = Guid.NewGuid(), Height = 1.8},
                    new Person {Id = Guid.NewGuid(), Age = 50, Family = new List<Person> {new Person()}}
                }
            };

            var printer = ObjectPrinter.For<Person>()
                .Excluding<int>()
                .Printing<string>().Using(p => $"это имя: {p}")
                .Printing<double>().Using(CultureInfo.InvariantCulture)
                .Printing(p => p.Surname).Using(p => $"это фамилия: {p}")
                .Printing(p => p.Surname).TrimmedToLength(11)
                .Excluding(p => p.Age);

            var s1 = printer.PrintToString(person);
            var s2 = person.PrintToString();
            var s3 = person.PrintToString(c => c.Excluding<Guid>());

            Console.WriteLine(s1);
            Console.WriteLine(s2);
            Console.WriteLine(s3);
        }
    }
}