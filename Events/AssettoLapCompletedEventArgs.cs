using AssettoNet.Network.Struct;

using System;

namespace AssettoNet.Events
{
    /// <summary>
    /// Contains information about the completed lap.
    /// </summary>
    public class AssettoLapCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Contains information about the completed lap.
        /// </summary>
        public AssettoSpotData Data { get; set; }

        /// <summary>
        /// Contains information about the completed lap.
        /// </summary>
        public AssettoLapCompletedEventArgs(AssettoSpotData response)
        {
            Data = response;
        }
    }
}
