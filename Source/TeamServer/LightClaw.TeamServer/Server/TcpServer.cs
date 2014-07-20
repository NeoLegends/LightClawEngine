using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LightClaw.Common.Logging;

namespace LightClaw.TeamServer.Server
{
    /// <summary>
    /// Represents a threaded TCP-server.
    /// </summary>
    public abstract class TcpServer : Entity, IDisposable
    {
        /// <summary>
        /// Used for locking.
        /// </summary>
        private object stateLock = new object();

        /// <summary>
        /// Indicates whether the listening loop is running.
        /// </summary>
        private CancellationTokenSource ctSource;

        /// <summary>
        /// A <see cref="TcpListener"/> instance listening for TCP requests.
        /// </summary>
        private TcpListener listener;

        /// <summary>
        /// Indicates whether the instance has already been disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Initializing a new <see cref="TcpServer"/> using listening to any IP adress and using the default (44444) port.
        /// </summary>
        public TcpServer() : this(IPAddress.Any, 44444) { }

        /// <summary>
        /// Initializes a new <see cref="TcpServer"/> listening to the specified IP adress only using the given port.
        /// </summary>
        /// <param name="address">The <see cref="IPAddress"/> to listen to.</param>
        /// <param name="port">The port to listen to.</param>
        public TcpServer(IPAddress address, int port)
        {
            Contract.Requires<ArgumentNullException>(address != null);

            this.listener = new TcpListener(new IPEndPoint(address, port));
        }

        /// <summary>
        /// Finalizes the object releasing all allocated resource before the object is reclaimed by garbage collection.
        /// </summary>
        ~TcpServer()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Starts the <see cref="TcpServer"/>.
        /// </summary>
        public void Start()
        {
            Log.Write("Server starting, entering async TCP receiver loop...", MessageLevel.ImportantInformation);

            lock (stateLock)
            {
                this.ctSource = new CancellationTokenSource();
                listener.Start();
                listener.BeginAcceptTcpClient(this.OnAcceptConnection, listener);
            }

            Log.Write("Server started successfully.", MessageLevel.ImportantInformation);
        }

        /// <summary>
        /// Stops the <see cref="TcpServer"/>.
        /// </summary>
        public void Stop()
        {
            Log.Write("Server shutting down...", MessageLevel.ImportantInformation);

            lock (stateLock)
            {
                this.ctSource.Cancel();
                this.listener.Stop();
            }

            Log.Write("Cancellation token submitted, waiting for async listener loop to stop.", MessageLevel.ImportantInformation);
        }

        /// <summary>
        /// Disposes the instance releasing all object-allocated resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Disposes the instance releasing all unmanaged resources and optionally managed resources as well.
        /// </summary>
        /// <param name="disposing">A boolean indicating whether to release managed resources as well.</param>
        protected virtual void Dispose(bool disposing)
        {
            lock (stateLock)
            {
                if (!this.IsDisposed)
                {
                    this.Stop();
                    if (this.ctSource != null)
                        this.ctSource.Dispose();

                    this.IsDisposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Handles the interaction with the client.
        /// </summary>
        /// <param name="tcpClient">A <see cref="TcpClient"/>-instance.</param>
        protected abstract void ClientHandler(TcpClient tcpClient);

        /// <summary>
        /// Accepts a connection.
        /// </summary>
        /// <param name="asyncResult"><see cref="IAsyncResult"/>.</param>
        private void OnAcceptConnection(IAsyncResult asyncResult)
        {
            if (!this.ctSource.IsCancellationRequested)
            {
                TcpListener listener = ((TcpListener)asyncResult.AsyncState);

                Task.Factory.StartNew(() =>
                {
                    using (TcpClient client = listener.EndAcceptTcpClient(asyncResult))
                    {
                        IPAddress clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address;

                        Log.Write("Client '{0}' connected, executing handler.", clientIP);
                        this.ClientHandler(client);
                        Log.Write("Handler for client '{0}' executed, disconnecting.", clientIP);
                    }
                });

                listener.BeginAcceptTcpClient(OnAcceptConnection, listener);
            }
        }
    }
}
