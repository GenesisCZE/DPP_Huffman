using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Huffman
{
    public class Writer : IDisposable
    {
        private readonly TextWriter _writer;

        public Writer(TextWriter writer)
        {
            _writer = writer;
        }

        ///// <summary>
        ///// Prevzaty z toho hrozneho kodu
        ///// </summary>
        ///// <param name="node"></param>
        ///// <param name="pre"></param>
        //public void WriteTree(Node node, string pre)
        //{
        //    var branchNode = node as BranchNode;
        //    if (branchNode != null)
        //    {
        //        _writer.Write("{0,4} -+- ", branchNode);
        //        pre = pre + "      ";
        //        WriteTree(branchNode.RightSon, pre + "|  ");
        //        _writer.Write("{0}|\n", pre);
        //        _writer.Write("{0}`- ", pre);
        //        WriteTree(branchNode.LeftSon, pre + "   ");
        //        return;
        //    }

        //    _writer.WriteLine(node);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="indents"></param>
        public void WriteTree(Node node, IEnumerable<Indent> indents)
        {
            var branchNode = node as BranchNode;
            if (branchNode != null)
            {
                _writer.Write("{0,4} -+- ", branchNode);
                indents = indents.Any()
                    ? indents.Concat(new[] {Indent.SpaceEight}).ToList()
                    : new List<Indent> {Indent.SpaceSix};
                WriteTree(branchNode.RightSon, indents.Concat(new[] {Indent.LineDown}));
                _writer.Write("{0}\n", GetIndent(indents.Concat(new[] {Indent.LineDown})));
                _writer.Write(GetIndent(indents.Concat(new[] { Indent.DownRight })));
                WriteTree(branchNode.LeftSon, indents.Concat(new[] {Indent.SpaceOne}));
                return;
            }

            _writer.WriteLine(node);
        }


        private string GetIndent(IEnumerable<Indent> indents)
        {
            string result = "";
            foreach (var indent in indents)
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

        public void Dispose()
        {
            _writer.Close();
            _writer.Dispose();
        }
    }

    public enum Indent
    {
        SpaceOne,
        SpaceSix,
        SpaceEight,
        LineDown,
        DownRight
    }
}