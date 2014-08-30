using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    [Serializable]
    public class CompilationFailedException : InvalidOperationException
    {
        public int ErrorCode { get; private set; }

        public string InfoLog { get; private set; }

        public CompilationFailedException() { }

        public CompilationFailedException(string message) : base(message) { }

        public CompilationFailedException(string message, string infoLog)
            : this(message)
        {
            this.InfoLog = infoLog;
        }

        public CompilationFailedException(string message, string infoLog, int errorCode)
            : this(message, infoLog)
        {
            this.ErrorCode = errorCode;
        }

        public CompilationFailedException(string message, int errorCode)
            : this(message)
        {
            this.ErrorCode = errorCode;
        }

        protected CompilationFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
