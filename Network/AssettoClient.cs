using AssettoNet.Events;

using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace AssettoNet.Network
{
    public class AssettoClient
    {
        public event EventHandler<EventArgs>? OnClientConnected;
        public event EventHandler<AssettoHandshakeEventArgs>? OnClientHandshake;
        public event EventHandler<AssettoOperationEventArgs>? OnClientOperationEvent;
        public event EventHandler<EventArgs>? OnClientListening;

        private UdpClient _client;
        private bool _isListening;
        private bool _hasSubscribed;

        public AssettoClient()
        {
            _client = new UdpClient();
            _isListening = false;
            _hasSubscribed = false;
        }

        public async Task ConnectAsync(string host, int port)
        {
            _client.Connect(host, port);

            await SendHandshakeAsync();
            await ReadHandshakeAsync();

            OnClientConnected?.Invoke(this, EventArgs.Empty);
        }

        public void Disconnect()
        {
            _isListening = false;
        }

        private async Task SendHandshakeAsync()
        {
            var handshakeRequest = new AssettoOperationRequest()
            {
                Identifier = 1,
                Version = 1,
                OperationId = AssettoOperationId.HANDSHAKE
            };

            var packet = await handshakeRequest.SerializeAsync();

            await _client.SendAsync(packet, packet.Length);
        }

        private async Task ReadHandshakeAsync()
        {
            var result = await _client.ReceiveAsync();
            var response = await AssettoHandshakeResponse.DeserializeAsync(result.Buffer);

            OnClientHandshake?.Invoke(this, new AssettoHandshakeEventArgs(response));
        }

        private async Task SubscribeAsync(AssettoEventType eventType)
        {
            AssettoOperationRequest request;

            if(eventType == AssettoEventType.Spot)
            {
                request = new AssettoOperationRequest()
                {
                    Identifier = 1,
                    Version = 1,
                    OperationId = AssettoOperationId.SUBSCRIBE_SPOT
                };
            }
            else if(eventType == AssettoEventType.Update)
            {
                request = new AssettoOperationRequest()
                {
                    Identifier = 1,
                    Version = 1,
                    OperationId = AssettoOperationId.SUBSCRIBE_UPDATE
                };
            }
            else
            {
                throw new Exception("Invalid event type.");
            }

            var packet = await request.SerializeAsync();
            await _client.SendAsync(packet, packet.Length);

            _hasSubscribed = true;
        }

        private async Task UnsubscribeAsync()
        {
            var request = new AssettoOperationRequest()
            {
                Identifier = 1,
                Version = 1,
                OperationId = AssettoOperationId.DISMISS
            };

            var packet = await request.SerializeAsync();
            await _client.SendAsync(packet, packet.Length);
        }

        public async Task ListenForEventsAsync(AssettoEventType eventType)
        {
            if(_hasSubscribed)
            {
                await UnsubscribeAsync();
            }

            await SubscribeAsync(eventType);

            _isListening = true;

            OnClientListening?.Invoke(this, new EventArgs());

            while (_isListening)
            {
                var result = await _client.ReceiveAsync();
                if(result.Buffer.Length < 2)
                {
                    throw new Exception("Received buffer from server lower than expected length of 2.");
                }

                var magic = (char)result.Buffer[0];

                if(magic == 'a')
                {
                    await ProcessUpdateEventAsync(result.Buffer);
                }
                else
                {
                    await ProcessSpotEventAsync(result.Buffer);
                }
            }
        }

        private async Task ProcessUpdateEventAsync(byte[] data)
        {
            var response = await AssettoUpdateResponse.DeserializeAsync(data);

            OnClientOperationEvent?.Invoke(this, new AssettoOperationEventArgs()
            {
                EventType = AssettoEventType.Update,
                Update = response
            });
        }

        private async Task ProcessSpotEventAsync(byte[] data)
        {
            var response = await AssettoSpotResponse.DeserializeAsync(data);

            OnClientOperationEvent?.Invoke(this, new AssettoOperationEventArgs()
            {
                EventType = AssettoEventType.Spot,
                Spot = response
            });
        }
    }
}
