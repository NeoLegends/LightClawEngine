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
    /// Contains information about the place where the log entry was created.
    /// </summary>
    [DataContract]
    public struct LocationInfo
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        [DataMember]
        public string ClassName { get; private set; }

        /// <summary>
        /// The name of the file.
        /// </summary>
        [DataMember]
        public string FileName { get; private set; }

        /// <summary>
        /// The line number inside the file.
        /// </summary>
        [DataMember]
        public int LineNumber { get; private set; }

        /// <summary>
        /// The name of the method currently executing.
        /// </summary>
        [DataMember]
        public string MethodName { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="LocationInfo"/> from the specified <see cref="LoggingEvent"/> .
        /// </summary>
        /// <param name="loggingEvent">The <see cref="LoggingEvent"/> to create a <see cref="LocationInfo"/> from.</param>
        public LocationInfo(LoggingEvent loggingEvent)
            : this()
        {
            Contract.Requires<ArgumentNullException>(loggingEvent != null);
            Contract.Requires<ArgumentException>(loggingEvent.LocationInformation != null);

            this.ClassName = loggingEvent.LocationInformation.ClassName;
            this.FileName = loggingEvent.LocationInformation.FileName;
            this.LineNumber = int.Parse(loggingEvent.LocationInformation.LineNumber);
            this.MethodName = loggingEvent.LocationInformation.MethodName;
        }
    }
}
