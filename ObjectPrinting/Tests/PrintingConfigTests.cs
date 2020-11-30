using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    public class PrintingConfigTests
    {
        private PrintingConfig<Person> printer;
        
        private static readonly Person MyParent = 
            new Person() {Age = 40, Height = 200.001, Name = "Anthony", Surname = "Smit"};
        
        private static readonly Person Me =
            new Person() {Age = 20, Height = 150.5, Name = "Natasha", Parent = MyParent, Surname = "Smit"};

        [SetUp]
        public void SetUp()
        {
            printer = ObjectPrinter.For<Person>();
        }

        [Test]
        public void PrintEverything()
        {
            printer.PrintToString(Me).Should()
                .Contain("Age = 20")
                .And.Contain("Height = 150,5")
                .And.Contain("Name = Natasha")
                .And.Contain("Surname = Smit")
                .And.Contain("Parent = ")
                .And.Contain("Name = Anthony")
                .And.Contain("Age = 40")
                .And.Contain("Height = 200,001");

        }

        [Test]
        public void ExcludingType_DoNotPrintStrings_On()
        {
            printer.Excluding<string>();
            printer.PrintToString(Me).Should()
                .NotContain("Name = Natasha")
                .And.NotContain("Name = Anthony")
                .And.NotContain("Surname = Smit");
        }

        [Test]
        public void ExcludingField()
        {
            printer.Excluding(person => person.Name);
            printer.PrintToString(Me).Should()
                .NotContain("Name = Natasha")
                .And.NotContain("Name = Anthony")
                .And.Contain("Surname = Smit");
        }

        [Test]
        public void AddSerialization_ForType()
        {
            printer.Printing<string>().Using(str => '"' + str + '"');
            printer.PrintToString(Me).Should()
                .Contain("Name = \"Natasha\"")
                .And.Contain("Name = \"Natasha\"")
                .And.Contain("Surname = \"Smit\"");
        }

        [Test]
        public void AddCultureInfo_ForType()
        {
            printer.Printing<double>().Using(CultureInfo.InvariantCulture);
            printer.PrintToString(Me).Should()
                .Contain("Height = 150.5")
                .And.Contain("Height = 200.001");
        }

        [Test]
        public void AddLengthTrim_ForProperty()
        {
            printer.Printing(p => p.Name).TrimmedToLength(3);
            printer.PrintToString(Me).Should()
                .Contain("Name = Nat" + Environment.NewLine)
                .And.Contain("Name = Ant" + Environment.NewLine)
                .And.Contain("Surname = Smit");
        }

        [Test]
        public void AddSerialization_ForNumberProperty()
        {
            printer.Printing(p => p.Age).Using(age => age + ".00");
            printer.PrintToString(Me).Should()
                .Contain("Age = 20.00")
                .And.Contain("Age = 40.00");
        }

        [Test]
        public void AddSerialization_ForStringProperty()
        {
            printer.Printing(p => p.Name).Using(name => name.ToUpper());
            printer.PrintToString(Me).Should()
                .Contain("Name = NATASHA")
                .And.Contain("Name = ANTHONY")
                .And.Contain("Surname = Smit");
        }

        [Test]
        public void ShouldNotFall_OnSimpleCircleRef()
        {
            Me.Parent = Me;
            printer.PrintToString(Me).Should()
                .Contain("Parent = circle ref")
                .And.Contain("Name = Natasha")
                .And.Contain("Age = 20")
                .And.Contain("Height = 150,5");
        }

        [Test]
        public void ShouldNotFall_OnDeepCircleRef()
        {
            MyParent.Parent = Me;
            printer.PrintToString(Me).Should().Contain("Parent = circle ref");
        }

        [Test]
        public void ShouldAddSerializationByPropertyAndByType()
        {
            printer.Printing<string>().Using(str => str.ToUpper());
            printer.Printing(p => p.Name)
                .Using(str => str.Substring(1));
            printer.PrintToString(Me).Should()
                .Contain("SMIT")
                .And.Contain(" atasha")
                .And.Contain(" nthony");
        }

        [Test]
        public void ShouldAddCulture_ToDateTime()
        {
            var dataPrinter = ObjectPrinter.For<DateTime>();
            dataPrinter.Printing<DateTime>().Using(CultureInfo.InvariantCulture);
            dataPrinter.PrintToString(new DateTime(2020, 11, 27)).Should()
                .Contain("11/27/2020");
        }

        [Test]
        public void ShouldSerializePropertyWithCorrectParent_ParentIsTOwner()
        {
            Me.PersonPet = new Pet(){Name = "Cat"};
            printer.Printing(p => p.Name).Using(str => str.ToUpper());
            printer.PrintToString(Me).Should()
                .Contain("NATASHA")
                .And.Contain("Cat");
        }
        
        [Test]
        public void ShouldSerializePropertyWithCorrectParent_ParentIsNotTOwner()
        {
            Me.PersonPet = new Pet(){Name = "Cat"};
            printer.Printing(p => p.PersonPet.Name).Using(str => str.ToUpper());
            printer.PrintToString(Me).Should()
                .Contain("Natasha")
                .And.Contain("CAT");
        }
        
        [Test]
        public void ShouldSerializePropertyWithCorrectParent_DifferentParents()
        {
            Me.PersonPet = new Pet(){Name = "Cat"};
            printer.Printing(p => p.PersonPet.Name).Using(str => str.ToUpper());
            printer.Printing(p => p.Name).Using(str => str.ToLower());
            printer.PrintToString(Me).Should()
                .Contain("natasha")
                .And.Contain("CAT");
        }

        [Test]
        public void ShouldExcludePropertyWithCorrectParent_ParentIsTOwner()
        {
            Me.PersonPet = new Pet(){Name = "Cat"};
            printer.Excluding(p => p.Name);
            printer.PrintToString(Me).Should()
                .Contain("Cat")
                .And.NotContain("Natasha");
        }
        
        [Test]
        public void ShouldExcludePropertyWithCorrectParent_DifferentParents()
        {
            Me.PersonPet = new Pet(){Name = "Cat"};
            printer.Excluding(p => p.Name);
            printer.Excluding(p => p.PersonPet.Name);
            printer.PrintToString(Me).Should()
                .NotContain("Cat")
                .And.NotContain("Natasha");
        }
        
        
        [Test]
        public void ShouldExcludePropertyWithCorrectParent_ParentIsNotTOwner()
        {
            Me.PersonPet = new Pet(){Name = "Cat"};
            printer.Excluding(p => p.PersonPet.Name);
            printer.PrintToString(Me).Should()
                .Contain("Natasha")
                .And.NotContain("Cat");
        }

        [Test]
        public void ShouldNotFindCircleRef_OnOneLevelEqualObjects()
        {
            MyParent.Parent = null;
            Me.Parent = MyParent;
            Me.PersonPet = new Pet(){Name = "Dog"};
            Me.AnotherPet = Me.PersonPet;
            printer.PrintToString(Me).Should()
                .NotContain("circle ref");
        }

        [Test]
        public void ShouldNotFindCircleRef_InDifferentObjects()
        {
            MyParent.Parent = null;
            Me.Parent = MyParent;
            Me.PersonPet = new Pet() {Name = "Dog"};
            Me.PersonPet.Owner = MyParent;
            printer.PrintToString(Me).Should()
                .NotContain("circle ref");
        }
        
        [Test]
        public void ShouldFindCircleRef_TwoStepsDown()
        {
            Me.Parent = MyParent;
            Me.PersonPet = new Pet() {Name = "Dog"};
            Me.PersonPet.Owner = MyParent;
            MyParent.Parent = Me;
            printer.PrintToString(Me).Should()
                .Contain("circle ref");
        }
    }
}