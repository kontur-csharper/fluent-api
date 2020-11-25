using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ObjectPrinting.Configuration;
using ObjectPrinting.Nodes;

namespace ObjectPrinting
{
    public class PrintingConfigBuilder<TOwner>
    {
        private readonly Type[] finalTypes;
        private readonly ChildedNode<IPropertyConfigurator> configurationRoot;
        private readonly IDictionary<Type, IPropertyConfigurator> groupAppliedConfigurators;

        public PrintingConfigBuilder(Type[] finalTypes)
        {
            this.finalTypes = finalTypes;
            configurationRoot = new ChildedNode<IPropertyConfigurator>(nameof(TOwner));
            groupAppliedConfigurators = new Dictionary<Type, IPropertyConfigurator>();
        }

        public PrintingConfig<TOwner> Build() =>
            new PrintingConfig<TOwner>(finalTypes, configurationRoot, groupAppliedConfigurators);

        public SelectedPropertyGroup<TOwner, TProperty> Choose<TProperty>()
        {
            var propertyGroup = new SelectedPropertyGroup<TOwner, TProperty>(this);
            groupAppliedConfigurators.Add(typeof(TProperty), propertyGroup);
            return propertyGroup;
        }

        public SelectedProperty<TOwner, TProperty> Choose<TProperty>(Expression<Func<TOwner, TProperty>>? selector)
        {
            if (!(selector?.Body is MemberExpression memberExpression))
                throw new ArgumentException($"{nameof(selector)} must be {nameof(MemberExpression)}, but was " +
                    selector?.Body.GetType().Name ?? "<null>");

            var target = memberExpression.Member switch
            {
                PropertyInfo propertyInfo => new SerializationTarget(propertyInfo),
                FieldInfo fieldInfo => new SerializationTarget(fieldInfo),
                _ => throw new ArgumentException($"{nameof(selector)} must point on a property or field")
            };

            var selectedProperty = new SelectedProperty<TOwner, TProperty>(target, this);
            var nodePath = GetPathFromExpression(memberExpression);
            var currentNode = GetOrCreateNodeByPath(nodePath, configurationRoot);
            currentNode.AddChild(Node.Terminal<IPropertyConfigurator>(target.MemberName, selectedProperty));

            return selectedProperty;
        }

        public static PrintingConfigBuilder<TOwner> Default() => new PrintingConfigBuilder<TOwner>(new[]
        {
            typeof(int), typeof(double), typeof(float), typeof(string), typeof(bool),
            typeof(DateTime), typeof(TimeSpan), typeof(Guid)
        });

        private static IChildedNode<IPropertyConfigurator> GetOrCreateNodeByPath(IEnumerable<string> path,
            IChildedNode<IPropertyConfigurator> currentNode)
        {
            foreach (var part in path)
            {
                if (currentNode.TryGetChild(part, out var child))
                    currentNode.RemoveChild(part);
                child = Node.Childed<IPropertyConfigurator>(part);
                currentNode.AddChild(child);

                currentNode = (IChildedNode<IPropertyConfigurator>) child;
            }

            return currentNode;
        }

        private static List<string> GetPathFromExpression(MemberExpression memberExpression)
        {
            var path = new List<string>();
            var innerExpression = memberExpression.Expression;
            while (innerExpression is MemberExpression expr)
            {
                if (expr.Member is FieldInfo || expr.Member is PropertyInfo)
                {
                    path.Add(expr.Member.Name);
                    innerExpression = expr.Expression;
                }
                else break;
            }

            if (!(innerExpression is ParameterExpression))
                throw new ArgumentException($"Selector contain wrong expression inside: {innerExpression.Type}");
            return path;
        }
    }
}