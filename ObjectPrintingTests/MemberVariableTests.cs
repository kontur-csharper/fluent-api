using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting;

namespace ObjectPrintingTests
{
    [TestFixture]
    public class MemberVariableTests
    {
        private TestClass testClass;

        private static MemberVariable[] _memberVarsFromTestClass =
        {
            new MemberVariable(typeof(TestClass).GetProperty(nameof(TestClass.FirstProp))),
            new MemberVariable(typeof(TestClass).GetProperty(nameof(TestClass.SecondProp))),
            new MemberVariable(typeof(TestClass).GetField(nameof(TestClass.Field)))
        };

        [SetUp]
        public void SetUp()
        {
            testClass = new TestClass();
        }

        [Test]
        public void GetMemberVariables_ReturnOnlyPublicFieldsAndProperties()
        {
            var memberVars = MemberVariable.GetMemberVariables(typeof(TestClass));

            memberVars.Should().BeEquivalentTo(_memberVarsFromTestClass);
        }

        [Test]
        public void FromExpression_IfSelectField_ReturnMemberVariableForThisField()
        {
            var expected = new MemberVariable(typeof(TestClass).GetField(nameof(TestClass.Field)));
            
            MemberVariable.FromExpression((TestClass testClass) => testClass.Field).Should()
                .BeEquivalentTo(expected);
        }
        
        [Test]
        public void FromExpression_IfSelectProperty_ReturnMemberVariableForThisProperty()
        {
            var expected = new MemberVariable(typeof(TestClass).GetProperty(nameof(TestClass.FirstProp)));
            
            MemberVariable.FromExpression((TestClass testClass) => testClass.FirstProp).Should()
                .BeEquivalentTo(expected);
        }

        [Test]
        public void GetValue_CanReturnValueOfProperty()
        {
            var memberVar = new MemberVariable(
                typeof(TestClass).GetProperty(nameof(TestClass.SecondProp)));

            memberVar.GetValue(testClass).Should().Be(testClass.SecondProp);
        }

        [Test]
        public void GetValue_CanReturnValueOfField()
        {
            var memberVar = new MemberVariable(
                typeof(TestClass).GetField(nameof(TestClass.Field)));

            memberVar.GetValue(testClass).Should().Be(testClass.Field);
        }

        [Test]
        public void MemberVarType_CanReturnTypeOfProperty()
        {
            var memberVar = new MemberVariable(
                typeof(TestClass).GetProperty(nameof(TestClass.SecondProp)));

            memberVar.MemberVarType.Should().Be(
                testClass.SecondProp.GetType());
        }

        [Test]
        public void MemberVarType_CanReturnTypeOfField()
        {
            var memberVar = new MemberVariable(
                typeof(TestClass).GetField(nameof(TestClass.Field)));

            memberVar.MemberVarType.Should().Be(
                testClass.Field.GetType());
        }

        [Test]
        public void Equals_IfMemberInfoFieldsAreEqual_ReturnTrue()
        {
            var member = new MemberVariable(typeof(TestClass).GetProperty(nameof(TestClass.FirstProp)));
            var sameMember = new MemberVariable(typeof(TestClass).GetProperty(nameof(TestClass.FirstProp)));

            member.Equals(sameMember).Should().BeTrue();
        }
        
        [Test]
        public void Equals_IfMemberInfoFieldsAreNotEqual_ReturnFalse()
        {
            var member = new MemberVariable(typeof(TestClass).GetProperty(nameof(TestClass.FirstProp)));
            var otherMember = new MemberVariable(typeof(TestClass).GetProperty(nameof(TestClass.SecondProp)));

            member.Equals(otherMember).Should().BeFalse();
        }
    }

    public class TestClass
    {
        public int FirstProp { get; set; } = 1;
        public string SecondProp { get; set; } = "two";

        public double Field = 2.5;
        private double closeField;
    }
}