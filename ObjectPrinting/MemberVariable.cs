using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectPrinting
{
    public class MemberVariable : IEquatable<MemberVariable>
    {
        private readonly MemberInfo memberVarInfo;

        public string Name => memberVarInfo.Name;

        public Type MemberVarType => memberVarInfo switch
        {
            PropertyInfo property => property.PropertyType,
            FieldInfo field => field.FieldType,
        };

        public MemberVariable(PropertyInfo varInfo)
        {
            if (varInfo == null)
                throw new ArgumentException("Parameter is null");
            memberVarInfo = varInfo;
        }

        public MemberVariable(FieldInfo varInfo)
        {
            if (varInfo == null)
                throw new ArgumentException("Parameter is null");
            memberVarInfo = varInfo;
        }

        public object GetValue(object obj)
        {
            return memberVarInfo switch
            {
                PropertyInfo property => property.GetValue(obj),
                FieldInfo field => field.GetValue(obj),
            };
        }

        public bool Equals(MemberVariable other)
        {
            return memberVarInfo.Equals(other?.memberVarInfo);
        }

        public override bool Equals(object? obj)
        {
            return obj is MemberVariable memberVar && Equals(memberVar);
        }

        public override int GetHashCode()
        {
            return memberVarInfo.GetHashCode();
        }

        public static MemberVariable[] GetMemberVariables(Type type)
        {
            var fieldMembers = type.GetFields()
                .Select(info => new MemberVariable(info));
            var propertyMembers = type.GetProperties()
                .Select(info => new MemberVariable(info));

            return fieldMembers.Concat(propertyMembers).ToArray();
        }

        public static MemberVariable FromExpression<TDeclaring, TMember>(
            Expression<Func<TDeclaring, TMember>> memberSelector)
        {
            var member = ((MemberExpression) memberSelector.Body).Member;

            return member switch
            {
                PropertyInfo propertyInfo => new MemberVariable(propertyInfo),
                FieldInfo fieldInfo => new MemberVariable(fieldInfo),
                _ => throw new ArgumentException("Member isn't property or field")
            };
        }
    }
}