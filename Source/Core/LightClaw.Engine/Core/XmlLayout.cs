using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using log4net.Core;
using log4net.Layout;

namespace LightClaw.Engine.Core
{
    public class XmlLayout : XmlLayoutBase
    {
        protected override void FormatXml(XmlWriter writer, LoggingEvent loggingEvent)
        {
            writer.WriteStartElement("Entry");

            // Attributes
            writer.WriteElementString("Level", loggingEvent.Level.DisplayName);
            writer.WriteElementString("Logger", loggingEvent.LoggerName);
            writer.WriteElementString("Message", loggingEvent.RenderedMessage);
            writer.WriteElementString("Thread", loggingEvent.ThreadName);
            writer.WriteElementString("Timestamp", loggingEvent.TimeStamp.ToUniversalTime().ToString());

            // Location
            writer.WriteStartElement("Location");
            writer.WriteElementString("ClassName", loggingEvent.LocationInformation.ClassName);
            writer.WriteElementString("File", loggingEvent.LocationInformation.FileName);
            writer.WriteElementString("Line", loggingEvent.LocationInformation.LineNumber);
            writer.WriteElementString("Method", loggingEvent.LocationInformation.MethodName);
            writer.WriteEndElement();

            // Exception
            this.WriteException(loggingEvent.ExceptionObject, writer);

            writer.WriteEndElement();
        }

        private void WriteException(Exception ex, XmlWriter writer)
        {
            if (ex != null)
            {
                writer.WriteStartElement("Exception");
                writer.WriteElementString("Message", ex.Message);
                AggregateException aggrEx = ex as AggregateException;
                if (aggrEx != null)
                {
                    writer.WriteStartElement("InnerExceptions");
                    foreach (Exception inner in aggrEx.InnerExceptions)
                    {
                        this.WriteException(inner, writer);
                    }
                    writer.WriteEndElement();
                }
                if (aggrEx == null && ex.InnerException != null)
                {
                    writer.WriteStartElement("InnerException");
                    this.WriteException(ex, writer);
                    writer.WriteEndElement();
                }
                writer.WriteElementString("Type", ex.GetType().AssemblyQualifiedName);
                writer.WriteEndElement();
            }
        }
    }
}
