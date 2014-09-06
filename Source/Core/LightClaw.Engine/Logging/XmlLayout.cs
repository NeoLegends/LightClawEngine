using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
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
        private readonly DataContractSerializer serializer = new DataContractSerializer(typeof(LoggingEventInfo));

        /// <summary>
        /// Initializes a new <see cref="XmlLayout"/>.
        /// </summary>
        public XmlLayout() { }

        /// <summary>
        /// Converts the specified <see cref="LoggingEvent"/> into XML and writes it into the <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to output the XML into.</param>
        /// <param name="loggingEvent">The <see cref="LoggingEvent"/> containing the data about the log entry.</param>
        protected override void FormatXml(XmlWriter writer, LoggingEvent loggingEvent)
        {
            if (loggingEvent != null && writer != null && loggingEvent.Level != null)
            {
                using (StringWriter sw = new StringWriter(new StringBuilder(2048)))
                using (XmlTextWriter xmlTw = new XmlTextWriter(sw))
                {
                    this.serializer.WriteObject(xmlTw, new LoggingEventInfo(loggingEvent));

                    writer.WriteRaw(sw.ToString());
                }
            }
        }
    }
}
