using AssettoNet.Events;
using AssettoNet.IO;
using AssettoNet.Network.Struct;

using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
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
        /// This event is fired when the client DisconnectAsync is called.
        /// </summary>
        public event EventHandler<EventArgs>? OnDisconnected;

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
        /// Gets the current state of the UDP connection.
        /// </summary>
        public bool IsConnected => _isConnected;

        private bool _isConnected;
        private bool _isDisconnecting;
        
        private string _listeningHost;
        private int _listeningPort;

        private UdpClient _updateClient;
        private UdpClient _spotClient;
        
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// A UDP client for connecting to the Assetto Corsa UDP telemetry server.
        /// </summary>
        /// <param name="host">The hostname or IP address of the machine running Assetto Corsa.</param>
        /// <param name="port">The port of the Assetto Corsa UDP server (default: 9996).</param>
        public AssettoClient(string host, int port = 9996)
        {
            _updateClient = new UdpClient();
            _spotClient = new UdpClient();

            _isConnected = false;
            _isDisconnecting = false;
            
            _listeningHost = host;
            _listeningPort = port;
            
            _cancellationTokenSource = new CancellationTokenSource();
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

            _isDisconnecting = false;
            
            // Dispose of the old clients if they are active
            _updateClient?.Dispose();
            _spotClient?.Dispose();
            
            _updateClient = new UdpClient();
            _spotClient = new UdpClient();
            
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

            _cancellationTokenSource = new CancellationTokenSource();
            
            await Task.WhenAll(
                Task.Run(() => SpotLoopAsync(_cancellationTokenSource.Token)), 
                Task.Run(() => UpdateLoopAsync(_cancellationTokenSource.Token)), 
                Task.Run(() => ServerListenStateChangedLoop()));
        }

        /// <summary>
        /// Unsubscribes from telemetry events and stops the event loop.  
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task DisconnectAsync()
        {
            if (!_isConnected ||
                _isDisconnecting)
            {
                return;
            }

            _isDisconnecting = true;
            
            _cancellationTokenSource?.Cancel();

            if (IsAssettoUdpServerListening())
            {
                await UnsubscribeAsync(_spotClient);
                await UnsubscribeAsync(_updateClient);   
            }

            _isConnected = false;
            
            OnDisconnected?.Invoke(this, new EventArgs());
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

        private async Task UpdateLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                var updatePacketLength = 328;
                var buffer = new ByteBuffer(updatePacketLength * 5);

                while (!cancellationToken.IsCancellationRequested)
                {
                    var resultTask = _updateClient.ReceiveAsync();
                    var completedTask = await Task.WhenAny(resultTask, Task.Delay(-1, cancellationToken));

                    if (completedTask == resultTask)
                    {
                        var result = await resultTask;
                        buffer.Write(result.Buffer);

                        if (buffer.AvailableBytes >= updatePacketLength)
                        {
                            var receivedData = buffer.Read(updatePacketLength);
                            buffer.Normalize();

                            var updateData = receivedData.ToStruct<AssettoUpdateData>();

                            OnPhysicsUpdate?.Invoke(this, new AssettoPhysicsUpdateEventArgs(updateData));
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (SocketException ex)
            {
                await DisconnectAsync();
                OnUnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
            }
            catch (Exception ex)
            {
                OnUnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
            }
        }

        private async Task SpotLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                var spotPacketLength = 212;
                var buffer = new ByteBuffer(spotPacketLength * 5);

                while (!cancellationToken.IsCancellationRequested)
                {
                    var resultTask = _spotClient.ReceiveAsync();
                    var completedTask = await Task.WhenAny(resultTask, Task.Delay(-1, cancellationToken));

                    if (completedTask == resultTask)
                    {
                        var result = await resultTask;
                        buffer.Write(result.Buffer);

                        if (buffer.AvailableBytes >= spotPacketLength)
                        {
                            var receivedData = buffer.Read(spotPacketLength);
                            buffer.Normalize();

                            var spotData = receivedData.ToStruct<AssettoSpotData>();

                            OnLapCompleted?.Invoke(this, new AssettoLapCompletedEventArgs(spotData));
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (SocketException ex)
            {
                await DisconnectAsync();
                OnUnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
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
                if (!currentListenerState)
                {
                    await DisconnectAsync();
                }
            }
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
            _updateClient?.Dispose();
            _spotClient?.Dispose();

            _isConnected = false;
            _isDisconnecting = false;
        }
    }
}
