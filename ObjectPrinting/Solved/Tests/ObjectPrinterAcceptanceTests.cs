using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ObjectPrinting.Solved.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        Person person;
        Collections collections;

        [SetUp]
        public void SetUp()
        {
            var alterEgo = new Person { Name = "Ivan IV", Age = 52, Height = 164, Id = Guid.NewGuid() };
            var array = new int[]{ 1, 2, 3 };
            var list = new List<double> { 2.5, 3.2, 8.8 };
            var dict = new Dictionary<string, string> { ["a1"] = "a", ["a2"] = "aa", ["a3"] = "aaa" };
            var listOfList = new List<List<int>> { 
                new List<int> { 1, 2, 3}, new List<int> { 2, 3, 4}, new List<int> { 3, 4, 5} };
            person = new Person { Name = "Misha", Age = 21, Height = 172, Id = Guid.NewGuid(), AlterEgo = alterEgo };
            collections = new Collections { Array = array, List = list, Dict = dict, ListOfList = listOfList };
        }
        [Test]
        public void Demo()
        {
            var printer = ObjectPrinter.For<Person>()
                //1. Исключить из сериализации свойства определенного типа
                .Excluding<Guid>()
                //2. Указать альтернативный способ сериализации для определенного типа
                .Printing<int>().Using(i => i.ToString("X"))
                //3. Для числовых типов указать культуру
                .Printing<double>().Using(CultureInfo.InvariantCulture)
                //4. Настроить сериализацию конкретного свойства
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                .Printing(p => p.Name).TrimmedToLength(10)
                //6. Исключить из сериализации конкретного свойства
                .Excluding(p => p.Age);

            string s1 = printer.PrintToString(person);

            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию
            string s2 = person.PrintToString();

            //8. ...с конфигурированием
            string s3 = person.PrintToString(s => s.Excluding(p => p.Age));
            Console.WriteLine(s1);
            Console.WriteLine(s2);
            Console.WriteLine(s3);
        }

        [Test]
        public void ExcludeType_ResultWithoutExcludedType()
        {
            var guids = person.GetType().GetProperties().Where(p => p.PropertyType == typeof(Guid));
            var printing = ObjectPrinter.For<Person>().Excluding<Guid>();
            var result = printing.PrintToString(person);
            foreach (var guid in guids)
            {
                Assert.IsFalse(result.Contains($"{guid.Name} = {guid.GetValue(person)}"));
                Assert.IsFalse(result.Contains($"{guid.Name} = {guid.GetValue(person.AlterEgo)}"));
            }
        }

        [Test]

        public void ExcludingFieldFirstLevel_ResultWithoutExcludedFieldOnlyFirstLevel()
        {
            var printing = ObjectPrinter.For<Person>().Excluding(p => p.Age);
            var result = printing.PrintToString(person);
            var personAge = $"{Environment.NewLine}\tAge = {person.Age}{Environment.NewLine}";
            var egoAge = $"{Environment.NewLine}\t\tAge = {person.AlterEgo.Age}{Environment.NewLine}";
            Assert.IsTrue(!result.Contains(personAge) && result.Contains(egoAge));
        }

        [Test]
        public void ExcludingFieldAlterEgo_ResultWithoutExcludedFieldOnlyAlterEgo()
        {
            var printing = ObjectPrinter.For<Person>().Excluding(p => p.AlterEgo.Age);
            var result = printing.PrintToString(person);
            var personAge = $"{Environment.NewLine}\tAge = {person.Age}{Environment.NewLine}";
            var egoAge = $"{Environment.NewLine}\t\tAge = {person.AlterEgo.Age}{Environment.NewLine}";
            Assert.IsTrue(result.Contains(personAge) && !result.Contains(egoAge));
        }
        
        [Test]
        public void TrimmedToLength_PrintingTrimAllStrings()
        {
            var printing = ObjectPrinter.For<Person>()
                .Printing<string>().TrimmedToLength(2);
            var result = printing.PrintToString(person);
            var nameTrim = $"{Environment.NewLine}\tName = {person.Name[0..2]}{Environment.NewLine}";
            var nameEgo = $"{Environment.NewLine}\t\tName = {person.AlterEgo.Name[0..2]}{Environment.NewLine}";
            Assert.IsTrue(result.Contains(nameTrim) && result.Contains(nameEgo));
        }
        
        [Test]
        public void TrimmedToLength_PrintingTrimSomeString()
        {
            var printing = ObjectPrinter.For<Person>()
                .Printing(p => p.Name).TrimmedToLength(2);
            var nameTrim = $"{Environment.NewLine}\tName = {person.Name[0..2]}{Environment.NewLine}";
            var nameEgo = $"{Environment.NewLine}\t\tName = {person.AlterEgo.Name}{Environment.NewLine}";
            var result = printing.PrintToString(person);
            Assert.IsTrue(result.Contains(nameTrim) && result.Contains(nameEgo));
        }
        
        [TestCase(2, 3)]
        [TestCase(2, 2)]
        [TestCase(3, 2)]
        public void TrimmedToLength_FieldsAndOtherStringDifferentTrim(int stringTrim, int fieldTrim)
        {
            var printing = ObjectPrinter.For<Person>()
                .Printing<string>().TrimmedToLength(stringTrim)
                .Printing(p => p.AlterEgo.Name).TrimmedToLength(fieldTrim);
            var result = printing.PrintToString(person);
            var nameTrim = $"{Environment.NewLine}\tName = {person.Name[0..stringTrim]}{Environment.NewLine}";
            var nameEgo = $"{Environment.NewLine}\t\tName = {person.AlterEgo.Name[0..fieldTrim]}{Environment.NewLine}";
            Assert.IsTrue(result.Contains(nameTrim) && result.Contains(nameEgo));
        }

        [Test]
        public void PrintingMustBeWithoutCircleReference()
        {
            person.AlterEgo = person;
            Assert.AreEqual(person.PrintToString(p => p.Excluding(t => t.AlterEgo.AlterEgo)), person.PrintToString());
        }

        [Test]
        public void PrintingWithCultureDouble_DoubleMustBePrintWithCulture()
        {
            var culture = CultureInfo.InvariantCulture;
            var printer = ObjectPrinter.For<Person>()
                .Printing<double>().Using(culture);
            var result = printer.PrintToString(person);
            var heightPerson = $"{Environment.NewLine}\tHeight = {person.Height.ToString(culture)}{Environment.NewLine}";
            var heightEgo = $"{Environment.NewLine}\t\tHeight = {person.AlterEgo.Height.ToString(culture)}{Environment.NewLine}";
            Assert.IsTrue(result.Contains(heightPerson) && result.Contains(heightEgo));
        }

        [Test]
        public void PrintingWithCultureInt_IntMustBePrintWithCulture()
        {
            var culture = CultureInfo.InvariantCulture;
            var printer = ObjectPrinter.For<Person>()
                .Printing<int>().Using(culture);
            var result = printer.PrintToString(person);
            var heightPerson = $"{Environment.NewLine}\tHeight = {person.Height.ToString(culture)}{Environment.NewLine}";
            var heightEgo = $"{Environment.NewLine}\t\tHeight = {person.AlterEgo.Height.ToString(culture)}{Environment.NewLine}";
            Assert.IsTrue(result.Contains(heightPerson) && result.Contains(heightEgo));
        }

        [Test]
        public void PrintingWithAlternativeSerialization_AllIntFieldsMustBeAlternative()
        {
            var printer = ObjectPrinter.For<Person>()
                .Printing<int>().Using(i => i.ToString("X"));
            var result = printer.PrintToString(person);
            var agePerson = $"{Environment.NewLine}\tAge = {person.Age:X}{Environment.NewLine}";
            var ageEgo = $"{Environment.NewLine}\t\tAge = {person.AlterEgo.Age:X}{Environment.NewLine}";
            Assert.IsTrue(result.Contains(agePerson) && result.Contains(ageEgo));
        }

        [Test]
        public void PrintingWithAlternativeSerialisationFirstLevel_IntFieldFirstLevelOnlyMustBeAlternative()
        {
            var printer = ObjectPrinter.For<Person>()
               .Printing(p => p.Age).Using(i => i.ToString("X"));
            var result = printer.PrintToString(person);
            var agePerson = $"{Environment.NewLine}\tAge = {person.Age:X}{Environment.NewLine}";
            var ageEgo = $"{Environment.NewLine}\t\tAge = {person.AlterEgo.Age}{Environment.NewLine}";
            Assert.IsTrue(result.Contains(agePerson) && result.Contains(ageEgo));
        }
        
        [Test]
        public void PrintingWithAlternativeSerialisationDeepLevel_IntFieldSecondLevelOnlyMustBeAlternative()
        {
            var printer = ObjectPrinter.For<Person>()
               .Printing(p => p.AlterEgo.Age).Using(i => i.ToString("X"));
            var result = printer.PrintToString(person);
            var agePerson = $"{Environment.NewLine}\tAge = {person.Age}{Environment.NewLine}";
            var ageEgo = $"{Environment.NewLine}\t\tAge = {person.AlterEgo.Age:X}{ Environment.NewLine}";
            Assert.IsTrue(result.Contains(agePerson) && result.Contains(ageEgo));
        }

        [Test]
        public void PrintingWithArray_ResultContainAllArrayElements()
        {
            var result = collections.PrintToString();
            foreach (var i in collections.Array)
                Assert.IsTrue(result.Contains($"{Environment.NewLine}\t\t{i}{Environment.NewLine}"));
        }

        [Test]
        public void PrintingWithList_ResultContainAllListElements()
        {
            var result = collections.PrintToString();
            foreach (var i in collections.List)
                Assert.IsTrue(result.Contains($"{Environment.NewLine}\t\t{i}{Environment.NewLine}"));
        }

        [Test]
        public void PrintingWithDictionary_ResultContainAllDictionaryKeysAndValues()
        {
            var result = collections.PrintToString();
            foreach (var i in collections.Dict)
                Assert.IsTrue(result.Contains($"{Environment.NewLine}\t\t[Key] = [{i.Key}], [Value] = [{i.Value}]"));
        }
    }
}