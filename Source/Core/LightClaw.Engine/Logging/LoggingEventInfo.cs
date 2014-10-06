using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;

namespace LightClaw.Engine.Logging
{
    /// <summary>
    /// Contains information about a log entry.
    /// </summary>
    [DataContract]
    public sealed class LoggingEventInfo
    {
        /// <summary>
        /// An exception that lead to the log entry being written.
        /// </summary>
        [DataMember]
        public ExceptionInfo Exception { get; private set; }

        /// <summary>
        /// The log entry's level.
        /// </summary>
        [DataMember]
        public string Level { get; private set; }

        /// <summary>
        /// Information about the place where the log entry was written.
        /// </summary>
        [DataMember]
        public LocationInfo Location { get; private set; }

        /// <summary>
        /// The logger that created the entry.
        /// </summary>
        [DataMember]
        public string Logger { get; private set; }

        /// <summary>
        /// The log message.
        /// </summary>
        [DataMember]
        public string Message { get; private set; }

        /// <summary>
        /// The thread in which the log entry was created.
        /// </summary>
        [DataMember]
        public string Thread { get; private set; }

        /// <summary>
        /// The log's timestamp.
        /// </summary>
        [DataMember]
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="LoggingEventInfo"/> .
        /// </summary>
        /// <param name="loggingEvent">
        /// The <see cref="LoggingEvent"/> to create a new <see cref="LoggingEventInfo"/> from.
        /// </param>
        public LoggingEventInfo(LoggingEvent loggingEvent)
        {
            Contract.Requires<ArgumentNullException>(loggingEvent != null);
            Contract.Requires<ArgumentException>(loggingEvent.Level != null);

            if (loggingEvent.ExceptionObject != null)
            {
                this.Exception = new ExceptionInfo(loggingEvent.ExceptionObject);
            }
            this.Level = loggingEvent.Level.DisplayName;
            if (loggingEvent.LocationInformation != null)
            {
                this.Location = new LocationInfo(loggingEvent);
            }
            this.Logger = loggingEvent.LoggerName;
            this.Message = loggingEvent.RenderedMessage;
            this.Thread = loggingEvent.ThreadName;
            this.Timestamp = loggingEvent.TimeStamp.ToUniversalTime();
        }
    }
}
