using System;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronSockets;
using QscQsys.ModuleFramework.Events;
using QscQsys.ModuleFramework.Logging;

namespace QscQsys.Communications.Sockets
{
    /// <summary>
    /// Represents a TCP client for socket communication.
    /// </summary>
    public sealed class TcpClient : AbstractSocketCommunicator
    {
        private TCPClient _client;
        private readonly CTimer _retryTimer;
        private bool _disconnectRequested;

        /// <summary>
        /// Initializes a new instance of the TcpClient class.
        /// </summary>
        public TcpClient()
        {
            _retryTimer = new CTimer(x =>
                Reconnect(), Timeout.Infinite);
        }

        /// <summary>
        /// Initializes a new instance of the TcpClient class with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the client.</param>
        public TcpClient(string id)
            : base(id)
        {
            _retryTimer = new CTimer(x =>
                Reconnect(), Timeout.Infinite);
        }

        /// <summary>
        /// Initializes a new instance of the TcpClient class with the specified logger.
        /// </summary>
        /// <param name="logger">The logger to use for logging.</param>
        public TcpClient(ILogger logger)
            : base(logger)
        {
            _retryTimer = new CTimer(x =>
                Reconnect(), Timeout.Infinite);
        }

        /// <summary>
        /// Initializes a new instance of the TcpClient class with the specified ID and logger.
        /// </summary>
        /// <param name="id">The ID of the client.</param>
        /// <param name="logger">The logger to use for logging.</param>
        public TcpClient(string id, ILogger logger)
            : base(id, logger)
        {
            _retryTimer = new CTimer(x =>
                Reconnect(), Timeout.Infinite);
        }

        private void ConnectToServerCallback(TCPClient client)
        {
            if (Disposed)
                return;

            if (client.ClientStatus == SocketStatus.SOCKET_STATUS_CONNECTED) return;
            WaitAndTryReconnect();
        }

        private void ReceiveCallback(TCPClient client, int numBytes)
        {
            lock (MainLock)
            {
                if (ProtectedDisposed)
                    return;

                if (client == null)
                    return;

                if (numBytes > 0)
                {
                    var response = new string(client.IncomingDataBuffer.Take(numBytes).Select(b => (char)b).ToArray());
                    OnResponseReceived(new StringEventArgs(response));
                }
            }

            client.ReceiveDataAsync(ReceiveCallback);
        }

        private void Client_SocketStatusChange(TCPClient client, SocketStatus clientSocketStatus)
        {
            lock (MainLock)
            {
                if (ProtectedDisposed)
                    return;

                if (clientSocketStatus != SocketStatus.SOCKET_STATUS_CONNECTED)
                {
                    ProtectedConnected = false;
                    WaitAndTryReconnect();
                }
                else
                {
                    _client.ReceiveDataAsync(ReceiveCallback);
                    ProtectedConnected = true;
                }

                var status = GetSocketStatusName(clientSocketStatus);
                Logger.LogNotice("SocketStatus changed {0}", status);

                OnConnectedChange(new BoolEventArgs(ProtectedConnected));
                OnConnectionStatusChange(
                    new ConnectionStatusChangeEventArgs(new ConnectionStatusChangePayload((ushort)clientSocketStatus,
                        status)));
            }
        }

        /// <summary>
        /// Connects to the TCP server.
        /// </summary>
        public override void Connect()
        {
            var ipaddress = IpAddress;
            var port = Port;
            Connect(ipaddress, port);
        }

        /// <summary>
        /// Connects to the TCP server at the specified IP address and port.
        /// </summary>
        /// <param name="ipAddress">The IP address of the TCP server.</param>
        /// <param name="port">The port number of the TCP server.</param>
        public override void Connect(string ipAddress, ushort port)
        {
            ThrowIfDisposed();

            lock (MainLock)
            {
                ProtectedIpAddress = ipAddress;
                ProtectedPort = port;

                if (string.IsNullOrEmpty(ipAddress) && port <= 0)
                {
                    Logger.LogError("Host is null or emptry and/or port is set to 0");
                    return;
                }
                if (ProtectedConnected)
                {
                    Disconnect();
                }

                Logger.PrintLine("Connection starting {0}:{1}...", ipAddress, port);
                Logger.LogNotice("Connection starting {0}:{1}...", ipAddress, port);
                _retryTimer.Stop();

                if (_client != null)
                    _client.SocketStatusChange -= Client_SocketStatusChange;

                _client = new TCPClient(ipAddress, port, 65535);
                _client.SocketStatusChange += Client_SocketStatusChange;
                _disconnectRequested = false;
                var error = _client.ConnectToServerAsync(ConnectToServerCallback);
                ConnectionRequested = true;
                if (error != SocketErrorCodes.SOCKET_OK)
                {
                    Logger.PrintLine("ConnectToServerAsync error: {0}", GetSocketErrorCodeName(error));
                    WaitAndTryReconnect();
                }
            }
        }

        private void Reconnect()
        {
            lock (MainLock)
            {
                if (ProtectedDisposed)
                    return;

                if (_client == null)
                    return;

                if (ProtectedConnected || _disconnectRequested)
                    return;

                Logger.PrintLine("Connection was not established, retrying...");
                var error = _client.ConnectToServerAsync(ConnectToServerCallback);

                if (error != SocketErrorCodes.SOCKET_OK)
                {
                    Logger.PrintLine("ConnectToServerAsync error: {0}", GetSocketErrorCodeName(error));
                    WaitAndTryReconnect();
                }
            }
        }

        private void WaitAndTryReconnect()
        {
            CrestronInvoke.BeginInvoke(x =>
            {
                lock (MainLock)
                {
                    if (ProtectedDisposed)
                        return;

                    try
                    {
                        if (ProtectedConnected || _disconnectRequested || _client == null) return;
                        _client.DisconnectFromServer();

                        if (_retryTimer == null) return;
                        _retryTimer.Reset(2500);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }

                }
            });
        }

        /// <summary>
        /// Disconnects from the TCP server.
        /// </summary>
        public override void Disconnect()
        {
            ThrowIfDisposed();

            lock (MainLock)
            {
                if (ProtectedDisposed)
                    return;

                ConnectionRequested = false;

                if (_client == null)
                    return;

                if (!ProtectedConnected)
                    return;

                Logger.LogNotice("Connection closing...");
                Logger.PrintLine("Connection closing...");
                _disconnectRequested = true;

                if (_retryTimer != null)
                {
                    _retryTimer.Stop();
                }
                _client.DisconnectFromServer();
            }
        }

        /// <summary>
        /// Sends a command to the TCP server.
        /// </summary>
        /// <param name="command">The command to send.</param>
        public override void SendCommand(string command)
        {
            var data = command.ToCharArray().Select(c => (byte)c).ToArray();
            SendCommand(data);
        }

        /// <summary>
        /// Sends a command to the TCP server.
        /// </summary>
        /// <param name="command">The command to send as a character array.</param>
        public override void SendCommand(char[] command)
        {
            if (command == null)
                return;

            var data = new byte[command.Length];

            for (var i = 0; i < command.Length; i++)
            {
                data[i] = Convert.ToByte(command[i]);
            }

            SendCommand(data);
        }

        /// <summary>
        /// Sends a command to the TCP server.
        /// </summary>
        /// <param name="command">The command to send as a byte array.</param>
        public override void SendCommand(byte[] command)
        {
            ThrowIfDisposed();

            if (command == null)
                return;

            try
            {
                if (!IsConnected) return;
                var c = new string(command.Take(command.Length).Select(b => (char)b).ToArray());
                Logger.PrintLine("Sending command -->{0}<--", c.ReplaceHex());
                _client.SendData(command, command.Length);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public override void Dispose()
        {
            ThrowIfDisposed();

            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (ProtectedDisposed)
                return;

            lock (MainLock)
            {
                if (ProtectedDisposed)
                    return;

                ProtectedDisposed = true;

                if (disposing)
                {
                    _retryTimer.Stop();
                    _retryTimer.Dispose();

                    if (_client != null)
                    {
                        _client.SocketStatusChange -= Client_SocketStatusChange;
                        _client.Dispose();
                    }

                    ProtectedConnected = false;
                }
            }
        }
    }
}