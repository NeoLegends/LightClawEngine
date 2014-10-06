using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.UI
{
    public class ConsoleWriter : TextWriter
    {
        public Console Console { get; private set; }

        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }

        public ConsoleWriter(Console console)
        {
            Contract.Requires<ArgumentNullException>(console != null);

            this.Console = console;
        }

        public override void Write(char value)
        {
            throw new NotImplementedException();
        }

        public override Task WriteAsync(char value)
        {
            throw new NotImplementedException();
        }
    }
}
