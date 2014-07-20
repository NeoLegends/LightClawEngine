using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using LightClaw.TeamServer.Database;
using LightClaw.TeamServer.Server;

namespace LightClaw.TeamServer.LiveEdit
{
    /// <summary>
    /// Responsible for synchronizing assets between multiple clients.
    /// </summary>
    public class AssetSynchronization
    {
        /// <summary>
        /// A <see cref="ActionGuidServer"/> used to receive incoming TCP requests.
        /// </summary>
        public AuthedActionGuidServer Server { get; private set; }

        /// <summary>
        /// A <see cref="TeamDatabase"/> storing the assets.
        /// </summary>
        public TeamDatabase Database { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="AssetSynchronization"/>.
        /// </summary>
        /// <param name="server">A <see cref="ActionGuidServer"/> used to receive incoming TCP requests.</param>
        /// <param name="db">A <see cref="TeamDatabase"/> storing the assets.</param>
        public AssetSynchronization(AuthedActionGuidServer server, TeamDatabase db)
        {
            this.Server = server;
            this.Database = db;
        }

        private void ReceiveFile(TcpClient client)
        {
            
        }

        private void PushFile(TcpClient client)
        {

        }
    }
}
