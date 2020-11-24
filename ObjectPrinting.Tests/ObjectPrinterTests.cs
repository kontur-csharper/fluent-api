using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting.TestObjects;
using ObjectPrinting.Tests.TestObjects;

namespace ObjectPrinting.Tests
{
    public class ObjectPrinterTests
    {
        private Person Person { get; set; }
        private Person SecondPerson { get; set; }
        private Person ThirdPerson { get; set; }
        private Person FourthPerson { get; set; }

        [SetUp]
        public void SetUp()
        {
            Person = new Person {Name = "Alex", Age = 19, Height = 1.73, Id = Guid.Empty};
            SecondPerson = new Person {Name = "Martin", Age = 18, Height = 0.6};
            ThirdPerson = new Person {Name = "Melman", Age = 20, Height = 3.0};
            FourthPerson = new Person {Name = "Gloria", Age = 5, Height = 0.2};
        }

        [Test]
        public void Demo()
        {
            var printer = ObjectPrinter.For<Person>()
                //1. Исключить из сериализации свойства определенного типа
                .Exclude(typeof(string))
                //2. Указать альтернативный способ сериализации для определенного типа
                .Printing<string>().Using(x => x + ".0")
                //3. Для всех типов, имеющих культуру, есть возможность ее указать
                .SetCultureInfo<int>(CultureInfo.CurrentCulture)
                //4. Настроить сериализацию конкретного свойства
                .SelectProperty(properties => properties.Name).Using(name => $"<em>{name}</em>")
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                .SelectProperty(properties => properties.Name).Trimmed(2)
                //6. Исключить из сериализации конкретного свойства
                .Exclude(properties => properties.Id);

            var peronSerialization = printer.PrintToString(Person);
            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию
            var secondPeronSerialization = SecondPerson.PrintToString();
            //8. ...с конфигурированием
            var thirdPeronSerialization =
                ThirdPerson.PrintToString(config => config.SelectProperty(x => x.Name).Trimmed(4).Exclude(x => x.Id));
        }



        [Test]
        public void ReturnsRightString_WhenIntegerExcluded()
        {
            var expectedResult = $"Person\r\n\tId = {Person.Id}\r\n\tName = Alex\r\n\tHeight = {Person.Height}\r\n\tParents = empty\r\n";
            var printer = ObjectPrinter.For<Person>().Exclude(typeof(int));

            printer.PrintToString(Person).Should().Be(expectedResult);
        }

        [Test]
        public void ReturnsRightString_WhenStringExcluded()
        {
            var expectedResult = $"Person\r\n\tId = {Person.Id}\r\n\tHeight = {Person.Height}\r\n\tAge = 19\r\n\tParents = empty\r\n";
            var printer = ObjectPrinter.For<Person>().Exclude(typeof(string));

            printer.PrintToString(Person).Should().Be(expectedResult);
        }

        [Test]
        public void ReturnsRightString_WhenObjectExcluded()
        {
            var expectedResult = $"Person\r\n\tName = Alex\r\n\tHeight = {Person.Height}\r\n\tAge = 19\r\n\tParents = empty\r\n";
            var printer = ObjectPrinter.For<Person>().Exclude(typeof(Guid));

            printer.PrintToString(Person).Should().Be(expectedResult);
        }

        [Test]
        public void ReturnsRightString_WhenChangedIntegerSerialization()
        {
            var expectedResult =
                $"Person\r\n\tId = {Person.Id}\r\n\tName = Alex\r\n\tHeight = {Person.Height}\r\n\tAge = 19.00\r\n\tParents = empty\r\n";
            var printer = ObjectPrinter.For<Person>().Printing<int>().Using(x => x + ".00");

            printer.PrintToString(Person).Should().Be(expectedResult);
        }

        [Test]
        public void ReturnsRightString_WhenChangedDoubleCulture()
        {
            var expectedResult =
                $"Person\r\n\tId = {Person.Id}\r\n\tName = Alex\r\n\tHeight = 1,73\r\n\tAge = 19\r\n\tParents = empty\r\n";
            var printer = ObjectPrinter.For<Person>()
                .SetCultureInfo<double>(CultureInfo.CreateSpecificCulture("fr-FR"));

            printer.PrintToString(Person).Should().Be(expectedResult);
        }

        [Test]
        public void ReturnsRightString_WhenChangedPropertySerialization()
        {
            var expectedResult =
                $"Person\r\n\tId = {Person.Id}\r\n\tName = <em>Alex</em>\r\n\tHeight = {Person.Height}\r\n\tAge = 19\r\n\tParents = empty\r\n";
            var printer = ObjectPrinter.For<Person>()
                .SelectProperty(x => x.Name).Using(x => $"<em>{x}</em>");

            printer.PrintToString(Person).Should().Be(expectedResult);
        }

        [Test]
        public void ReturnsRightString_WhenTrimmedStringProperty()
        {
            var expectedResult =
                $"Person\r\n\tId = {Person.Id}\r\n\tName = Al\r\n\tHeight = {Person.Height}\r\n\tAge = 19\r\n\tParents = empty\r\n";
            var printer = ObjectPrinter.For<Person>()
                .SelectProperty(x => x.Name).Trimmed(2);

            printer.PrintToString(Person).Should().Be(expectedResult);
        }

        [Test]
        public void ReturnsRightString_WhenPropertyExcluded()
        {
            var expectedResult =
                $"Person\r\n\tName = Alex\r\n\tHeight = {Person.Height}\r\n\tAge = 19\r\n\tParents = empty\r\n";
            var printer = ObjectPrinter.For<Person>().Exclude(x => x.Id);

            printer.PrintToString(Person).Should().Be(expectedResult);
        }

        [Test]
        public void ReturnRightString_WhenDefaultPrintToString()
        {
            var expectedResult =
                $"Person\r\n\tId = {SecondPerson.Id}\r\n\tName = Martin\r\n\tHeight = {SecondPerson.Height}\r\n\tAge = 18\r\n\tParents = empty\r\n";


            SecondPerson.PrintToString().Should().Be(expectedResult);
        }

        [Test]
        public void ReturnRightString_WhenPrintToStringWithParameters()
        {
            var expectedResult =
                $"Person\r\n\tName = Me\r\n\tHeight = {ThirdPerson.Height}\r\n\tAge = 20\r\n\tParents = empty\r\n";

            ThirdPerson.PrintToString(config => config.SelectProperty(x => x.Name).Trimmed(4).Exclude(x => x.Id))
                .Should()
                .Be(expectedResult);
        }

        [Test]
        public void ReturnRightString_WhenSerializeList()
        {
            const string expectedResult =
                "Person\r\n\tName = Gloria\r\n\tAge = 5\r\n\tParents = {\r\nPerson\r\n\t\t\tName = Martin\r\n\t\t\tAge = 18\r\n\t\t\tParents = empty\r\n\t}\r\n";
            FourthPerson.Parents.Add(SecondPerson);

            FourthPerson.PrintToString(x => x.Exclude(typeof(Guid), typeof(double))).Should().Be(expectedResult);
        }

        [Test]
        public void ReturnRightString_WhenSerializeDictionary()
        {
            const string expectedResult =
                "House\r\n\tApartmentOwner = {\r\n1\r\n : Oleg\r\n13\r\n : GoodWork\r\n\t}\r\n\tApartments = null\r\n";
            var house = new House();
            house.ApartmentOwner.Add(1, "Oleg");
            house.ApartmentOwner.Add(13, "GoodWork");

            house.PrintToString().Should().Be(expectedResult);
        }

        [Test]
        public void ReturnRightString_WhenSerializeArray()
        {
            const string expectedResult = "House\r\n\tApartments = {\r\n1\r\n2\r\n4\r\n13\r\n\t}\r\n";
            var house = new House {Apartments = new[] {1, 2, 4, 13}};

            house.PrintToString(x => x.Exclude(y => y.ApartmentOwner)).Should().Be(expectedResult);
        }

        [Test]
        public void ReturnRightString_WhenCycleAppear()
        {
            const string expectedResult =
                "Person\r\n\tName = Alex\r\n\tParents = {\r\nPerson\r\n\t\t\tName = Melman\r\n\t\t\tParents = {\r\nFall in cycle\r\n\t\t\t}\r\n\t}\r\n";
            Person.Parents.Add(ThirdPerson);
            ThirdPerson.Parents.Add(Person);

            Person.PrintToString(x => x.Exclude(typeof(int), typeof(double), typeof(Guid))).Should().Be(expectedResult);
        }

        [Test]
        public void ReturnRightString_WhenExcludedCollection()
        {
            const string expectedResult = "House\r\n\tApartments = null\r\n";
            var house = new House();

            house.PrintToString(x => x.Exclude(typeof(Dictionary<int, string>))).Should().Be(expectedResult);
        }

        [Test]
        public void ReturnRightString_WhenChangeAnotherPerson()
        {
            var expectedResult = new StringBuilder();
            expectedResult.Append("AnotherPerson\r\n\tAge = 13%\r\n\tParents = {\r\nPerson\r\n\t\t\tId = ");
            expectedResult.Append($"{Person.Id}\r\n\t\t\tName = Alex\r\n\t\t\tHeight = {Person.Height}");
            expectedResult.Append("\r\n\t\t\tAge = 19\r\n\t\t\tParents = empty\r\n\t}\r\n");

            var person = new AnotherPerson {Name = "Nikita", Age = 13, Parents = {Person}};
            var printer = ObjectPrinter.For<AnotherPerson>()
                .Exclude(x => x.Name)
                .Printing<int>().Using(x => x + "%")
                .Exclude(typeof(Guid), typeof(double));

            printer.PrintToString(person).Should().Be(expectedResult.ToString());
        }

        [Test]
        public void ReturnRightString_WhenObjectInCollection()
        {
            var expectedResult = new StringBuilder();
            expectedResult.Append(
                "City\r\n\tName = Msk\r\n\tRoadsToCity = {\r\n1\r\n : City\r\n\t\t\tName = Ekb\r\n\t\t\tRoadsToCity = empty\r\n\t}\r\n");
            var printer = ObjectPrinter.For<City>().Exclude(typeof(int));
            var city = new City {Name = "Msk", Population = 13};
            var secondCity = new City {Name = "Ekb", Population = 2};
            city.RoadsToCity.Add(1, secondCity);

            printer.PrintToString(city).Should().Be(expectedResult.ToString());
        }
    }
}