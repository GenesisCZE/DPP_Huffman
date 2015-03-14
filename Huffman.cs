using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Huffman.PrintVisitors;
using System.Diagnostics;

namespace Huffman.PrintVisitors
{
    public interface IVisitedNode
    {
        void Accept(IVisitor v);
    }

    public interface IVisitor
    {
        void Visit(LeafNode node);

        void Visit(BranchNode node);
    }

    public class PrintVisitor : IVisitor, IDisposable
    {
        private readonly TextWriter _writer;
        private readonly List<Indent> _indents;


        public PrintVisitor(TextWriter writer)
        {
            _writer = writer;
            _indents = new List<Indent>();
        }


        public void Visit(LeafNode node)
        {
            _writer.Write(node);
            WriteEndLine();
        }

        public void Visit(BranchNode node)
        {
            _writer.Write("{0,4} -+- ", node);
            
            // First indent is SpaceSix, otherwise SpaceEight
            Add(_indents.Any() ? Indent.SpaceEight : Indent.SpaceSix);

            VisitNodeUsingIndent(node.RightSon, Indent.LineDown);

            _writer.Write(CreateStringFromIndents(Indent.LineDown));
            WriteEndLine();
            _writer.Write(CreateStringFromIndents(Indent.DownRight));

            VisitNodeUsingIndent(node.LeftSon, Indent.SpaceOne);
            // Remove last indent (SpaceEight or SpaceSix)
            RemoveLast();
        }

        /// <summary>
        /// Method calls methods Close and Dispose on <see cref="_writer"/>
        /// </summary>
        void IDisposable.Dispose()
        {
            _writer.Close();
            _writer.Dispose();
        }


        /// <summary>
        /// Method add <paramref name="indent"/> to <see cref="_indents"/>.
        /// Call Accept on <paramref name="node"/> and remove 
        /// last <see cref="Indent"/> from <see cref="_indents"/>
        /// </summary>
        /// <param name="node"></param>
        /// <param name="indent"></param>
        private void VisitNodeUsingIndent(Node node, Indent indent)
        {
            Add(indent);
            node.Accept(this);
            RemoveLast();
        }

        /// <summary>
        /// Add indent to <see cref="_indents"/>
        /// </summary>
        /// <param name="indent"></param>
        private void Add(Indent indent)
        {
            _indents.Add(indent);
        }

        /// <summary>
        /// Remove last indent from <see cref="_indents"/>
        /// </summary>
        private void RemoveLast()
        {
            _indents.RemoveAt(_indents.Count - 1);
        }

        /// <summary>
        /// Write end of line (this "\n").
        /// We are using this, because WriteLine writes "\n" or "\r\n"
        /// </summary>
        private void WriteEndLine()
        {
            _writer.Write("\n");
        }

        /// <summary>
        /// Create string from <see cref="_indents"/> and 
        /// <paramref name="lastIndent"/>
        /// </summary>
        /// <param name="lastIndent">
        /// last indent using for creating string
        /// </param>
        /// <returns></returns>
        private string CreateStringFromIndents(Indent lastIndent)
        {
            string result = "";
            Add(lastIndent);
            foreach (var indent in _indents)
            {
                switch (indent)
                {
                    case Indent.SpaceOne:
                        result += GetSpaces(1);
                        break;
                    case Indent.SpaceSix:
                        result += GetSpaces(6);
                        break;
                    case Indent.SpaceEight:
                        result += GetSpaces(8);
                        break;
                    case Indent.LineDown:
                        result += "|";
                        break;
                    case Indent.DownRight:
                        result += "`- ";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            RemoveLast();

            return result;
        }

        /// <summary>
        /// Create string contains only " ".
        /// </summary>
        /// <param name="count">Number of " " in the result</param>
        /// <returns>string contains " "</returns>
        private string GetSpaces(int count)
        {
            return "".PadLeft(count);
        }
    }
}

namespace Huffman
{
    public enum Indent
    {
        SpaceOne,
        SpaceSix,
        SpaceEight,
        LineDown,
        DownRight
    }

    public abstract class Node : IComparable<Node>, IVisitedNode
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


        int IComparable<Node>.CompareTo(Node other)
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
        public abstract void Accept(IVisitor v);


        protected abstract int Compare(Node other);
    }

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

    public class LeafNode : Node
    {
        private readonly byte _symbol;


        public LeafNode(byte symbol, ulong sum)
            : base(sum)
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
                ? string.Format(" ['{0}':{1}]", (char)Symbol, Sum)
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

    public class HuffmanTree
    {
        private readonly Dictionary<ulong, List<Node>> _nodes;
        private Node _root;

        private int _order;

        public HuffmanTree(Dictionary<byte, ulong> frequencies)
        {
            _nodes =
                frequencies
                    .Select(pair => (Node)new LeafNode(pair.Key, pair.Value))
                    .GroupBy(node => node.Sum)
                    .ToDictionary(nodes => nodes.Key, nodes => nodes.ToList());
        }


        public Node Root
        {
            get
            {
                if (_root == null)
                    BuildTree();

                return _root;
            }
        }


        private void BuildTree()
        {
            // count of all nodes
            var count = _nodes.SelectMany(pair => pair.Value).Count();

            while (count > 1)
            {
                // Get two nodes with minimal values
                var minNode = RemoveMinNode();
                var secondMinNode = RemoveMinNode();

                // Create new BranchNode
                var node = new BranchNode(minNode, secondMinNode, _order++);
                
                // Add new node to Dictionary
                if (_nodes.ContainsKey(node.Sum))
                    _nodes[node.Sum].Add(node);
                else
                    _nodes.Add(node.Sum, new List<Node> {node});
                count--;
            }

            // Set Root node
            _root = _nodes.Single().Value.Single();
        }


        /// <summary>
        /// Remove and return Node from <see cref="_nodes"/>.
        /// </summary>
        /// <returns></returns>
        private Node RemoveMinNode()
        {
            var minKey = _nodes.Keys.Min();
            var nodes = _nodes[minKey];
            var minNode = nodes.Min();
            nodes.Remove(minNode);
            if (!nodes.Any())
                _nodes.Remove(minKey);
            return minNode;
        }
    }

    public class Reader : IDisposable
    {
        private readonly BinaryReader _reader;

        private const int MaxBufferSize = 10000;


        public Reader(Stream stream)
        {
            _reader = new BinaryReader(stream);
        }


        /// <summary>
        /// Read bytes from <see cref="_reader"/> using buffer.
        /// </summary>
        /// <returns>
        /// <see cref="Dictionary{TKey,TValue}"/> 
        /// where TKey is readed symbol and TValue is count of that symbol
        /// </returns>
        public Dictionary<byte, ulong> ReadFileByteFrequencies()
        {
            SetReaderToStart();

            var frequencies = new Dictionary<byte, ulong>();
            while (_reader.BaseStream.Position != _reader.BaseStream.Length)
            {
                var difference =
                    _reader.BaseStream.Length - _reader.BaseStream.Position;
                var bufferSize = difference < MaxBufferSize 
                                    ? (int)difference 
                                    : MaxBufferSize;
                var buffer = _reader.ReadBytes(bufferSize);

                foreach (var readedByte in buffer)
                {
                    if (frequencies.ContainsKey(readedByte))
                        frequencies[readedByte]++;
                    else
                        frequencies.Add(readedByte, 1);
                }
            }

            return frequencies;
        }


        /// <summary>
        /// Set <see cref="_reader"/> to position 0.
        /// </summary>
        private void SetReaderToStart()
        {
            _reader.BaseStream.Position = 0;
        }

        void IDisposable.Dispose()
        {
            _reader.Close();
            _reader.Dispose();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (!ChechParameters(args))
            {
                Console.WriteLine("Argument Error");
                return;
            }
            Stream stream;
            if (!TryCreateStream(args[0], out stream))
            {
                Console.WriteLine("File Error");
                return;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Dictionary<byte, ulong> frequencies;
            using (var reader = new Reader(stream))
            { // Read frequencies from file
                frequencies = reader.ReadFileByteFrequencies();
            }

            // Get root node from tree
            var root = new HuffmanTree(frequencies).Root;

            // Print tree
            //using (var visitor = 
            //    new PrintVisitor(new StreamWriter("D:\\out.out")))
            using (var visitor = new PrintVisitor(Console.Out))
            {
                root.Accept(visitor);
                //Console.Write("\n");
            }

            stopwatch.Stop();
            Console.Write("Minutes :{0}\nSeconds :{1}\n Mili seconds :{2}", 
                stopwatch.Elapsed.Minutes, stopwatch.Elapsed.Seconds, 
                stopwatch.Elapsed.TotalMilliseconds);
            Console.ReadLine();
        }

        private static bool ChechParameters(string[] parameters)
        {
            return parameters.Length == 1;
        }

        private static bool TryCreateStream(string fileName, out Stream stream)
        {
            try
            {
                stream = File.OpenRead(fileName);
                return true;
            }
            catch (Exception)
            {
                stream = null;
                return false;
            }
        }
    }
}
