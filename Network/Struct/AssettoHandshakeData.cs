using AssettoNet.IO;
using System;
using System.Runtime.InteropServices;

namespace AssettoNet.Network.Struct
{
    /// <summary>
    /// Contains information about the track and vehicle for the current session.
    /// </summary>
    public struct AssettoHandshakeData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        private byte[] carName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        private byte[] driverName;

        private int identifier;
        private int version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        private byte[] trackName;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        private byte[] trackConfig;

        /// <summary>
        /// Gets the name of the vehicle being driven.
        /// </summary>
        public string CarName { get => carName.GetAssettoUnicodeString(); }

        /// <summary>
        /// Gets the name of the driver.
        /// </summary>
        public string DriverName { get => driverName.GetAssettoUnicodeString(); }

        /// <summary>
        /// Gets the name of the track being driven.
        /// </summary>
        public string TrackName { get => trackName.GetAssettoUnicodeString(); }

        /// <summary>
        /// UNDOCUMENTED
        /// </summary>
        public string TrackConfig { get => trackConfig.GetAssettoUnicodeString(); }

        /// <returns>
        /// A formatted string containing the all of the handshake data.
        /// </returns>
        public override string ToString()
        {
            return $"Car: {CarName}, Driver: {DriverName}, " + Environment.NewLine +
                $"Track: {TrackName}, Config: {TrackConfig}";
        }
    }
}
