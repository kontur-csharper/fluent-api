using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectPrinting.Nodes
{
    public class ChildedNode<TPayload> : IChildedNode<TPayload>
    {
        private readonly IDictionary<string, INode<TPayload>> childNodes = new Dictionary<string, INode<TPayload>>();

        public ChildedNode(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public IChildedNode<TPayload>? Parent { get; set; }

        public void RemoveChild(string name)
        {
            if(childNodes.TryGetValue(name, out var existing))
            {
                childNodes.Remove(name);
                existing.Parent = null;
            }
        }

        public bool TryGetChild(string childName, out INode<TPayload> child) =>
            childNodes.TryGetValue(childName, out child);

        public void AddChild(INode<TPayload> child)
        {
            childNodes.Add(child.Name, child);
            child.Parent = this;
        }

        public IEnumerable<INode<TPayload>> GetChildNodes() => childNodes.Select(d => d.Value);

        public INode<TPayload> this[string childName] => TryGetChild(childName, out var child)
            ? child
            : throw new ArgumentException($"Node {Name} does not contain child with name {childName}");
    }
}