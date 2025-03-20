using AssettoNet.Network;

using System;

namespace AssettoNet.Events
{
    public class AssettoOperationEventArgs : EventArgs
    {
        public AssettoEventType EventType { get; set; }

        public AssettoSpotResponse? Spot { get; set; }
        public AssettoUpdateResponse? Update { get; set; }
    }
}
