using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting;
using ObjectPrinting.Infrastructure;
using ObjectPrintingTests.Mocks;

namespace ObjectPrintingTests
{
    public class Tests
    {
        [Test]
        [TestCaseSource(nameof(GetDocumentedTestCaseData))]
        public void PrintToStringTests<T>(PrintingConfig<T> printingConfig, T toPrint, string expected)
        {
            var actual = printingConfig.PrintToString(toPrint);

            actual.Should().Be(expected);
        }

        private static IEnumerable<TestCaseData> GetDocumentedTestCaseData()
        {
            List<(string testName, object config, object toPrint, string expectedRaw)> testData =
                new List<(string testName, object config, object toPrint, string expectedRaw)>
                {
                    ("Cycle reference",
                        ObjectPrinter.For<Node>(),
                        GetCycleReferencedObject(),
                        $"Node{Environment.NewLine}\tNext = Node{Environment.NewLine}\t\tNext = [cycle]{Environment.NewLine}"),

                    ("With String Shortening",
                        ObjectPrinter.For<Text>()
                            .Printing<string>()
                            .TrimmedToLength(2),
                        new Text {String = "1234"},
                        $"Text{Environment.NewLine}\tString = 12{Environment.NewLine}"),

                    ("Custom Property",
                        ObjectPrinter.For<Household>()
                            .Printing(p => p.Property)
                            .Using(p => "custom"),
                        new Household {Property = null},
                        $"Household{Environment.NewLine}\tProperty = custom{Environment.NewLine}"),

                    ("Culture setting",
                        ObjectPrinter.For<Number>()
                            .Printing<double>()
                            .Using(CultureInfo.GetCultureInfo("sv-SE")),
                        new Number() {Double = 1.1},
                        $"Number{Environment.NewLine}\tDouble = 1,1{Environment.NewLine}"),

                    ("Selected field formatted",
                        ObjectPrinter.For<Index>()
                            .Printing(p => p.Integer)
                            .Using(i => "i"),
                        new Index(),
                        $"Index{Environment.NewLine}\tInteger = i{Environment.NewLine}"),

                    ("Selected type formatted",
                        ObjectPrinter.For<Index>()
                            .Printing<int>()
                            .Using(i => "i"),
                        new Index(),
                        $"Index{Environment.NewLine}\tInteger = i{Environment.NewLine}"),

                    ("Excluding By Type",
                        ObjectPrinter.For<Person>()
                            .Excluding<int>(),
                        new Person {Name = "name", Age = 10},
                        $"Person{Environment.NewLine}\tName = name{Environment.NewLine}"),

                    ("Excluding By Field",
                        ObjectPrinter.For<Person>()
                            .Excluding(p => p.Age),
                        new Person {Name = "name", Age = 10},
                        $"Person{Environment.NewLine}\tName = name{Environment.NewLine}"),

                    ("Array of Persons",
                        ObjectPrinter.For<Person[]>(),
                        new[]
                        {
                            new Person {Name = "A", Age = 1},
                            new Person {Name = "B", Age = 2},
                            new Person {Name = "C", Age = 3},
                        },
                        $"Person[]{Environment.NewLine}\tPerson{Environment.NewLine}\t\tAge = 1{Environment.NewLine}\t\tName = A{Environment.NewLine}\tPerson{Environment.NewLine}\t\tAge = 2{Environment.NewLine}\t\tName = B{Environment.NewLine}\tPerson{Environment.NewLine}\t\tAge = 3{Environment.NewLine}\t\tName = C{Environment.NewLine}"),

                    ("List of Persons",
                        ObjectPrinter.For<List<Person>>(),
                        new List<Person>
                        {
                            new Person {Name = "A", Age = 1},
                            new Person {Name = "B", Age = 2},
                            new Person {Name = "C", Age = 3},
                        },
                        $"List`1{Environment.NewLine}\tPerson{Environment.NewLine}\t\tAge = 1{Environment.NewLine}\t\tName = A{Environment.NewLine}\tPerson{Environment.NewLine}\t\tAge = 2{Environment.NewLine}\t\tName = B{Environment.NewLine}\tPerson{Environment.NewLine}\t\tAge = 3{Environment.NewLine}\t\tName = C{Environment.NewLine}"),

                    ("Dictionary with int key, Person value",
                        ObjectPrinter.For<Dictionary<int, Person>>(),
                        new Dictionary<int, Person>
                        {
                            {1, new Person {Name = "A", Age = 1}},
                            {2, new Person {Name = "B", Age = 2}},
                            {3, new Person {Name = "C", Age = 3}},
                        },
                        $"Dictionary`2{Environment.NewLine}\tKeyValuePair`2{Environment.NewLine}\t\tKey = 1{Environment.NewLine}\t\tValue = Person{Environment.NewLine}\t\t\tAge = 1{Environment.NewLine}\t\t\tName = A{Environment.NewLine}\tKeyValuePair`2{Environment.NewLine}\t\tKey = 2{Environment.NewLine}\t\tValue = Person{Environment.NewLine}\t\t\tAge = 2{Environment.NewLine}\t\t\tName = B{Environment.NewLine}\tKeyValuePair`2{Environment.NewLine}\t\tKey = 3{Environment.NewLine}\t\tValue = Person{Environment.NewLine}\t\t\tAge = 3{Environment.NewLine}\t\t\tName = C{Environment.NewLine}"),

                    ("Dictionary",
                        ObjectPrinter.For<Dictionary<Person, List<(Person, TimeSpan)>>[]>(),
                        new Dictionary<Person, List<(Person, TimeSpan)>>[]
                        {
                            new Dictionary<Person, List<(Person, TimeSpan)>>()
                            {
                                {
                                    new Person {Age = 1},
                                    new List<(Person, TimeSpan)>
                                    {
                                        (new Person {Name = "a"}, TimeSpan.Zero),
                                        (new Person {Name = "c"}, TimeSpan.MinValue),
                                    }
                                },
                            },
                            new Dictionary<Person, List<(Person, TimeSpan)>>()
                            {
                                {
                                    new Person {Age = 2},
                                    new List<(Person, TimeSpan)>
                                    {
                                        (new Person {Name = "b"}, TimeSpan.MaxValue)
                                    }
                                },
                                {
                                    new Person {Age = 3},
                                    new List<(Person, TimeSpan)>
                                    {
                                        (new Person {Name = "c"}, TimeSpan.MinValue),
                                        (new Person {Name = "c"}, TimeSpan.MinValue),
                                        (new Person {Name = "c"}, TimeSpan.MinValue),
                                        (new Person {Name = "c"}, TimeSpan.MinValue),
                                    }
                                },
                            },
                        },
                        @"Dictionary`2[]
	Dictionary`2
		KeyValuePair`2
			Key = Person
				Age = 1
				Name = null
			Value = List`1
				ValueTuple`2
					Item1 = Person
						Age = 0
						Name = a
					Item2 = 00:00:00
				ValueTuple`2
					Item1 = Person
						Age = 0
						Name = c
					Item2 = -10675199.02:48:05.4775808
	Dictionary`2
		KeyValuePair`2
			Key = Person
				Age = 2
				Name = null
			Value = List`1
				ValueTuple`2
					Item1 = Person
						Age = 0
						Name = b
					Item2 = 10675199.02:48:05.4775807
		KeyValuePair`2
			Key = Person
				Age = 3
				Name = null
			Value = List`1
				ValueTuple`2
					Item1 = Person
						Age = 0
						Name = c
					Item2 = -10675199.02:48:05.4775808
				ValueTuple`2
					Item1 = Person
						Age = 0
						Name = c
					Item2 = -10675199.02:48:05.4775808
				ValueTuple`2
					Item1 = Person
						Age = 0
						Name = c
					Item2 = -10675199.02:48:05.4775808
				ValueTuple`2
					Item1 = Person
						Age = 0
						Name = c
					Item2 = -10675199.02:48:05.4775808
"
                    ),
                };

            foreach (var (testName, config, toPrint, expectedRaw) in testData)
            {
                yield return new TestCaseData(
                        config,
                        toPrint,
                        expectedRaw)
                    {TestName = testName};
            }
        }

        private static Node GetCycleReferencedObject()
        {
            var node1 = new Node();
            var node2 = new Node {Next = node1};
            node1.Next = node2;
            return node1;
        }
    }
}