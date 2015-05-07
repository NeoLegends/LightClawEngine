using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;

namespace LightClaw.Engine.UI
{
    [DataContract]
    public class Console : Component
    {
        public event EventHandler<ValueChangedEventArgs<bool>> IsShownChanged;

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

        [DataMember]
        public bool IsShown
        {
            get
            {
                return _IsShown;
            }
            set
            {
                bool previousValue = _IsShown;
                this.SetProperty(ref _IsShown, value);
                this.Raise(this.IsShownChanged, value, previousValue);
            }
        }

        private ConsoleWriter _Writer;

        public ConsoleWriter Writer
        {
            get
            {
                return _Writer;
            }
            private set
            {
                this.SetProperty(ref _Writer, value);
            }
        }

        public Console()
            : this(true)
        {
        }

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
                    this.Register(defaultCommand.CreateDelegate<Action<string[]>>());
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

        protected override bool OnUpdate(GameTime gameTime, int pass)
        {
            try
            {
                if (this.IsShown)
                {
                }
                return true;
            }
            finally
            {
                base.OnUpdate(gameTime, pass);
            }
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
            for (int i = 0; i < paramChars.Length; i++)
            {
                if (paramChars[i] == '"')
                {
                    insideQuote = !insideQuote;
                }
                if (!insideQuote && paramChars[i] == ' ')
                {
                    paramChars[i] = '\n';
                }
            }
            return new string(paramChars).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
