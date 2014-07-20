using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LightClaw.TeamServer.Server
{
    /// <summary>
    /// Represents an <see cref="ActionGuidServer"/> requiring public key authentification before any actions will be taken.
    /// </summary>
    public class AuthedActionGuidServer : ActionGuidServer
    {
        /// <summary>
        /// Initializes a new <see cref="AuthedActionGuidServer"/>.
        /// </summary>
        public AuthedActionGuidServer() { }

        /// <summary>
        /// Initializes a new <see cref="AuthedActionGuidServer"/> specifying the <see cref="IPAddress"/> to listen to and the port to use.
        /// </summary>
        /// <param name="adress">The <see cref="IPAddress"/> to listen to.</param>
        /// <param name="port">The port to listen to.</param>
        public AuthedActionGuidServer(IPAddress adress, int port) : base(adress, port) { }

        /// <summary>
        /// Handles a connected <see cref="TcpClient"/>
        /// </summary>
        /// <param name="client">The <see cref="TcpClient"/> to handle.</param>
        protected override void ClientHandler(TcpClient client)
        {
            base.ClientHandler(client);
        }
    }
}
