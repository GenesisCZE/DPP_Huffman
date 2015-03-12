using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Huffman.PrintVisitors;

namespace Huffman
{
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

            //var stopwatch = new Stopwatch();
            //stopwatch.Start();

            Dictionary<byte, ulong> frequencies;
            using (var reader = new Reader(stream))
            { // Read frequencies from file
                frequencies = reader.ReadFileByteFrequencies();
            }

            // Get root node from tree
            var root = new HuffmanTree(frequencies).Root;

            // Print tree
            //using (var visitor = new PrintVisitor(new StreamWriter("D:\\out.out")))
            using (var visitor = new PrintVisitor(Console.Out))
            {
                root.Accept(visitor);
                //Console.Write("\n");
            }

            //stopwatch.Stop();
            //Console.Write("Minutes :{0}\nSeconds :{1}\n Mili seconds :{2}", stopwatch.Elapsed.Minutes,
            //    stopwatch.Elapsed.Seconds, stopwatch.Elapsed.TotalMilliseconds);
            //Console.ReadLine();
        }

        private static bool ChechParameters(string[] parameters)
        {
            return parameters.Length == 1;
        }

        private static bool TryCreateStream(string fileName, out Stream stream)
        {
            try
            {
                stream = File.Open(fileName, FileMode.Open);
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
