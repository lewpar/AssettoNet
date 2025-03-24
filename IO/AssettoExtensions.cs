using AssettoNet.Network.Struct;
using System.Runtime.InteropServices;
using System.Text;

namespace AssettoNet.IO
{
    internal static class AssettoExtensions
    {
        /// <summary>
        /// Converts a byte array containing Unicode-encoded data into a string,  
        /// removing any characters following and including the '%' terminator.
        /// </summary>
        /// <param name="data">The byte array containing Unicode-encoded text.</param>
        /// <returns>The decoded Unicode string with any content after (and including) '%' removed.</returns>
        public static string GetAssettoUnicodeString(this byte[] data)
        {
            var result = Encoding.Unicode.GetString(data);

            int percentIndex = result.IndexOf('%'); // Each string is terminated with a % in the assetto responses
            if (percentIndex >= 0)
            {
                result = result.Substring(0, percentIndex);
            }

            return result;
        }

        public static T ToStruct<T>(this byte[] data) where T : struct
        {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            }
            catch
            {
                throw;
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
