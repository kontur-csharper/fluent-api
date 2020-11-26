using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using ObjectPrinting;
using ObjectPrinting.Printer;
using ObjectPrintingTests.TestingObjects;

namespace ObjectPrintingTests
{
    [TestFixture]
    public class PrintingConfigTests
    {
        [SetUp]
        public void SetUp()
        {
            specificCulture = new CultureInfo("ru-RU")
            {
                NumberFormat = new NumberFormatInfo
                {
                    NaNSymbol = "https://habr.com/ru/news/t/526412/"
                }
            };

            var node = new PathNode
            {
                Id = 2
            };
            pathNodeWithRecursiveLinks.Right = node;
            node.Right = pathNodeWithRecursiveLinks;

            node = new PathNode
            {
                Id = 101
            };
            pathNode.Right = node;
            pathNode.Left = node;
        }

        private readonly Person person = new Person
        {
            Id = 22,
            Age = 12,
            Name = "Jeff",
            Width = 99,
            Height = 33.4
        };

        private readonly PathNode pathNodeWithRecursiveLinks = new PathNode
        {
            Id = 1
        };

        private readonly PathNode pathNode = new PathNode
        {
            Id = 100
        };


        private CultureInfo specificCulture;

        [Test]
        public void ExcludePropertyByType()
        {
            var config = new PrintingConfig<Person>();
            config.Exclude<int>();

            var printed = config.PrintToString(person);

            AssertPrinting(printed, new[] {"Name", "SleptToday"}, new[] {"Age", "Id"});
        }

        [Test]
        public void SpecificCulture()
        {
            var config = new PrintingConfig<Person>();
            config.Select<double>().SetCulture(specificCulture);

            person.Height = double.NaN;
            var printed = config.PrintToString(person);

            AssertPrinting(printed, new[] {"https://habr.com/ru/news/t/526412/"}, new string[0]);
        }

        [Test]
        public void AlternateSerializeWayByType()
        {
            var config = new PrintingConfig<Person>();
            config.Select<int>().SetSerializeWay(value => $"This is int: {value}");

            var printed = config.PrintToString(person);

            AssertPrinting(printed, new[] {"This is int:"}, new string[0]);
        }

        [Test]
        public void AlternateSerializeWayByName()
        {
            var config = new PrintingConfig<Person>();
            config.Select(x => x.Width).SetSerializeWay(value => "Secret number");
            config.Select(x => x.Height).SetSerializeWay(value => $"Around {Math.Round(value, 0)}");

            var printed = config.PrintToString(person);

            AssertPrinting(printed, new[] {"Secret number", "Around 33"}, new[] {"99"});
        }

        [Test]
        public void ExcludePropertyByName()
        {
            var config = new PrintingConfig<Person>();
            config.Exclude(x => x.Age);

            var printed = config.PrintToString(person);

            AssertPrinting(printed, new[] {"Id"}, new[] {"Age"});
        }

        [Test]
        public void StringMustBeCropped()
        {
            var config = new PrintingConfig<Person>();
            config.Select<string>().CropTo(2);

            var printed = config.PrintToString(person);

            AssertPrinting(printed, new[] {"Je"}, new[] {"ff"});
        }

        [Test]
        public void RecursiveLinksShouldNotBeBreakingApp()
        {
            var config = new PrintingConfig<PathNode>();

            var printed = config.PrintToString(pathNodeWithRecursiveLinks);

            AssertPrinting(printed, new[] {"Recursive link"}, new string[0]);
        }

        [Test]
        public void SameObjectsAreNotRecursiveLinks()
        {
            var config = new PrintingConfig<PathNode>();

            var printed = config.PrintToString(pathNode);

            AssertPrinting(printed, new string[0], new[] {"Recursive link"});
        }

        [Test]
        public void CanBePrintEnumerable()
        {
            var config = new PrintingConfig<List<int>>();
            var list = new List<int>() { 1, 99, 2 };
            var expectedString = "[ 1, 99, 2 ]";

            var printed = config.PrintToString(list);

            AssertPrinting(printed, new[] { expectedString }, new string[0]);
        }

        [Test]
        public void CanBePrintDictionary()
        {
            var config = new PrintingConfig<Dictionary<string, string>>();
            var dictionary = new Dictionary<string, string>()
            {
                { "hello", "world" },
                { "1", "2" }
            };
            var expectedStrings = new List<string>()
            {
                "{ hello: world }", "{ 1: 2 }"
            };

            var printed = config.PrintToString(dictionary);

            AssertPrinting(printed, expectedStrings, new string[0]);
        }

        [Test]
        public void NullShouldBePrintCorrect()
        {
            var config = new PrintingConfig<Dictionary<int, int>>();

            var printed = config.PrintToString(null);

            AssertPrinting(printed, new[] { "null" }, new string[0]);
        }

        private void AssertPrinting(string text, IEnumerable<string> contains, IEnumerable<string> notContains)
        {
            foreach (var item in contains)
                Assert.True(text.Contains(item), $"'{item}' doesn't contains in text");

            foreach (var item in notContains)
                Assert.False(text.Contains(item), $"'{item}' contains in text, but should not be");
        }
    }
}
