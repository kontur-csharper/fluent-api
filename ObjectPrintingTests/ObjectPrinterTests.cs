using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting.Extensions;

namespace ObjectPrintingTests
{
    public class ObjectPrinterTests
    {
        private Person person;

        [SetUp]
        public void SetUp()
        {
            person = new Person();
        }

        [Test]
        public void PrintToString_ExcludeType()
        {
            person.PrintToString(config => config.Excluding<double>())
                .Should()
                .NotContainAll("Height =", "FortuneInDollars =");
        }

        [Test]
        public void PrintToString_UseTypeSerializer()
        {
            var serializedPerson = person
                 .PrintToString(config => config
                     .Printing<int>().Using(i => "integer")
                     .Printing<double>().Using(i => "double"));

            serializedPerson.Should().ContainAll("integer", "double");
            serializedPerson.Should().NotContainAll("0");
        }

        [Test]
        public void PrintToString_UseCultureForType()
        {
            person.Height = 12.13;

            person.PrintToString(config => config
                    .Printing<double>()
                    .Using(new CultureInfo("eu-ES")))
                .Should().Contain("12,13");
        }

        [Test]
        public void PrintToString_UsePropertySerializer()
        {
            person.Name = "John";
            person.Age = 12;

            var serializedPerson = person
                .PrintToString(config => config
                    .Printing(p => p.Name).Using(name => "Bob")
                    .Printing(p => p.Age).Using(age => "SomeAge"));

            serializedPerson.Should().ContainAll("Name = Bob", "Age = SomeAge");
            serializedPerson.Should().NotContainAll("Age = 12", "Name = John");
        }

        [Test]
        public void PrintToString_UseFieldSerializer()
        {
            person.Nickname = "abracadabra";
            person.FortuneInDollars = 200d;

            var serializedPerson = person
                .PrintToString(config => config
                    .Printing(p => p.Nickname)
                    .Using(nick => "ColonelSanders")
                    .Printing(p => p.FortuneInDollars)
                    .Using(fortune => "1000"));

            serializedPerson.Should().ContainAll("Nickname = ColonelSanders", "FortuneInDollars = 1000");
            serializedPerson.Should().NotContainAll("Nickname = abracadabra", "FortuneInDollars = 200.0");
        }

        [Test]
        public void PrintToString_TrimStringProperties()
        {
            person.Name = "Alexander";
            person.Nickname = "abracadabra";

            var serializedPerson = person
                .PrintToString(config => config
                    .Printing(p => p.Name).TrimmedToLength(4));

            serializedPerson.Should().Contain("Name = Alex");
            serializedPerson.Should().NotContain("Name = Alexander");
            serializedPerson.Should().Contain("Nickname = abracadabra");
        }

        [Test]
        public void PrintToString_ExcludeField()
        {
            var serializedPerson = person
                .PrintToString(config => config
                    .Excluding(p => p.Nickname)
                    .Excluding(p => p.FortuneInDollars));

            serializedPerson.Should().NotContain("Nickname =");
            serializedPerson.Should().NotContain("FortuneInDollars =");
        }

        [Test]
        public void PrintToString_ShouldNotSerializeCircularLinks()
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
        public void PrintToString_ShouldSerializeIEnumerable()
        {
            var mom = new Person { Name = "Helen" };
            var dad = new Person { Name = "Andrew" };
            person.Parents = new List<Person>() { mom, dad };

            person.PrintToString().Should().ContainAll("Name = Helen", "Name = Andrew");
        }

        [Test]
        public void PrintToString_ShouldSerializeDictionary()
        {
            person.Wallet = new Dictionary<string, double>() { { "USD", 1000 }, { "RUB", 20000 } };

            person.PrintToString()
                .Should().ContainAll("Key = USD", "Value = 1000", "Key = USD", "Value = 20000");
        }
    }
}
