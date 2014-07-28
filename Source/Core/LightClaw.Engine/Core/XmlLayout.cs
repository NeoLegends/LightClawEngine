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

namespace LightClaw.Engine.Core
{
    public class XmlLayout : XmlLayoutBase
    {
        private readonly DataContractSerializer serializer = new DataContractSerializer(typeof(LoggingEventInfo));

        protected override void FormatXml(XmlWriter writer, LoggingEvent loggingEvent)
        {
            if (loggingEvent != null)
            {
                using (StringWriter sw = new StringWriter())
                using (XmlTextWriter xmlTw = new XmlTextWriter(sw))
                {
                    this.serializer.WriteObject(xmlTw, new XmlLayout.LoggingEventInfo(loggingEvent));

                    writer.WriteRaw(sw.ToString());
                }
            }
        }
        
        [DataContract]
        public class LoggingEventInfo
        {
            [DataMember]
            public ExceptionInfo Exception { get; set; }

            [DataMember]
            public string Level { get; set; }

            [DataMember]
            public Location Location { get; set; }

            [DataMember]
            public string Logger { get; set; }

            [DataMember]
            public string Message { get; set; }

            [DataMember]
            public string Thread { get; set; }

            [DataMember]
            public string Timestamp { get; set; }

            public LoggingEventInfo() { }

            public LoggingEventInfo(LoggingEvent loggingEvent)
            {
                Contract.Requires<ArgumentNullException>(loggingEvent != null);

                if (loggingEvent.ExceptionObject != null)
                {
                    this.Exception = new ExceptionInfo(loggingEvent.ExceptionObject);
                }
                this.Level = loggingEvent.Level.DisplayName;
                if (loggingEvent.LocationInformation != null)
                {
                    this.Location = new Location(loggingEvent);
                }
                this.Logger = loggingEvent.LoggerName;
                this.Message = loggingEvent.RenderedMessage;
                this.Thread = loggingEvent.ThreadName;
                this.Timestamp = loggingEvent.TimeStamp.ToUniversalTime().ToString();
            }
        }

        [DataContract]
        public struct Location
        {
            [DataMember]
            public string ClassName { get; set; }

            [DataMember]
            public string FileName { get; set; }

            [DataMember]
            public int LineNumber { get; set; }

            [DataMember]
            public string MethodName { get; set; }

            public Location(LoggingEvent loggingEvent)
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

        [DataContract]
        public class ExceptionInfo
        {
            [DataMember]
            public string Message { get; set; }

            [DataMember]
            public ExceptionInfo InnerException { get; set; }

            [DataMember]
            public ExceptionInfo[] InnerExceptions { get; set; }

            [DataMember]
            public string Type { get; set; }

            public ExceptionInfo(System.Exception ex)
            {
                Contract.Requires<ArgumentNullException>(ex != null);

                this.Message = ex.Message;
                this.Type = ex.GetType().AssemblyQualifiedName;
                AggregateException aggrEx = ex as AggregateException;
                if (aggrEx != null)
                {
                    this.InnerExceptions = aggrEx.InnerExceptions.Select(innerEx => new ExceptionInfo(innerEx)).ToArray();
                }
                if (aggrEx == null && ex.InnerException != null)
                {
                    this.InnerException = new ExceptionInfo(ex.InnerException);
                }
            }
        }
    }
}
