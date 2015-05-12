using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics.OpenGL
{
    /// <summary>
    /// Represents the error that occurs when the compilation of a shader failed.
    /// </summary>
    [Serializable]
    public class ValidationFailedException : OpenGLException
    {
        /// <summary>
        /// Initializes a new <see cref="ValidationFailedException"/>.
        /// </summary>
        public ValidationFailedException() { }

        /// <summary>
        /// Initializes a new <see cref="ValidationFailedException"/> and sets the message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ValidationFailedException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new <see cref="ValidationFailedException"/> and sets the message and the OpenGL info log.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="infoLog">The OpenGL info log.</param>
        public ValidationFailedException(string message, string infoLog) : base(message, infoLog) { }

        /// <summary>
        /// Initializes a new <see cref="ValidationFailedException"/> and sets the message, the error code and the
        /// OpenGL info log.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="infoLog">The OpenGL info log.</param>
        /// <param name="errorCode">The result code.</param>
        public ValidationFailedException(string message, string infoLog, int errorCode) : base(message, infoLog, errorCode) { }

        /// <summary>
        /// Initializes a new <see cref="ValidationFailedException"/> and sets the message and the error code.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="errorCode">The result code.</param>
        public ValidationFailedException(string message, int errorCode) : base(message, errorCode) { }

        /// <summary>
        /// Initializes a new <see cref="ValidationFailedException"/> and sets the message and inner exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The <see cref="Exception"/> that lead to this exception being thrown.</param>
        public ValidationFailedException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new <see cref="ValidationFailedException"/>
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/>.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected ValidationFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
