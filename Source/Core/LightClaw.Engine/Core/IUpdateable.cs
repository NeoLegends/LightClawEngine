using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Defines a mechanism to update an instance.
    /// </summary>
    [ContractClass(typeof(IUpdateableContracts))]
    public interface IUpdateable
    {
        /// <summary>
        /// Notifies about the start of the updating process.
        /// </summary>
        /// <remarks>Raised before any updating operations.</remarks>
        event EventHandler<UpdateEventArgs> Updating;

        /// <summary>
        /// Notifies about the finsih of the updating process.
        /// </summary>
        /// <remarks>Raised after any updating operations.</remarks>
        event EventHandler<UpdateEventArgs> Updated;

        /// <summary>
        /// Updates the instance with the specified <see cref="GameTime"/>.
        /// </summary>
        /// <param name="gameTime">The current <see cref="GameTime"/>.</param>
        /// <param name="pass"></param>
        bool Update(GameTime gameTime, int pass);
    }

    [ContractClassFor(typeof(IUpdateable))]
    abstract class IUpdateableContracts : IUpdateable
    {
        public event EventHandler<UpdateEventArgs> Updating;

        public event EventHandler<UpdateEventArgs> Updated;

        public bool Update(GameTime gameTime, int pass)
        {
            Contract.Requires<ArgumentOutOfRangeException>(pass >= 0);

            return false;
        }
    }
}
