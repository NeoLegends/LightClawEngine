using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LightClaw.Engine.Core;
using ProtoBuf;

namespace LightClaw.Editor.Core
{
    /// <summary>
    /// LightClaw's scene and game editor.
    /// </summary>
    [Serializable, DataContract(IsReference = true), ProtoContract]
    public class LightClawEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightClawEditor"/> class.
        /// </summary>
        public LightClawEditor() { }

        public static void Main(String[] args)
        {
            throw new NotImplementedException();
        }
    }
}
