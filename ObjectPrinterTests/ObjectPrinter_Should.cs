using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using ObjectPrinterTests.ForSerialization;
using ObjectPrinting.Core;
using FluentAssertions;
using ObjectPrinting.Extensions;

namespace ObjectPrinterTests
{
    [TestFixture]
    public class ObjectPrinter_Should
    {
        private PrintingConfig<Person> _personPrinter;

        [SetUp]
        public void Setup()
        {
            _personPrinter = ObjectPrinter.For<Person>();
        }

        [Test]
        public void Excluding_ThrowException_WhenNotMemberExpression()
        {
            var act = new Action(() => _personPrinter.Excluding(person => person.Age + 1));

            act.Should().Throw<Exception>();
        }

        [Test]
        public void Excluding_NotException_WhenMemberExpression()
        {
            var act = new Action(() => _personPrinter.Excluding(person => person.Age));

            act.Should().NotThrow<Exception>();
        }

        [Test]
        public void Printing_ThrowException_WhenNotMemberExpression()
        {
            var act = new Action(() => _personPrinter.Printing(person => person.Age + 1));

            act.Should().Throw<Exception>();
        }

        [Test]
        public void Printing_NotException_WhenMemberExpression()
        {
            var act = new Action(() => _personPrinter.Printing(person => person.Age));

            act.Should().NotThrow<Exception>();
        }

        [Test]
        public void PrintToString_SimpleSerializedText_WhenNotConfiguration()
        {
            var act = _personPrinter.PrintToString(Instances.Person);

            act.Should()
                .Be(
                    $"Person\r\n\tId = {Guid.Empty.ToString(null, CultureInfo.CurrentCulture)}\r\n\t" +
                    $"Name = Danil\r\n\tHeight = {160.5.ToString(CultureInfo.CurrentCulture)}\r\n\t" +
                    $"Age = {15.ToString(CultureInfo.CurrentCulture)}\r\n\t" +
                    $"Weight = {67.778.ToString(CultureInfo.CurrentCulture)}\r\n\tBirthPlace = null\r\n");
        }

        [Test]
        public void PrintToString_StringWithoutCertainTypeMembers_WhenExcludingMembersType()
        {
            var act = _personPrinter
                .Excluding<double>()
                .PrintToString(Instances.Person);

            act.Should()
                .Be(
                    $"Person\r\n\tId = {Guid.Empty.ToString(null, CultureInfo.CurrentCulture)}\r\n\tName = Danil" +
                    $"\r\n\tAge = {15.ToString(CultureInfo.CurrentCulture)}\r\n\tBirthPlace = null\r\n");
        }

        [Test]
        public void
            PrintToString_StringWithAlternativeSerializationCertainMembers_WhenAlternativeSerializationByMembersType()
        {
            var act = _personPrinter
                .Printing<double>()
                .Using(val => ((int) Math.Ceiling(val)).ToString())
                .PrintToString(Instances.Person);

            act.Should()
                .Be(
                    $"Person\r\n\tId = {Guid.Empty.ToString(null, CultureInfo.CurrentCulture)}\r\n\t" +
                    $"Name = Danil\r\n\tHeight = {161.ToString(CultureInfo.CurrentCulture)}\r\n\t" +
                    $"Age = {15.ToString(CultureInfo.CurrentCulture)}\r\n\t" +
                    $"Weight = {68.ToString(CultureInfo.CurrentCulture)}\r\n\tBirthPlace = null\r\n");
        }

        [Test]
        public void PrintToString_StringWithCertainCultureOfMember_WhenSpecifyCulture()
        {
            var act = _personPrinter
                .Printing<double>()
                .SpecifyCulture(CultureInfo.CreateSpecificCulture("fr-CA"))
                .PrintToString(Instances.Person);

            act.Should()
                .Be(
                    $"Person\r\n\tId = {Guid.Empty.ToString(null, CultureInfo.CurrentCulture)}\r\n\tName = Danil" +
                    $"\r\n\tHeight = 160,5\r\n\tAge = {15.ToString(CultureInfo.CurrentCulture)}\r\n\tWeight = 67,778" +
                    "\r\n\tBirthPlace = null\r\n");
        }

        [Test]
        public void
            PrintToString_StringWithAlternativeSerializationOfCertainMember_WhenAlternativeSerializationOfMember()
        {
            var act = _personPrinter
                .Printing(person => person.Age)
                .Using(age => age + " years old")
                .PrintToString(Instances.Person);

            act.Should()
                .Be(
                    $"Person\r\n\tId = {Guid.Empty.ToString(null, CultureInfo.CurrentCulture)}\r\n\tName = Danil" +
                    $"\r\n\tHeight = {160.5.ToString(CultureInfo.CurrentCulture)}\r\n\t" +
                    $"Age = {15.ToString(CultureInfo.CurrentCulture)} years old\r\n\t" +
                    $"Weight = {67.778.ToString(CultureInfo.CurrentCulture)}\r\n\tBirthPlace = null\r\n");
        }

        [Test]
        public void TrimmedToLength_ThrowException_WhenMaxLengthOfCroppedStringNotPositive()
        {
            var act = new Action(() => _personPrinter
                .Printing<string>()
                .TrimmedToLength(0));

            act.Should().Throw<Exception>();
        }

        [Test]
        public void PrintToString_StringWithTrimmedStringMembers_WhenStringMembersAreTruncated()
        {
            var act = _personPrinter
                .Printing<string>()
                .TrimmedToLength(3)
                .PrintToString(Instances.Person);

            act.Should()
                .Be(
                    $"Person\r\n\tId = {Guid.Empty.ToString(null, CultureInfo.CurrentCulture)}\r\n\t" +
                    $"Name = Dan\r\n\tHeight = {160.5.ToString(CultureInfo.CurrentCulture)}\r\n\t" +
                    $"Age = {15.ToString(CultureInfo.CurrentCulture)}\r\n\t" +
                    $"Weight = {67.778.ToString(CultureInfo.CurrentCulture)}\r\n\tBirthPlace = null\r\n");
        }

        [Test]
        public void PrintToString_StringWithTrimmedCertainMember_WhenSpecificStringMemberIsTruncated()
        {
            var printer = ObjectPrinter.For<Employee>();

            var act = printer
                .Printing(employee => employee.Subordinate.Subordinate.Subordinate.Name)
                .TrimmedToLength(3)
                .PrintToString(Instances.EmployeeWithFewSubordinated);

            act.Should()
                .Contain(Instances.EmployeeWithFewSubordinated.Subordinate.Subordinate.Subordinate.Name.Substring(0, 3))
                .And.NotContain(Instances.EmployeeWithFewSubordinated.Subordinate.Subordinate.Name.Substring(0, 3) +
                                Environment.NewLine)
                .And.NotContain(Instances.EmployeeWithFewSubordinated.Subordinate.Name.Substring(0, 3) +
                                Environment.NewLine)
                .And.NotContain(Instances.EmployeeWithFewSubordinated.Name.Substring(0, 3) + Environment.NewLine);
        }

        [Test]
        public void PrintToString_StringWithoutCertainMember_WhenExcludingCertainMember()
        {
            var act = _personPrinter
                .Excluding(person => person.Id)
                .PrintToString(Instances.Person);

            act.Should()
                .Be(
                    $"Person\r\n\tName = Danil\r\n\tHeight = {160.5.ToString(CultureInfo.CurrentCulture)}\r\n\t" +
                    $"Age = {15.ToString(CultureInfo.CurrentCulture)}\r\n\t" +
                    $"Weight = {67.778.ToString(CultureInfo.CurrentCulture)}\r\n\tBirthPlace = null\r\n");
        }

        [Test]
        public void PrintToString_SerializedString_WhenUsingObjectPrinterExtensionsWithConfiguration()
        {
            var act = Instances.Person
                .PrintToString(config => config
                    .Excluding<Guid>()
                    .Printing<int>()
                    .Using(val => val + "*")
                    .Printing<string>()
                    .TrimmedToLength(3)
                    .Printing(person => person.Weight)
                    .Using(weight => weight + " kilo"));

            act.Should()
                .Be(
                    $"Person\r\n\tName = Dan\r\n\tHeight = {160.5.ToString(CultureInfo.CurrentCulture)}\r\n\t" +
                    $"Age = {15.ToString(CultureInfo.CurrentCulture)}*\r\n\t" +
                    $"Weight = {67.778.ToString(CultureInfo.CurrentCulture)} kilo\r\n\tBirthPlace = null\r\n");
        }

        [Test]
        public void PrintToString_StringWithInformationAboutCycle_WhenContainsCycleLinks()
        {
            var chief = new Employee {Name = "Ivan"};
            var subordinate = new Employee {Name = "Victor", Chief = chief};
            chief.Subordinate = subordinate;

            var act = chief.PrintToString();

            act.Should().Contain("found a cyclic link");
        }

        [Test]
        public void PrintToString_StringNotContainsInfoAboutCycle_WhenIdenticalInstancesAtSameNestingLevel()
        {
            var chief = new Employee {Name = "Ivan"};
            var subordinate = new Employee {Name = "Victor", Chief = chief, Subordinate = chief};

            var act = subordinate.PrintToString();

            act.Should().NotContain("found a cyclic link");
        }

        [Test]
        public void PrintToString_SerializedInstances_WhenOnePrinterSerializeManyInstances()
        {
            var printer = ObjectPrinter.For<Employee>();
            var chief = new Employee {Name = "Ivan"};
            var subordinate = new Employee {Name = "Victor", Chief = chief};
            chief.Subordinate = subordinate;

            var act = printer.PrintToString(chief);

            act.Should().Contain("found a cyclic link");

            chief.Subordinate = null;

            act = printer.PrintToString(subordinate);

            act.Should().NotContain("found a cyclic link");
        }

        [Test]
        public void
            PrintToString_StringWithAlternativeSerializationCertainMember_WhenIntersectionOfAlternativeSerializationByTypeAndMember()
        {
            var act = _personPrinter
                .Printing(person => person.Name)
                .Using(name => name + " years old")
                .Printing<string>()
                .Using(str => str + " ***")
                .PrintToString(Instances.Person);

            act.Should().Contain("Name = Danil years old");
        }

        [Test]
        public void
            PrintToString_StringWithAlternativeSerializationCertainMember_WhenInstanceContainsMembersWithSameNames()
        {
            var collection = new List<object> {new Employee {Name = "Victor"}, new Person {Name = "Ivan"}};
            var printer = ObjectPrinter.For<List<object>>();

            var act = printer.Printing(list => (list[0] as Employee).Name)
                .Using(name => name + "***").PrintToString(collection);

            act.Should().Contain("Victor***").And.NotContain("Ivan***");
        }

        [Test]
        public void PrintingToString_SerializedTextUpToMaxNestingLevel_WhenMaxNestingLevelExceeded()
        {
            var act = Instances.EmployeeWithManySubordinates.PrintToString();

            act.Should().NotContain($"{new Employee {Subordinate = new Employee()}.Subordinate.GetType().Name} null")
                .And.Contain("Nesting level exceeded");
        }

        [Test]
        public void PrintToString_SerializedCollection_WhenSerializeCollection()
        {
            var act = Instances.Collection.PrintToString();

            act.Should().Be(
                $"Dictionary`2\r\n\tKeyValuePair`2\r\n\t\tKey = List`1\r\n\t\t\t{1.ToString(CultureInfo.CurrentCulture)}" +
                $"\r\n\t\t\t{2.ToString(CultureInfo.CurrentCulture)}\r\n\t\t\t{3.ToString(CultureInfo.CurrentCulture)}" +
                $"\r\n\t\tValue = Int32[]\r\n\t\t\t{5.ToString(CultureInfo.CurrentCulture)}\r\n\t\t\t" +
                $"{7.ToString(CultureInfo.CurrentCulture)}\r\n\tKeyValuePair`2\r\n\t\tKey = List`1\r\n\t\t\t" +
                $"{7.ToString(CultureInfo.CurrentCulture)}\r\n\t\t\t{9.ToString(CultureInfo.CurrentCulture)}" +
                $"\r\n\t\tValue = Int32[]\r\n\t\t\t{10.ToString(CultureInfo.CurrentCulture)}\r\n");
        }
    }
}