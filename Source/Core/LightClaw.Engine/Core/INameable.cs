using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Defines a mechanism to give an instance a name.
    /// </summary>
    public interface INameable
    {
        /// <summary>
        /// The instance's name.
        /// </summary>
        string Name { get; set; }
    }
}
