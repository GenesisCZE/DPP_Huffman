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
            if (IsPrintableCharacter(Symbol))
                return string.Format(" ['{0}':{1}]", (char)Symbol, Sum);
            return string.Format(" [{0}:{1}]", Symbol, Sum);
        }


        protected override int Compare(Node other)
        {
            return Symbol < ((LeafNode)other).Symbol ? -1 : 1;
        }
        

        private bool IsPrintableCharacter(byte candidate)
        {
            return !(candidate < 32 || candidate > 126);
        }
    }
}