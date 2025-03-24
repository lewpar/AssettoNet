using AssettoNet.Network.Struct;

using System;

namespace AssettoNet.Events
{
    /// <summary>
    /// Contains information about the track and vehicle for the current session.
    /// </summary>
    public class AssettoConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Contains information about the track and vehicle for the current session.
        /// </summary>
        public AssettoHandshakeData Handshake { get; set; }

        /// <summary>
        /// Contains information about the track and vehicle for the current session.
        /// </summary>
        public AssettoConnectedEventArgs(AssettoHandshakeData response)
        {
            Handshake = response;
        }
    }
}
