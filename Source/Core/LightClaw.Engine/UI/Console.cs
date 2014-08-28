using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Graphics;
using LightClaw.Extensions;

namespace LightClaw.Engine.UI
{
    [DataContract]
    public class Console : Component
    {
        private ConcurrentDictionary<string, Action<string[]>> _Commands = new ConcurrentDictionary<string, Action<string[]>>();

        [IgnoreDataMember]
        public ConcurrentDictionary<string, Action<string[]>> Commands
        {
            get
            {
                Contract.Ensures(Contract.Result<ConcurrentDictionary<string, Action<string[]>>>() != null);

                return _Commands;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Commands, value);
            }
        }

        private bool _IsShown = false;

        public bool IsShown
        {
            get
            {
                return _IsShown;
            }
            set
            {
                this.SetProperty(ref _IsShown, value);
            }
        }

        public Console() { }

        public void Register(Action<string[]> command)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            this.Register(command.Method.Name, command);
        }

        public void Register(string commandName, Action<string[]> command)
        {
            Contract.Requires<ArgumentNullException>(commandName != null);
            Contract.Requires<ArgumentNullException>(command != null);

            if (!this.TryRegister(commandName, command))
            {
                throw new InvalidOperationException("The command could not be registered as {0}. Try a different name.".FormatWith(commandName));
            }
        }

        public bool TryRegister(Action<string[]> command)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            return this.TryRegister(command.Method.Name, command);
        }

        public bool TryRegister(string commandName, Action<string[]> command)
        {
            Contract.Requires<ArgumentNullException>(commandName != null);
            Contract.Requires<ArgumentNullException>(command != null);

            return this.Commands.TryAdd(commandName, command);
        }

        protected override void OnDraw()
        {
            if (this.IsShown)
            {

            }
            base.OnDraw();
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (this.IsShown)
            {

            }
            base.OnUpdate(gameTime);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Commands != null);
        }
    }
}
