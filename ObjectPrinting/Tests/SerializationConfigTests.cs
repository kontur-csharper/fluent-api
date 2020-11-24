using System.Globalization;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    public class SerializationConfigTests
    {
        private PrintingConfig<Person> printingConfig;

        [SetUp]
        public void SetUp()
        {
            printingConfig = ObjectPrinter.For<Person>();
        }

        [Test]
        public void ShouldExcludeTypes()
        {
            printingConfig.Excluding<double>();
            printingConfig.Config.ExcludedTypes.Should().Contain(typeof(double));
        }

        [Test]
        public void ShouldExcludeField()
        {
            printingConfig.Excluding(x => x.AgeField);

            var person = new Person();
            var personType = typeof(Person);
            printingConfig.Config.ExcludedMembers.Should().Contain(personType.GetMember(nameof(person.AgeField)));
        }

        [Test]
        public void ShouldExcludeProperty()
        {
            printingConfig.Excluding(x => x.Name);

            var person = new Person();
            var personType = typeof(Person);
            printingConfig.Config.ExcludedMembers.Should().Contain(personType.GetMember(nameof(person.Name)));
        }

        [Test]
        public void ShouldSetPrintingRule_ForTypes()
        {
            printingConfig.Printing<double>().Using(x => x.ToString(CultureInfo.InvariantCulture));
            printingConfig.Config.TypePrintingRules.Should().ContainKey(typeof(double));
        }

        [Test]
        public void ShouldSetPrintingRule_ForField()
        {
            printingConfig.Printing(x => x.AgeField).Using(x => x.ToString(CultureInfo.InvariantCulture));

            var person = new Person();
            var personType = typeof(Person);

            printingConfig.Config.MemberPrintingRules.Should().ContainKey(personType.GetMember(nameof(person.AgeField)).FirstOrDefault());
        }

        [Test]
        public void ShouldSetPrintingRule_ForProperty()
        {
            printingConfig.Printing(x => x.Name).Using(x => x.ToString(CultureInfo.InvariantCulture));

            var person = new Person();
            var personType = typeof(Person);
            printingConfig.Config.MemberPrintingRules.Should().ContainKey(personType.GetMember(nameof(person.Name)).FirstOrDefault());
        }

        [Test]
        public void ShouldSetMaxLength()
        {
            printingConfig.Printing(x => x.Name).SetMaxLength(1);

            var person = new Person();
            var personType = typeof(Person);
            printingConfig.Config.MemberTrimToLength.Should().ContainKey(personType.GetMember(nameof(person.Name)).FirstOrDefault());
        }

        [Test]
        public void ShouldSetCulture_ForProperty()
        {
            printingConfig.Printing(x => x.Height).Using(CultureInfo.CurrentCulture);

            var person = new Person();
            var personType = typeof(Person);
            printingConfig.Config.MemberCultures.Should().ContainKey(personType.GetMember(nameof(person.Height)).FirstOrDefault());
        }

        [Test]
        public void ShouldSetCulture_ForType()
        {
            printingConfig.Printing<double>().Using(CultureInfo.CurrentCulture);

            var person = new Person();
            printingConfig.Config.TypeCultures.Should().ContainKey(person.Height.GetType());
        }
    }
}