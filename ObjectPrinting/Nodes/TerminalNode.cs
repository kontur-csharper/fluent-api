namespace ObjectPrinting.Nodes
{
    public class TerminalNode<TPayload> : INode<TPayload>
    {
        internal TerminalNode(string name, TPayload payload)
        {
            Name = name;
            Payload = payload;
        }

        public string Name { get; }
        public IChildedNode<TPayload>? Parent { get; set; }
        public TPayload Payload { get; }
    }
}