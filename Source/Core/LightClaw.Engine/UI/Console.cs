using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public Console() : this(true) { }

        public Console(bool registerDefaultCommands)
        {
            if (registerDefaultCommands)
            {
                foreach (MethodInfo defaultCommand in typeof(DefaultCommands).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                                             .FilterNull()
                                                                             .Where(mInfo => 
                                                                             {
                                                                                 ParameterInfo[] pInfo = mInfo.GetParameters();
                                                                                 return (pInfo.Length == 1) && (pInfo[0].ParameterType == typeof(string[]));
                                                                             }))
                {
                    this.Register((Action<string[]>)defaultCommand.CreateDelegate(typeof(Action<string[]>)));
                }
            }
        }

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

        private static string[] ParseArguments(string commandLine)
        {
            Contract.Requires<ArgumentNullException>(commandLine != null);

            char[] paramChars = commandLine.ToCharArray();
            bool insideQuote = false;
            for (int index = 0; index < paramChars.Length; index++)
            {
                if (paramChars[index] == '"')
                {
                    insideQuote = !insideQuote;
                }
                if (!insideQuote && paramChars[index] == ' ')
                {
                    paramChars[index] = '\n';
                }
            }
            return (new string(paramChars)).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
