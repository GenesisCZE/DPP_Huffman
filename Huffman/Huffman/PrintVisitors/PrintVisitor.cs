using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Huffman.PrintVisitors
{
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
        /// Call Accept on <paramref name="node"/> and remove last <see cref="Indent"/> from <see cref="_indents"/>
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
        /// Create string from <see cref="_indents"/> and <paramref name="lastIndent"/>
        /// </summary>
        /// <param name="lastIndent">last indent using for creating string</param>
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