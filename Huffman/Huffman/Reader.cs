using System;
using System.Collections.Generic;
using System.IO;

namespace Huffman
{
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
        /// <returns></returns>
        public Dictionary<byte, ulong> ReadFileByteFrequencies()
        {
            SetReaderToStart();

            var frequencies = new Dictionary<byte, ulong>();
            while (_reader.BaseStream.Position != _reader.BaseStream.Length)
            {
                var difference = _reader.BaseStream.Length - _reader.BaseStream.Position;
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

        public void Dispose()
        {
            _reader.Close();
            _reader.Dispose();
        }
    }
}
