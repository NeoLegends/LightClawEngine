using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using log4net.Core;
using log4net.Layout;

namespace LightClaw.Engine.Logging
{
    /// <summary>
    /// Represents a log4net layout outputting the log data as XML.
    /// </summary>
    public class XmlLayout : XmlLayoutBase
    {
        /// <summary>
        /// The underlying <see cref="DataContractSerializer"/> serializing the data.
        /// </summary>
        private static readonly DataContractSerializer serializer = new DataContractSerializer(typeof(LoggingEventInfo));

        /// <summary>
        /// Backing field.
        /// </summary>
        private int _Capacity;

        /// <summary>
        /// The initial capacity the underlying <see cref="StringBuilder"/> will be instantiated with.
        /// </summary>
        /// <remarks>
        /// The performance of the <see cref="XmlLayout"/> depends on this value. Set this to a higher value if it is
        /// common to log larger messages.
        /// </remarks>
        public int Capacity
        {
            get
            {
                return _Capacity;
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                _Capacity = value;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="XmlLayout"/> .
        /// </summary>
        public XmlLayout()
            : this(1024)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="XmlLayout"/> and sets the capacity of the underlying
        /// <see cref="StringBuilder"/> .
        /// </summary>
        /// <param name="capacity">The capacity of the underlying <see cref="StringBuilder"/> .</param>
        public XmlLayout(int capacity)
        {
            Contract.Requires<ArgumentOutOfRangeException>(capacity >= 0);

            this.Capacity = capacity;
            this.Header = "<LoggingEvents>" + Environment.NewLine;
            this.Footer = "</LoggingEvents>" + Environment.NewLine;
        }

        /// <summary>
        /// Converts the specified <see cref="LoggingEvent"/> into XML and writes it into the <paramref name="writer"/> .
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to output the XML into.</param>
        /// <param name="loggingEvent">The <see cref="LoggingEvent"/> containing the data about the log entry.</param>
        protected override void FormatXml(XmlWriter writer, LoggingEvent loggingEvent)
        {
            if (loggingEvent != null && writer != null && loggingEvent.Level != null)
            {
                using (StringWriter sw = new StringWriter(new StringBuilder(this.Capacity)))
                using (XmlTextWriter xmlTw = new XmlTextWriter(sw))
                {
                    serializer.WriteObject(xmlTw, new LoggingEventInfo(loggingEvent));

                    writer.WriteRaw(sw.ToString());
                }
            }
        }
    }
}
