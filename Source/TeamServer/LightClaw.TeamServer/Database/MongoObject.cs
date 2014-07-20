using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;

namespace LightClaw.TeamServer.Database
{
    /// <summary>
    /// Represents the base class for all data stored in the MongoDB.
    /// </summary>
    public abstract class MongoObject
    {
        /// <summary>
        /// The object's Id.
        /// </summary>
        public ObjectId Id { get; set; }
    }
}
