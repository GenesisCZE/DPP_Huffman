using Huffman.PrintVisitors;

namespace Huffman
{
    public class BranchNode : Node
    {
        private readonly Node _leftSon;
        private readonly Node _rightSon;

        private readonly int _order;


        public BranchNode(Node leftSon, Node rightSon, int order)
            : base(leftSon.Sum + rightSon.Sum)
        {
            _leftSon = leftSon;
            _rightSon = rightSon;
            _order = order;
        }


        public Node LeftSon
        {
            get { return _leftSon; }
        }

        public Node RightSon
        {
            get { return _rightSon; }
        }

        public int Order
        {
            get { return _order; }
        }

        public override bool IsLeaf
        {
            get { return false; }
        }


        public override string ToString()
        {
            return Sum.ToString();
        }

        public override void Accept(IVisitor v)
        {
            v.Visit(this);
        }

        protected override int Compare(Node other)
        {
            return Order - ((BranchNode)other).Order;
        }
    }
}