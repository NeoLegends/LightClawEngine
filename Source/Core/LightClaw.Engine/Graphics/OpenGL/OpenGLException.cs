using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents an OpenGL exception.
    /// </summary>
    [Serializable]
    public abstract class OpenGLException : InvalidOperationException
    {
        /// <summary>
        /// The info log.
        /// </summary>
        public string InfoLog
        {
            get
            {
                Contract.Assume(this.Data != null);
                return (string)this.Data["InfoLog"];
            }
            protected set
            {
                Contract.Assume(this.Data != null);
                this.Data["InfoLog"] = value;
            }
        }

        /// <summary>
        /// The result code.
        /// </summary>
        public int ResultCode
        {
            get
            {
                Contract.Assume(this.Data != null);
                object value = this.Data["ErrorCode"];
                return (value != null) ? (int)value : 0;
            }
            protected set
            {
                Contract.Assume(this.Data != null);
                this.Data["ErrorCode"] = value;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="OpenGLException"/>.
        /// </summary>
        protected OpenGLException()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="OpenGLException"/> and sets the message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        protected OpenGLException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="OpenGLException"/> and sets the message and the OpenGL info log.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="infoLog">The OpenGL info log.</param>
        protected OpenGLException(string message, string infoLog)
            : base(message)
        {
            this.InfoLog = infoLog;
        }

        /// <summary>
        /// Initializes a new <see cref="OpenGLException"/> and sets the message, the error code and the OpenGL info log.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="infoLog">The OpenGL info log.</param>
        /// <param name="errorCode">The result code.</param>
        protected OpenGLException(string message, string infoLog, int resultCode)
            : this(message, infoLog)
        {
            this.ResultCode = resultCode;
        }

        /// <summary>
        /// Initializes a new <see cref="OpenGLException"/> and sets the message and the error code.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="errorCode">The result code.</param>
        protected OpenGLException(string message, int resultCode)
            : base(message)
        {
            this.ResultCode = ResultCode;
        }

        /// <summary>
        /// Initializes a new <see cref="OpenGLException"/>
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The <see cref="Exception"/> that lead to this exception being thrown.</param>
        protected OpenGLException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="CompilationFailedException"/> after deserialization.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/>.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected OpenGLException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
