using AssettoNet.Network.Struct;

using System;

namespace AssettoNet.Events
{
    /// <summary>
    /// Contains telemetry about the vehicle.
    /// </summary>
    public class AssettoPhysicsUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// Contains telemetry about the vehicle.
        /// </summary>
        public AssettoUpdateData Data { get; set; }

        /// <summary>
        /// Contains telemetry about the vehicle.
        /// </summary>
        public AssettoPhysicsUpdateEventArgs(AssettoUpdateData response)
        {
            Data = response;
        }
    }
}
