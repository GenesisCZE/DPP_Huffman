using Huffman.PrintVisitors;

namespace Huffman
{
    public class LeafNode : Node
    {
        private readonly byte _symbol;


        public LeafNode(byte symbol, ulong sum) : base(sum)
        {
            _symbol = symbol;
        }


        public byte Symbol
        {
            get { return _symbol; }
        }

        public override bool IsLeaf
        {
            get { return true; }
        }


        public override string ToString()
        {
            return IsPrintableCharacter(Symbol)
                ? string.Format(" ['{0}':{1}]", (char) Symbol, Sum)
                : string.Format(" [{0}:{1}]", Symbol, Sum);
        }

        public override void Accept(IVisitor v)
        {
            v.Visit(this);
        }


        protected override int Compare(Node other)
        {
            return Symbol - ((LeafNode)other).Symbol;
        }
        

        private bool IsPrintableCharacter(byte candidate)
        {
            return !(candidate < 32 || candidate > 126);
        }
    }
}