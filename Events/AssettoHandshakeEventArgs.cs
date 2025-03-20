using AssettoNet.Network;

using System;

namespace AssettoNet.Events
{
    public class AssettoHandshakeEventArgs : EventArgs
    {
        public AssettoHandshakeResponse Response { get; }

        public AssettoHandshakeEventArgs(AssettoHandshakeResponse response)
        {
            this.Response = response;
        }
    }
}
