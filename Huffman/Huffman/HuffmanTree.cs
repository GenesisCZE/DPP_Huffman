using System.Collections.Generic;
using System.Linq;

namespace Huffman
{
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
                var branchNode = new BranchNode(minNode, secondMinNode, _order++);
                
                // Add new node to Dictionary
                if (_nodes.ContainsKey(branchNode.Sum))
                    _nodes[branchNode.Sum].Add(branchNode);
                else
                    _nodes.Add(branchNode.Sum, new List<Node> {branchNode});
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
}