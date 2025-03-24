using AssettoNet.IO;

using System;
using System.Runtime.InteropServices;

namespace AssettoNet.Network.Struct
{
    /// <summary>
    /// Contains information about the completed lap.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct AssettoSpotData
    {
        private int carIdentifierNumber;
        private int lap;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        private byte[] driverName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        private byte[] carName;

        private int time;

        /// <summary>
        /// Gets the name of the driver.
        /// </summary>
        public string DriverName { get => driverName.GetAssettoUnicodeString(); }

        /// <summary>
        /// Gets the name of the car.
        /// </summary>
        public string CarName { get => carName.GetAssettoUnicodeString(); }

        /// <summary>
        /// Gets the car identifier number.
        /// </summary>
        public int CarIdentifier { get => carIdentifierNumber; }

        /// <summary>
        /// Get the lap that was completed.
        /// </summary>
        public int Lap { get => lap; }

        /// <summary>
        /// Gets the lap time.
        /// </summary>
        public TimeSpan Time { get => TimeSpan.FromMilliseconds(time); }

        /// <returns>
        /// A formatted string containing the driver's name, car name, car identifier, lap number, and time.
        /// </returns>
        public override string ToString()
        {
            return $"Driver: {DriverName}" + Environment.NewLine +
                $"Car: {CarName}" + Environment.NewLine +
                $"Car Id: {CarIdentifier}" + Environment.NewLine +
                $"Lap: {Lap}" + Environment.NewLine +
                $"Time: {Time}";
        }
    }
}
