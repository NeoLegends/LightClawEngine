using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Logging
{
    /// <summary>
    /// Contains information about an exception that lead to a log entry being created.
    /// </summary>
    [DataContract]
    public sealed class ExceptionInfo
    {
        /// <summary>
        /// The exceptions message.
        /// </summary>
        [DataMember]
        public string Message { get; private set; }

        /// <summary>
        /// The <see cref="Exception"/> 's inner exception.
        /// </summary>
        [DataMember]
        public ExceptionInfo InnerException { get; private set; }

        /// <summary>
        /// If the exception was an <see cref="AggregateException"/> , this contains its inner exceptions.
        /// </summary>
        [DataMember]
        public ExceptionInfo[] InnerExceptions { get; private set; }

        /// <summary>
        /// The exceptions stack trace.
        /// </summary>
        [DataMember]
        public string StackTrace { get; private set; }

        /// <summary>
        /// The <see cref="Type"/> of the <see cref="Exception"/> .
        /// </summary>
        [DataMember]
        public string Type { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="ExceptionInfo"/> from the specified <paramref name="exception"/> .
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to initialize from.</param>
        public ExceptionInfo(Exception exception)
        {
            Contract.Requires<ArgumentNullException>(exception != null);

            this.Message = exception.Message;
            this.StackTrace = exception.StackTrace;
            this.Type = exception.GetType().AssemblyQualifiedName;

            AggregateException aggrEx = exception as AggregateException;
            if (aggrEx != null && aggrEx.InnerExceptions != null)
            {
                this.InnerExceptions = aggrEx.InnerExceptions.Select(innerEx => new ExceptionInfo(innerEx)).ToArray();
            }
            if (aggrEx == null && exception.InnerException != null)
            {
                this.InnerException = new ExceptionInfo(exception.InnerException);
            }
        }
    }
}
