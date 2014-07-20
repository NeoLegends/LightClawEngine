using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace LightClaw.TeamServer.Database
{
    /// <summary>
    /// Connects the TeamServer to the underlying MongoDB database.
    /// </summary>
    public class TeamDatabase
    {
        /// <summary>
        /// Used for locking.
        /// </summary>
        private object lockObject = new object();

        /// <summary>
        /// A <see cref="MongoClient"/> to a MongoDB.
        /// </summary>
        public MongoClient Client { get; private set; }

        /// <summary>
        /// Backing field.
        /// </summary>
        private MongoServer _Server = null;

        /// <summary>
        /// The server the <see cref="MongoClient"/> is connected to.
        /// </summary>
        public MongoServer Server
        {
            get
            {
                lock (lockObject)
                {
                    return _Server ?? this.Client.GetServer();
                }
            }
        }

        /// <summary>
        /// Initializes a new <see cref="TeamDatabase"/> and connects to the host specified in the given <see cref="MongoClientSettings"/>.
        /// </summary>
        /// <param name="settings">The <see cref="MongoClientSettings"/> used to connect to the server.</param>
        public TeamDatabase(MongoClientSettings settings)
        {
            this.Connect(settings);
        }

        /// <summary>
        /// Initializes a new <see cref="TeamDatabase"/> and connects to the server with the given Url.
        /// </summary>
        /// <param name="serverUrl">The Url of the server to connect to.</param>
        public TeamDatabase(String serverUrl)
        {
            this.Connect(serverUrl);
        }

        /// <summary>
        /// Connects the <see cref="TeamDatabase"/> to a server using the specified <see cref="MongoClientSettings"/>.
        /// </summary>
        /// <param name="settings">The <see cref="MongoClientSettings"/> used to connect to the server.</param>
        public void Connect(MongoClientSettings settings)
        {
            lock (lockObject)
            {
                this._Server = null;
                this.Client = new MongoClient(settings);
            }
        }

        /// <summary>
        /// Connects the <see cref="TeamDatabase"/> to a server with the given Url.
        /// </summary>
        /// <param name="serverUrl">The Url of the <see cref="MongoServer"/> to connect to.</param>
        public void Connect(String serverUrl)
        {
            lock (lockObject)
            {
                this._Server = null;
                this.Client = new MongoClient(serverUrl);
            }
        }
    }
}
