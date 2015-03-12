namespace Huffman.PrintVisitors
{
    public interface IVisitedNode
    {
        void Accept(IVisitor v);
    }
}