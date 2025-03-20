using AssettoNet.IO;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AssettoNet.Network
{
    public class AssettoSpotResponse
    {
        public int CarIdentifierNumber { get; set; }
        public int Lap { get; set; }

        public string DriverName { get; set; }
        public string CarName { get; set; }

        public TimeSpan Time { get; set; }

        public AssettoSpotResponse()
        {
            CarIdentifierNumber = 0;
            Lap = 0;

            DriverName = string.Empty;
            CarName = string.Empty;

            Time = TimeSpan.Zero;
        }

        public async static Task<AssettoSpotResponse> DeserializeAsync(byte[] data)
        {
            using var stream = new MemoryStream(data);

            var carIdentifierNumber = await stream.ReadIntAsync();
            var lap = await stream.ReadIntAsync();
            var driverName = await stream.ReadStringAsync(100);
            var carName = await stream.ReadStringAsync(100);
            var time = await stream.ReadIntAsync();

            return new AssettoSpotResponse()
            {
                CarIdentifierNumber = carIdentifierNumber,
                Lap = lap,
                DriverName = driverName,
                CarName = carName,
                Time = TimeSpan.FromMilliseconds(time)
            };
        }

        public override string ToString()
        {
            return $"Car Identifier: {CarIdentifierNumber}, " +
                   $"Lap: {Lap}, " +
                   $"Driver: {DriverName}, " +
                   $"Car: {CarName}, " +
                   $"Time: {Time}";
        }
    }
}
