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
        private static readonly IocContainer _DefaultIocContainer = new IocContainer();

        [CLSCompliant(false)]
        public static IocContainer DefaultIocContainer
        {
            get
            {
                return _DefaultIocContainer;
            }
        }

        public static void Main(string[] args)
        {
        }
    }
}
