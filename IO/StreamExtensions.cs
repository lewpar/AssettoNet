using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AssettoNet.IO
{
    internal static class StreamExtensions
    {
        public static async Task<bool> ReadBooleanAsync(this MemoryStream ms)
        {
            var buf = new byte[1];
            int bytesRead = await ms.ReadAsync(buf, 0, 1);

            if (bytesRead == 0)
                throw new EndOfStreamException("Unexpected end of stream while reading a bool.");

            return buf[0] != 0;
        }

        public static async Task<byte> ReadByteAsync(this MemoryStream ms)
        {
            var buf = new byte[1];
            int bytesRead = await ms.ReadAsync(buf, 0, 1);

            if (bytesRead == 0)
                throw new EndOfStreamException("Unexpected end of stream while reading a byte.");

            return buf[0];
        }

        public static async Task<char> ReadCharAsync(this MemoryStream ms)
        {
            var buf = new byte[1];
            int bytesRead = await ms.ReadAsync(buf, 0, 1);

            if (bytesRead == 0)
                throw new EndOfStreamException("Unexpected end of stream while reading a char.");

            return (char)buf[0];
        }

        public static async Task<int> ReadIntAsync(this MemoryStream ms)
        {
            var buf = new byte[sizeof(int)];
            await ms.ReadAsync(buf, 0, sizeof(int));
            return BitConverter.ToInt32(buf, 0);
        }

        public static async Task<float> ReadFloatAsync(this MemoryStream ms)
        {
            var buf = new byte[sizeof(float)];
            await ms.ReadAsync(buf, 0, sizeof(float));
            return BitConverter.ToSingle(buf, 0);
        }

        public static async Task<float[]> ReadFloatArrayAsync(this MemoryStream ms, int length)
        {
            var buf = new byte[length * sizeof(float)];
            await ms.ReadAsync(buf, 0, buf.Length);

            var result = new float[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = BitConverter.ToSingle(buf, i * sizeof(float));
            }
            return result;
        }

        public static async Task<string> ReadStringAsync(this MemoryStream ms, int length)
        {
            var buf = new byte[length];
            await ms.ReadAsync(buf, 0, length);

            var result = Encoding.Unicode.GetString(buf);

            int percentIndex = result.IndexOf('%'); // Each string is terminated with a % in the responses
            if (percentIndex >= 0)
            {
                result = result.Substring(0, percentIndex);
            }

            return result;
        }
    }
}
