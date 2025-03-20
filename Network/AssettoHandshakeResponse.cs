using AssettoNet.IO;

using System.IO;
using System.Threading.Tasks;

namespace AssettoNet.Network
{
    public class AssettoHandshakeResponse
    {
        public string CarName { get; set; }
        public string DriverName { get; set; }

        public int Identifier { get; set; }
        public int Version { get; set; }

        public string TrackName { get; set; }
        public string TrackConfig { get; set; }

        public AssettoHandshakeResponse()
        {
            CarName = string.Empty;
            DriverName = string.Empty;

            Identifier = 0;
            Version = 0;

            TrackName = string.Empty;
            TrackConfig = string.Empty;
        }

        public static async Task<AssettoHandshakeResponse> DeserializeAsync(byte[] data)
        {
            using var stream = new MemoryStream(data);

            // Strings are 100 bytes, 2 bytes per character (UTF-16)

            var carName = await stream.ReadStringAsync(100);
            var driverName = await stream.ReadStringAsync(100);

            var identifier = await stream.ReadIntAsync();
            var version = await stream.ReadIntAsync();

            var trackName = await stream.ReadStringAsync(100);
            var trackConfig = await stream.ReadStringAsync(100);

            return new AssettoHandshakeResponse()
            {
                CarName = carName,
                DriverName = driverName,
                Identifier = identifier,
                Version = version,
                TrackName = trackName,
                TrackConfig = trackConfig
            };
        }

        public override string ToString()
        {
            return $"Car: {CarName}, Driver: {DriverName}, " +
                $"Identifier: {Identifier}, Version: {Version}, " +
                $"Track: {TrackName}, Config: {TrackConfig}";
        }
    }
}
