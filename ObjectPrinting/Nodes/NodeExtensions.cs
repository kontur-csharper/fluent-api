using System.Linq;

namespace ObjectPrinting.Nodes
{
    public static class NodeExtensions
    {
        public static string GetFullPath<TPayload>(this INode<TPayload> node) =>
            node.Parent == null
                ? node.Name
                : $"{node.Parent.GetFullPath()}.{node.Name}";

        public static INode<TPayload>? GetByPathOrDefault<TPayload>(this IChildedNode<TPayload> node,
            params string[] pathParts)
        {
            if (pathParts.Length == 0 || !node.TryGetChild(pathParts[0], out var child))
                return default;

            if (pathParts.Length == 1)
                return child;

            return child is ChildedNode<TPayload> childed
                ? childed.GetByPathOrDefault(pathParts.Skip(1).ToArray())
                : default;
        }
    }
}