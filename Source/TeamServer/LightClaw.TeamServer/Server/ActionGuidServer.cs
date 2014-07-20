using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LightClaw.TeamServer.Server
{
    /// <summary>
    /// Represents a <see cref="TcpServer"/> using <see cref="Guid"/>s to identify server-side methods to execute.
    /// </summary>
    /// <remarks>
    /// The first 16 bytes sent to the server will be the <see cref="Guid"/> pointing to a registered function. The <see cref="TcpClient"/> will then be
    /// transferred to the <see cref="Action{T}"/> as it is being executed. Any additional parameter handling will be done by the <see cref="Action{T}"/>.
    /// </remarks>
    public class ActionGuidServer : TcpServer
    {
        /// <summary>
        /// Backing field.
        /// </summary>
        private ConcurrentDictionary<Guid, Action<TcpClient>> packageActions = new ConcurrentDictionary<Guid, Action<TcpClient>>();

        /// <summary>
        /// Gets or sets the <see cref="Action{TcpClient}"/> associated with the specified key.
        /// </summary>
        /// <param name="key">The <see cref="Guid"/> identifying the <see cref="Action{TcpClient}"/> to get.</param>
        /// <returns>The associated <see cref="Action{TcpClient}"/>.</returns>
        public Action<TcpClient> this[Guid key]
        {
            get
            {
                return packageActions[key];
            }
            set
            {
                Log.Info(String.Format("Registering action with Guid '{0}'.", key.ToString()));
                packageActions[key] = value;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="ActionGuidServer"/>.
        /// </summary>
        public ActionGuidServer() { }

        /// <summary>
        /// Initializes a new <see cref="ActionGuidServer"/> specifying the <see cref="IPAddress"/> to listen to and the port to use.
        /// </summary>
        /// <param name="adress">The <see cref="IPAddress"/> to listen to.</param>
        /// <param name="port">The port to listen to.</param>
        public ActionGuidServer(IPAddress adress, int port) : base(adress, port) { }

        /// <summary>
        /// Handles the interaction with the client.
        /// </summary>
        /// <param name="client">The <see cref="TcpClient"/> to communicate with.</param>
        protected override void ClientHandler(TcpClient client)
        {
            using (NetworkStream clientStream = client.GetStream())
            {
                Guid packageId = TcpUtilities.ReadGuid(clientStream);

                Log.Info(String.Format("Package with Id '{0}' received, obtaining associated action.", packageId.ToString()));

                Action<TcpClient> action = null;
                if (this.packageActions.TryGetValue(packageId, out action))
                {
                    Log.Info("Action obtained, executing.");
                    action(client);
                }
                else
                {
                    Log.Info(String.Format("No action with Id '{0}' registered, notifying client and aborting.", packageId.ToString()));
                    TcpUtilities.WritePackage(clientStream, "No action with specified Guid registered, aborting.");
                }
            }
        }

        /// <summary>
        /// Disposes the instance releasing all unmanaged resources and optionally managed resources as well.
        /// </summary>
        /// <param name="disposing">A boolean indicating whether to release managed resources as well.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (disposing && this.packageActions != null)
                {
                    this.packageActions.Clear();
                    this.packageActions = null;
                }

                base.Dispose(disposing);
            }
        }
    }
}
