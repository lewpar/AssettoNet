using AssettoNet.Events;
using AssettoNet.IO;
using AssettoNet.Network.Struct;

using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace AssettoNet.Network
{
    /// <summary>
    /// A UDP client for connecting to the Assetto Corsa UDP telemetry server.
    /// </summary>
    public class AssettoClient : IDisposable
    {
        /// <summary>
        /// This event is fired when the client has passed handshake with the Assetto Telemetry server.
        /// </summary>
        public event EventHandler<AssettoConnectedEventArgs>? OnConnected;

        /// <summary>
        /// This event is fired for every physics update.
        /// </summary>
        public event EventHandler<AssettoPhysicsUpdateEventArgs>? OnPhysicsUpdate;

        /// <summary>
        /// This event is fired when a track lap is completed.
        /// </summary>
        public event EventHandler<AssettoLapCompletedEventArgs>? OnLapCompleted;

        /// <summary>
        /// This event is fired when an unhandled exception occurs in the event loops.
        /// </summary>
        public event EventHandler<UnhandledExceptionEventArgs>? OnUnhandledException;
        
        /// <summary>
        /// This event is fired when the UDP Server Listener state has changed from open to closed.
        /// </summary>
        public event EventHandler<EventArgs>? OnServerListenerClosed;

        private bool _isListening;
        private bool _isConnected;
        
        private bool _lastListenerState;
        private string _listeningHost;
        private int _listeningPort;

        private UdpClient _updateClient;
        private UdpClient _spotClient;

        /// <summary>
        /// A UDP client for connecting to the Assetto Corsa UDP telemetry server.
        /// </summary>
        /// <param name="host">The hostname or IP address of the machine running Assetto Corsa.</param>
        /// <param name="port">The port of the Assetto Corsa UDP server (default: 9996).</param>
        public AssettoClient(string host, int port = 9996)
        {
            _updateClient = new UdpClient();
            _spotClient = new UdpClient();

            _isListening = false;
            _isConnected = false;
            
            _lastListenerState = false;
            _listeningHost = host;
            _listeningPort = port;
        }

        /// <summary>
        /// Establishes a connection to the Assetto Corsa UDP server, performing a handshake and starting the listener for spot and update events.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ConnectAsync()
        {
            if (_isConnected)
            {
                return;
            }
            
            _spotClient.Connect(_listeningHost, _listeningPort);
            _updateClient.Connect(_listeningHost, _listeningPort);

            await SendHandshakeAsync(_spotClient);
            await SendHandshakeAsync(_updateClient);

            // Discard the first one since they will both be the same handshake data.
            _ = await ReadHandshakeAsync(_spotClient);
            var handshake = await ReadHandshakeAsync(_updateClient);

            await SubscribeAsync(_spotClient, AssettoEventType.Spot);
            await SubscribeAsync(_updateClient, AssettoEventType.Update);

            _isConnected = true;

            OnConnected?.Invoke(this, new AssettoConnectedEventArgs(handshake));

            await ListenForEventsAsync();
        }

        /// <summary>
        /// Unsubscribes from telemetry events and stops the event loop.  
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task DisconnectAsync()
        {
            if (!_isConnected)
            {
                return;
            }
            
            await UnsubscribeAsync(_spotClient);
            await UnsubscribeAsync(_updateClient);

            _isListening = false;
            _isConnected = false;
        }

        private async Task SendHandshakeAsync(UdpClient client)
        {
            var handshakeRequest = new AssettoOperationRequest()
            {
                Identifier = 1,
                Version = 1,
                OperationId = AssettoOperationId.HANDSHAKE
            };

            var packet = await handshakeRequest.SerializeAsync();

            await client.SendAsync(packet, packet.Length);
        }

        private async Task<AssettoHandshakeData> ReadHandshakeAsync(UdpClient client)
        {
            var result = await client.ReceiveAsync();
            var data = result.Buffer;

            return data.ToStruct<AssettoHandshakeData>();
        }

        private async Task SubscribeAsync(UdpClient client, AssettoEventType eventType)
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
            await client.SendAsync(packet, packet.Length);
        }

        private async Task UnsubscribeAsync(UdpClient client)
        {
            var request = new AssettoOperationRequest()
            {
                Identifier = 1,
                Version = 1,
                OperationId = AssettoOperationId.DISMISS
            };

            var packet = await request.SerializeAsync();
            await client.SendAsync(packet, packet.Length);
        }

        private async Task UpdateLoopAsync()
        {
            try
            {
                var updatePacketLength = 328;
                var buffer = new ByteBuffer(updatePacketLength * 5);

                while (_isListening)
                {
                    var result = await _updateClient.ReceiveAsync();
                    buffer.Write(result.Buffer);

                    if(buffer.AvailableBytes >= updatePacketLength)
                    {
                        var receivedData = buffer.Read(updatePacketLength);
                        buffer.Normalize();

                        var updateData = receivedData.ToStruct<AssettoUpdateData>();

                        OnPhysicsUpdate?.Invoke(this, new AssettoPhysicsUpdateEventArgs(updateData));
                    }
                }
            }
            catch (Exception ex)
            {
                OnUnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
            }
        }

        private async Task SpotLoopAsync()
        {
            try
            {
                var spotPacketLength = 212;
                var buffer = new ByteBuffer(spotPacketLength * 5);

                while (_isListening)
                {
                    var result = await _spotClient.ReceiveAsync();
                    buffer.Write(result.Buffer);

                    if (buffer.AvailableBytes >= spotPacketLength)
                    {
                        var receivedData = buffer.Read(spotPacketLength);
                        buffer.Normalize();

                        var spotData = receivedData.ToStruct<AssettoSpotData>();

                        OnLapCompleted?.Invoke(this, new AssettoLapCompletedEventArgs(spotData));
                    }
                }
            }
            catch (Exception ex)
            {
                OnUnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
            }
        }

        private async Task ServerListenStateChangedLoop()
        {
            while (_isConnected)
            {
                var currentListenerState = IsAssettoUdpServerListening();
                
                if (_lastListenerState != currentListenerState)
                {
                    // The UDP server is no longer running, disconnect.
                    if (!currentListenerState)
                    {
                        await DisconnectAsync();
                    }
                    
                    OnServerListenerClosed?.Invoke(this, new EventArgs());
                }
                
                _lastListenerState = currentListenerState;
            }
        }
        
        private async Task ListenForEventsAsync()
        {
            _isListening = true;

            await Task.WhenAll(Task.Run(SpotLoopAsync), Task.Run(UpdateLoopAsync), Task.Run(ServerListenStateChangedLoop));
        }

        /// <summary>
        /// Checks if the Assetto UDP server listener is active/inactive.
        /// </summary>
        /// <returns>The current state of the UDP listener.</returns>
        public bool IsAssettoUdpServerListening()
        {
            var ipProps = IPGlobalProperties.GetIPGlobalProperties();
            var listeners = ipProps.GetActiveUdpListeners();

            return listeners.Any(endpoint => endpoint.Port == _listeningPort);
        }

        /// <summary>
        /// Disposes of the internal Update and Spot UDP clients.
        /// </summary>
        public void Dispose()
        {
            _updateClient.Dispose();
            _spotClient.Dispose();
        }
    }
}
