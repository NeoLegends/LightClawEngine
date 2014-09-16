using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public abstract class EffectUniform : DisposableEntity, IBindable, IInitializable
    {
        private readonly object initializationLock = new object();

        private bool _IsInitialized;

        public bool IsInitialized
        {
            get
            {
                return _IsInitialized;
            }
            private set
            {
                this.SetProperty(ref _IsInitialized, value);
            }
        }

        private int _Location;

        public int Location
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return _Location;
            }
            protected set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                this.SetProperty(ref _Location, value);
            }
        }

        public override string Name
        {
            get
            {
                return this.UniformName;
            }
            set
            {
                throw new NotSupportedException("{0}'s name cannot be set. It is hardcoded in the shader file".FormatWith(typeof(EffectUniform).Name));
            }
        }

        private EffectPass _Pass;

        public EffectPass Pass
        {
            get
            {
                return _Pass;
            }
            protected set
            {
                this.SetProperty(ref _Pass, value);
            }
        }

        private string _UniformName;

        public string UniformName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

                return _UniformName;
            }
            protected set
            {
                Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(value));

                this.SetProperty(ref _UniformName, value);
            }
        }

        protected EffectUniform(EffectPass pass, string name)
        {
            Contract.Requires<ArgumentNullException>(pass != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(name));

            this.Location = GL.GetUniformLocation(pass.ShaderProgram, name);
            this.Pass = pass;
            this.UniformName = name;
        }

        public abstract void Bind();

        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        this.OnInitialize();
                        this.IsInitialized = true;
                    }
                }
            }
        }

        public abstract void Unbind();

        protected abstract void OnInitialize();

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Location >= 0);
            Contract.Invariant(this._Pass != null);
            Contract.Invariant(!string.IsNullOrWhiteSpace(this._UniformName));
        }
    }
}
