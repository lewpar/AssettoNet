using System;
using System.IO;
using System.Threading.Tasks;

namespace AssettoNet.Network
{
    internal class AssettoOperationRequest
    {
        public int Identifier { get; set; }
        public int Version { get; set; }
        public AssettoOperationId OperationId { get; set; }

        public async Task<byte[]> SerializeAsync()
        {
            using var ms = new MemoryStream();

            await ms.WriteAsync(BitConverter.GetBytes(Identifier));
            await ms.WriteAsync(BitConverter.GetBytes(Version));
            await ms.WriteAsync(BitConverter.GetBytes((int)OperationId));

            return ms.ToArray();
        }
    }
}
