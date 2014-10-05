using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    /// <summary>
    /// Represents the error that occurs when the compilation of a shader failed.
    /// </summary>
    [Serializable]
    public class CompilationFailedException : InvalidOperationException
    {
        /// <summary>
        /// The result code.
        /// </summary>
        public int ErrorCode
        {
            get
            {
                object value = this.Data["ErrorCode"];
                return (value != null) ? (int)value : 0;
            }
            private set
            {
                this.Data["ErrorCode"] = value;
            }
        }

        /// <summary>
        /// The shader info log.
        /// </summary>
        public string InfoLog
        {
            get
            {
                return (string)this.Data["InfoLog"];
            }
            private set
            {
                this.Data["InfoLog"] = value;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="CompilationFailedException"/>.
        /// </summary>
        public CompilationFailedException() { }

        /// <summary>
        /// Initializes a new <see cref="CompilationFailedException"/> and sets the message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public CompilationFailedException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new <see cref="CompilationFailedException"/> and sets the message and the OpenGL info log.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="infoLog">The OpenGL info log.</param>
        public CompilationFailedException(string message, string infoLog)
            : this(message)
        {
            this.InfoLog = infoLog;
        }

        /// <summary>
        /// Initializes a new <see cref="CompilationFailedException"/> and sets the message, the error code and the OpenGL info log.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="infoLog">The OpenGL info log.</param>
        /// <param name="errorCode">The result code.</param>
        public CompilationFailedException(string message, string infoLog, int errorCode)
            : this(message, infoLog)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new <see cref="CompilationFailedException"/> and sets the message and the error code.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="errorCode">The result code.</param>
        public CompilationFailedException(string message, int errorCode)
            : this(message)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new <see cref="CompilationFailedException"/>
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/>.</param>
        /// <param name="info"><see cref="StreamingContext"/>.</param>
        protected CompilationFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
