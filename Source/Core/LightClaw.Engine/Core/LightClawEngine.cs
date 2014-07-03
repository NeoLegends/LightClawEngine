using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics;
using Munq;
using ProtoBuf;

namespace LightClaw.Engine.Core
{
    public class LightClawEngine
    {
        private static readonly IocContainer _DefaultIoc = new IocContainer();

        [CLSCompliant(false)]
        public static IocContainer DefaultIoc
        {
            get
            {
                return _DefaultIoc;
            }
        }

        public static void Main(string[] args)
        {
        }
    }
}
