using System;
using System.Collections.Generic;
using System.IO;

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


            Dictionary<byte, ulong> frequencies;
            using (var reader = new Reader(stream))
            {
                frequencies = reader.ReadFileByteFrequencies();
            }
            var root = new HuffmanTree(frequencies).Root;

                //var writer = new Writer(new StreamWriter("D:\\out.out"));
            using (var writer = new Writer(Console.Out))
            {
                writer.WriteTree(root, new List<Indent>());
            }
            //var writer = new Writer(Console.Out);
            //    writer.WriteTree(root, new List<Indent>());
            //    writer.Dispose();

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
