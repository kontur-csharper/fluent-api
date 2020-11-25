using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting;
using ObjectPrinting.Tests;

namespace Tests
{
    [TestFixture]
    public class AcceptanceTests
    {
        [Test]
        public void Demo()
        {
            var person = new Person {Name = "Alex", Age = 19};

            ObjectPrinter.For<Person>()
                //1. Исключить из сериализации свойства определенного типа
                .Exclude<int>()
                //2. Указать альтернативный способ сериализации для определенного типа
                .TypeSerializer<double>(d => $"My beautiful shiny double: {d}")
                //3. Для числовых типов указать культуру
                .Choose(o => o.Age).SetCulture(CultureInfo.CurrentCulture)
                //4. Настроить сериализацию конкретного свойства
                .Choose(o => o.Id).UseSerializer(i => i.ToString())
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                .Choose(o => o.Name).Trim(2)
                //6. Исключить из сериализации конкретного свойства
                .Choose(o => o.Height).Exclude()
                .PrintToString(person);

            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию        
            var string1 = person.PrintToString();
            //8. ...с конфигурированием
            var string2 = person.PrintToString(d => d.Exclude<int>());
        }

        [Test]
        public void PrintToString_ShouldNotContainProperty_WhenPropertyExcluded()
        {
            var printer = ObjectPrinter.For<Person>().Exclude<int>();
            var person = new Person {Name = "Alexander", Age = 19, Height = 1.84};

            var personText = printer.PrintToString(person);

            personText.Should().NotContain("Age = ");
        }

        [Test]
        public void PrintToString_ShouldContainNewSerialization_AddedSerializationForType()
        {
            var printer = ObjectPrinter.For<Person>()
                .TypeSerializer<double>(d => $"My beautiful shiny double: {d}");
            var person = new Person {Name = "Alexander", Age = 19, Height = 1.84};

            var personText = printer.PrintToString(person);

            personText.Should().Contain("My beautiful shiny double:");
        }

        [Test]
        public void PrintToString_ShouldContainCorrectCulture_AddedCultureForProperty()
        {
            var printer = ObjectPrinter.For<Person>()
                .Choose(o => o.Height).SetCulture(new CultureInfo("ru-RU", false));
            var person = new Person {Name = "Alexander", Age = 19, Height = 44.5};

            var personText = printer.PrintToString(person);

            personText.Should().Contain("44,5");
        }

        [Test]
        public void PrintToString_ShouldNotContainProperty_ExcludeProperty()
        {
            var printer = ObjectPrinter.For<Person>()
                .Choose(o => o.Age).Exclude();
            var person = new Person {Name = "Alexander", Age = 19, Height = 1.84};

            var personText = printer.PrintToString(person);

            personText.Should().NotContain("Age = ");
        }

        [Test]
        public void PrintToString_ShouldContainSpecialFieldSerialization_PropertySerialization()
        {
            var printer = ObjectPrinter.For<Person>()
                .Choose(o => o.Name).UseSerializer(d => $"The name is: {d}");
            var person = new Person {Name = "Alexander", Age = 19, Height = 1.84};

            var personText = printer.PrintToString(person);

            personText.Should().Contain("The name is");
        }

        [Test]
        public void PrintToString_ShouldContainTrimmedString_TrimString()
        {
            var printer = ObjectPrinter.For<Person>()
                .Choose(o => o.Name).Trim(4);

            var person = new Person {Name = "Alexander", Age = 19, Height = 1.84};

            var personText = printer.PrintToString(person);

            personText.Should().NotContain("ander");
        }

        [Test]
        public void PrintToString_ShouldContainSonField_AddAnotherClass()
        {
            var subPerson = new Person {Name = "Ivan", Age = 10};
            var printer = ObjectPrinter.For<Person>();
            var person = new Person {Name = "Alexander", Age = 19, Height = 1.84, Son = subPerson};

            var personText = printer.PrintToString(person);

            personText.Should().Contain("Ivan");
        }

        [Test]
        public void PrintToString_ShouldNotContainSon_RemoveSon()
        {
            var subPerson = new Person {Name = "Ivan", Age = 10};
            var printer = ObjectPrinter.For<Person>()
                .Choose(o => o.Son).Exclude();
            var person = new Person {Name = "Alexander", Age = 19, Height = 1.84, Son = subPerson};

            var personText = printer.PrintToString(person);

            personText.Should().NotContain("Ivan");
        }

        [Test]
        public void PrintToString_ShouldContainFamily_DictionaryFamily()
        {
            var subPersons = new Dictionary<string, Person> {{"Артем", new Person {Name = "Артем", Age = 63}}};
            var printer = ObjectPrinter.For<Person>()
                .Choose(o => o.Son).Exclude();
            var person = new Person {Name = "Alexander", Age = 19, Height = 1.84, Family = subPersons};

            var personText = printer.PrintToString(person);

            personText.Should().Contain("Артем");
        }

        [Test]
        public void PrintToString_ShouldContainFamilyNames_ListWithNames()
        {
            var subPersons = new List<string> {"Sergey", "Rodion"};
            var printer = ObjectPrinter.For<Person>()
                .Choose(o => o.Son).Exclude();
            var person = new Person {Name = "Alexander", Age = 19, Height = 1.84, FamilyNames = subPersons};

            var personText = printer.PrintToString(person);

            personText.Should().Contain("Sergey");
        }

        [Test]
        public void PrintToString_CorrectWorkWithCycles_CyclesElements()
        {
            var subPerson = new Person {Name = "Oleg"};
            var printer = ObjectPrinter.For<Person>();
            var person = new Person {Name = "Alexander", Age = 19, Height = 1.84, Son = subPerson};
            subPerson.Son = person;

            var personText = printer.PrintToString(person);

            personText.Should().Contain("This element has been already added");
        }
    }
}