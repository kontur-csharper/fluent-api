using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{

    public class TestClassExcludedList
    {
        public List<int> listInt { get; set; }
    }
    public class TestClassExcludedString
    {
        public string TestText { get; set; }
    }
    public class TestClassExcludedStringAndTestClass
    {
        public string TestText { get; set; }
        public TestClassExcludedStringAndTestClass TestClass { get; set; }
    }
    public class TestClassExcluded
    {
        public string TestTextString { get; set; }
        public int TestTextInt { get; set; }
        public Person person { get; set; }
        
        public Double TestTextDouble { get; set; }
    }
    
    public class TestClassCultureDataTime
    {
        public DateTime dataTime { get; set; }
    }
    
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        [Test]
        public void DemoField()
        {
            var personField = new PersonField(){DateTime = DateTime.Today, Age = 19, Money = 2.2, Name = "name"};

            Console.WriteLine(DateTime.Today);
            var printer = ObjectPrinter.For<PersonField>()
                //.Excluding<string>();
                .Printing<int>().Using(i => i + "Ы")
                .Printing(x => x.Name).Using(y => y + "@@@")
                .Printing<DateTime>().Using(CultureInfo.InvariantCulture);

            string s1 = printer.PrintToString(personField);
            Console.WriteLine(s1);
        }


        [Test]
        public void Demo()
        {
            var person = new Person();
            person.Array = new[] {"a", "b", "c"};
            person.Blabla = "blabla";
            person.Dictionary = new Dictionary<int, int>();
            person.Dictionary.Add(1,1);
            person.Dictionary.Add(2,2);
            person.Dictionary.Add(3,3);
            person.Money = 0.0;
            person.InnerPerson = new Person();
            person.Age = 19;
            person.Height = 222;
            person.List = new List<int>(){1,2,3};
            person.Name = "name";

            var printer = ObjectPrinter.For<Person>()
                //.Excluding<string>();
                .Printing<int>().Using(i => i + "Ы")
                .Printing(x=>x.Name).Using(y =>y +"@@@");

            string s1 = printer.PrintToString(person);
            Console.WriteLine(s1);
        }

        [Test]
        public void PrintToString_ReturnCorrectWord_WhenTypeSetCulture()
        {
            var testClass = new TestClassExcluded();
            testClass.TestTextDouble = 2.3;
            var printer = ObjectPrinter.For<TestClassExcluded>()
                .Excluding<int>()
                .Excluding<Person>()
                .Excluding<string>()
                .Printing<double>().Using(CultureInfo.InvariantCulture);
                
        
            string s1 = printer.PrintToString(testClass);
            Console.WriteLine("|"+s1+"|");

            s1.Should().BeEquivalentTo("TestClassExcluded"
                                       +Environment.NewLine
                                       + "\t" +" TestTextDouble = 2.3"
                                       +Environment.NewLine);
        }
        [Test]
        public void PrintToString_ReturnCorrectWord_WhenTypeSetCultureDataTime()
        {
            var testClass = new TestClassCultureDataTime();
            testClass.dataTime = DateTime.Today;

            var printer = ObjectPrinter.For<TestClassCultureDataTime>()
                .Printing<DateTime>().Using(CultureInfo.InvariantCulture);

            string s1 = printer.PrintToString(testClass);
            Console.WriteLine("|"+s1+"|");

            s1.Should().BeEquivalentTo("TestClassCultureDataTime"
                                       +Environment.NewLine
                                       + "\t" +" dataTime = 11/26/2020 00:00:00");
        }
        
        [Test]
        public void PrintToString_ReturnCurrent_WhenListProperty()
        {
            var testClass = new TestClassExcludedList();
            testClass.listInt =new List<int>(){1,2};
            var printer = ObjectPrinter.For<TestClassExcludedList>();

            string s1 = printer.PrintToString(testClass);
            Console.WriteLine("|"+s1+"|");

            s1.Should().BeEquivalentTo("TestClassExcludedList" +
                                       ""+Environment.NewLine
                                       + "\t" +" listInt = "
                                       +Environment.NewLine 
                                       +"\t"+"\t"+"\t"+"\t" +"1"
                                       +Environment.NewLine
                                       +"\t"+"\t"+"\t"+"\t" +"2"
                                       +Environment.NewLine);
        }
        
        [Test]
        public void PrintToString_ReturnStringWithoutExcludedType_WhenTypeExcluded()
        {
            var testClass = new TestClassExcludedString();
            var printer = ObjectPrinter.For<TestClassExcludedString>()
                .Excluding<string>()
                .Excluding<double>();
            
        
            string s1 = printer.PrintToString(testClass);
            Console.WriteLine("|"+s1+"|");

            s1.Should().BeEquivalentTo("TestClassExcludedString"+Environment.NewLine);
        }
        
        [Test]
        public void PrintToString_ReturnCorrectString_WhenPropertyExcluded()
        {
            var testClass = new TestClassExcluded();
            testClass.TestTextString = "text!!!";
            var printer = ObjectPrinter.For<TestClassExcluded>()
                .Excluding(s => s.person)
                .Excluding(s => s.TestTextDouble)
                .Excluding(s => s.TestTextInt);
            
            string s1 = printer.PrintToString(testClass);
            Console.WriteLine("|"+s1+"|");

            s1.Should().BeEquivalentTo("TestClassExcluded"
                                       +Environment.NewLine
                                       + "\t" + " TestTextString = text!!!"
                                       +Environment.NewLine);
        }
        
        [Test]
        public void PrintToString_ReturnCorrectString_WhenReferenceCycle()
        {
            var testClass = new TestClassExcludedStringAndTestClass();
            testClass.TestText = "aaa";
            testClass.TestClass = testClass;
            var printer = ObjectPrinter.For<TestClassExcludedStringAndTestClass>();

            string s1 = printer.PrintToString(testClass);
            Console.WriteLine("|"+s1+"|");

            s1.Should().BeEquivalentTo("TestClassExcludedStringAndTestClass"
                                       + Environment.NewLine
                                       + "\t" + " TestText = aaa"
                                       + Environment.NewLine
                                       + "\t" + " TestClass = TestClassExcludedStringAndTestClass"
                                       + Environment.NewLine
                                       + "\t" + "\t" + " TestText = aaa"
                                       + Environment.NewLine
                                       + "\t" + "\t" +"reference cycle ObjectPrinting.Tests.TestClassExcludedStringAndTestClass");
        }
        
        [Test]
        public void PrintToString_ReturnCurrentString_WhenUsingAlternativeSerializationForProperty()
        {
            var testClass = new TestClassExcludedString();
            testClass.TestText = "Text";
            var printer = ObjectPrinter.For<TestClassExcludedString>()
                .Printing(x => x.TestText).Using(s => s + " + AltSerializeProperty(str)");
            
        
            string s1 = printer.PrintToString(testClass);
            Console.WriteLine("|"+s1+"|");

            s1.Should().BeEquivalentTo("TestClassExcludedString"
                                       +Environment.NewLine
                                       + "\t" + " TestText = Text + AltSerializeProperty(str)"
                                       +Environment.NewLine);
        }
        
        [Test]
        public void PrintToString_ReturnCurrentString_WhenStringTrim()
        {
            var testClass = new TestClassExcludedString();
            testClass.TestText = "Text";
            var printer = ObjectPrinter.For<TestClassExcludedString>()
                .Printing(x => x.TestText).TrimmedToLength(2);
            
        
            string s1 = printer.PrintToString(testClass);
            Console.WriteLine("|"+s1+"|");

            s1.Should().BeEquivalentTo("TestClassExcludedString"
                                       +Environment.NewLine
                                       + "\t" + " TestText = Te"
                                       +Environment.NewLine);
        }
        
        [Test]
        public void PrintToString_ReturnCorrectString_WhenUsingAlternativeSerializationForType()
        {
            var testClass = new TestClassExcluded();
            testClass.TestTextInt = 2;
            testClass.TestTextString = "text";
            var printer = ObjectPrinter.For<TestClassExcluded>()
                .Excluding<Person>()
                .Excluding<double>()
                .Printing<string>().Using(s => s+" + AltSerialize(str)")
                .Printing<int>().Using(i => i+" + AltSerialize(int)");
        
            string s1 = printer.PrintToString(testClass);
            Console.WriteLine("|"+s1+"|");

            s1.Should().BeEquivalentTo("TestClassExcluded"
                                       + Environment.NewLine
                                       + "\t" + " TestTextString = text + AltSerialize(str)"
                                       + Environment.NewLine
                                       + "\t" + " TestTextInt = 2 + AltSerialize(int)"
                                       + Environment.NewLine);
        }
        
        [Test]
        public void PrintToString_ReturnStringWithoutExcludedTypes_WhenManyTypesExcluded()
        {
            var testClass = new TestClassExcluded();
            var person = new Person { Name = "Alex", Age = 19 };
            testClass.person = person;
            testClass.TestTextInt = 2;
            testClass.TestTextString = "text";

            var printer = ObjectPrinter.For<TestClassExcluded>()
                .Excluding<string>()
                .Excluding<Person>()
                .Excluding<double>();

            string s1 = printer.PrintToString(testClass);
            Console.WriteLine("|"+s1+"|");

            s1.Should().BeEquivalentTo("TestClassExcluded" 
                                       + Environment.NewLine 
                                       +"\t"+" TestTextInt = 2"
                                       +Environment.NewLine);
        }
    }
}