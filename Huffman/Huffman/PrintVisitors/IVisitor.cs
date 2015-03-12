namespace Huffman.PrintVisitors
{
    public interface IVisitor
    {
        void Visit(LeafNode node);

        void Visit(BranchNode node);
    }
}