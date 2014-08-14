using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Graphics
{
    public class DataTooLargeException : ArgumentOutOfRangeException
    {
        public string ConditionString { get; private set; }

        public DataTooLargeException() { }

        public DataTooLargeException(string message) : base(message) { }

        public DataTooLargeException(string message, string condition)
            : this(message) 
        {
            this.ConditionString = condition;
        }

        public DataTooLargeException(string message, Exception inner) : base(message, inner) { }

        public DataTooLargeException(string message, Exception inner, string condition)
            : base(message, inner)
        {
            this.ConditionString = condition;
        }
    }
}
