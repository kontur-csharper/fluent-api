using System.Collections.Generic;

namespace ObjectPrinting.Nodes
{
    /// <summary>
    /// Node, containing childs
    /// </summary>
    public interface IChildedNode<TPayload> : INode<TPayload>
    {
        void RemoveChild(string name);
        bool TryGetChild(string childName, out INode<TPayload> child);
        void AddChild(INode<TPayload> child);
        IEnumerable<INode<TPayload>> GetChildNodes();
        INode<TPayload> this[string childName] { get; }
    }
}