namespace ObjectPrinting.Nodes
{
    public interface INode<TPayload>
    {
        /// <summary>
        /// Node that contains current node. Can be null when node is root.
        /// </summary>
        IChildedNode<TPayload>? Parent { get; set; }

        string Name { get; }
    }
}