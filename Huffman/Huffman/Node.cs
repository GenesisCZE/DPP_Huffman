using System;

namespace Huffman
{
    public abstract class Node : IComparable<Node>
    {
        private readonly ulong _sum;


        protected Node(ulong sum)
        {
            _sum = sum;
        }


        public ulong Sum
        {
            get { return _sum; }
        }

        public abstract bool IsLeaf { get; }


        public int CompareTo(Node other)
        {
            //By sum
            if (Sum < other.Sum)
                return -1;
            if (Sum > other.Sum)
                return 1;

            // Same type (both are leafs or not)
            if (IsLeaf == other.IsLeaf)
                return Compare(other);

            // One is leaf
            return IsLeaf ? -1 : 1;
        }


        protected abstract int Compare(Node other);
    }
}