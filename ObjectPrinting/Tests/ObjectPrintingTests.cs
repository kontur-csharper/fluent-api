using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrintingTests
    {
        private Person person;

        [SetUp]
        public void SetUp()
        {
            person = new Person();
        }

        [Test]
        public void Excluding_Should_Exclude_Type()
        {
            person
                .PrintToString(config => config.Excluding<int>())
                .Should()
                .NotContainAll(new[] { "Age =", "phoneNumber =" });
        }

        [Test]
        public void Using_Should_Use_Specified_Serialization_Method_For_Type()
        {
            var serializedPerson = person
                 .PrintToString(config => config.Printing<int>().Using(i => "Здесь было целое число")
                                                 .Printing<double>().Using(i => "Здесь было дробное число"));

            serializedPerson.Should().ContainAll(new[] { "Здесь было дробное число", "Здесь было целое число" });
            serializedPerson.Should().NotContain("0");
        }

        [Test]
        public void Using_Should_Use_Given_Culture_For_Type()
        {
            person.Height = 12.13;

            var serializedPerson = person.PrintToString(config => config
                                                                    .Printing<double>()
                                                                    .Using(new CultureInfo("eu-ES")));
            serializedPerson.Should().Contain("12,13");
        }

        [Test]
        public void Using_Should_Specified_Serialization_Method_For_Specific_Property()
        {
            person.Name = "Ivan";
            person.Age = 12;

            var serializedPerson = person.PrintToString(config => config
                                                                    .Printing(p => p.Name).Using(name => "Alex")
                                                                    .Printing(p => p.Age).Using(age => "Возраст"));

            serializedPerson.Should().ContainAll(new[] { "Name = Alex", "Age = Возраст" });
            serializedPerson.Should().NotContainAll(new[] { "Age = 12", "Name = Ivan" });
        }

        [Test]
        public void Using_Should_Specified_Serialization_Method_For_Specific_Field()
        {
            person.Address = "USA";
            person.PhoneNumber = 123;

            var serializedPerson = person.PrintToString(config => config
                                                                    .Printing(p => p.Address)
                                                                        .Using(address => "UK")
                                                                    .Printing(p => p.PhoneNumber)
                                                                        .Using(phoneNumber => "000"));

            serializedPerson.Should().ContainAll(new[] { "Address = UK", "PhoneNumber = 000" });
            serializedPerson.Should().NotContainAll(new[] { "Address = USA", "PhoneNumber = 123" });
        }

        [Test]
        public void TrimmedToLength_Should_Trimmed_String()
        {
            person.Name = "Alex";
            person.Address = "Russian Federation";

            var serializedPerson = person.PrintToString(config => config.Printing(p => p.Name).TrimmedToLength(2));

            serializedPerson.Should().Contain("Name = Al");
            serializedPerson.Should().NotContain("Name = Alex");
            serializedPerson.Should().Contain("Address = Russian Federation");
        }

        [Test]
        public void Excluding_Should_Exclude_Specific_Field()
        {
            var serializedPerson = person.PrintToString(config => config.Excluding(p => p.Address)
                                                                        .Excluding(p => p.PhoneNumber));

            serializedPerson.Should().NotContain("Address =");
            serializedPerson.Should().NotContain("PhoneNumber =");
        }

        [Test]
        public void Circular_References_Should_Not_Throw_Exceptions()
        {
            var friend = new Person();
            person.Friend = friend;
            friend.Friend = person;

            var serializedPerson = person.PrintToString();
            var serializedFriend = friend.PrintToString();

            serializedPerson.Should().Contain("Friend = cycle");
            serializedFriend.Should().Contain("Friend = cycle");
        }

        [Test]
        public void Serializer_Should_Process_Collections()
        {
            var mom = new Person { Name = "Mom", Address = "USA" };
            var dad = new Person { Name = "Dad", Address = "USA" };
            person.Family = new List<Person>() { mom, dad };

            var serializedPerson = person.PrintToString();

            serializedPerson.Should().ContainAll(new[] { "Name = Mom", "Name = Dad" });
            Console.WriteLine(serializedPerson);
        }

        [Test]
        public void Serializer_Should_Process_Dictionary()
        {
            var book = new Dictionary<string, int>() { { "Mom", 12345 }, { "Dad", 54321 } };
            person.PhoneBook = book;

            var serializedPerson = person.PrintToString();

            serializedPerson.Should().ContainAll(new[] { "Key = Mom", "Value = 12345",
                                                         "Key = Dad", "Value = 54321"});
            Console.WriteLine(serializedPerson);
        }
    }
}
