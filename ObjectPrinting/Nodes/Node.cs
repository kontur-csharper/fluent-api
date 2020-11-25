namespace ObjectPrinting.Nodes
{
    public static class Node
    {
        public static ChildedNode<TPayload> Childed<TPayload>(string name) => new ChildedNode<TPayload>(name);
        public static TerminalNode<TPayload> Terminal<TPayload>(string name, TPayload payload) =>
            new TerminalNode<TPayload>(name, payload);
    }
}