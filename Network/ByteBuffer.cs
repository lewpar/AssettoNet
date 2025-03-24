using System;

namespace AssettoNet.Network
{
    internal class ByteBuffer
    {
        public int AvailableBytes { get => _writePos; }
        public int Size { get => _buffer.Length; }

        private byte[] _buffer;

        private int _writePos;
        private int _readPos;

        public ByteBuffer(int size)
        {
            if(size < 1)
            {
                throw new ArgumentException("Size must be greater than or equal to 1.", nameof(size));
            }

            _buffer = new byte[size];
        }

        public void Write(byte[] data)
        {
            if(_writePos + data.Length > _buffer.Length)
            {
                throw new InvalidOperationException("Not enough space available in buffer to write.");
            }

            Buffer.BlockCopy(data, 0, _buffer, _writePos, data.Length);

            _writePos += data.Length;
        }

        public byte[] Read(int length)
        {
            if (length < 1)
            {
                throw new ArgumentException("Length must be greater than or equal to 1.", nameof(length));
            }

            if (length > AvailableBytes)
            {
                throw new InvalidOperationException("Not enough data available in buffer to read.");
            }

            var buffer = new byte[length];

            Buffer.BlockCopy(_buffer, _readPos, buffer, 0, length);
            _readPos += length;

            return buffer;
        }

        /// <summary>
        /// Normalizes the buffer by shifting the remaining data from the read position to the start of the buffer.
        /// This operation ensures that any data that has already been read is discarded.
        /// </summary>
        public void Normalize()
        {
            int remainingBytes = _writePos - _readPos;
            if (remainingBytes > 0)
            {
                Buffer.BlockCopy(_buffer, _readPos, _buffer, 0, remainingBytes);
            }

            _writePos = remainingBytes;
            _readPos = 0;
        }

        public void Reset()
        {
            _writePos = 0;
            _readPos = 0;
        }
    }
}
