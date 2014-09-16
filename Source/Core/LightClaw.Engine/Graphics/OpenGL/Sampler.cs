using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    public class Sampler : GLObject, IBindable, IInitializable
    {
        private readonly object initializationLock = new object();

        private bool _IsInitialized = false;

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

        private ImmutableList<SamplerParameterDescription> _Parameters;

        public ImmutableList<SamplerParameterDescription> Parameters
        {
            get
            {
                return _Parameters;
            }
            private set
            {
                this.SetProperty(ref _Parameters, value);
            }
        }

        private TextureUnit _TextureUnit;

        public TextureUnit TextureUnit
        {
            get
            {
                Contract.Ensures(Contract.Result<TextureUnit>() >= 0);

                return _TextureUnit;
            }
            private set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                this.SetProperty(ref _TextureUnit, value);
            }
        }

        public Sampler(TextureUnit textureUnit, IEnumerable<SamplerParameterDescription> parameters)
        {
            Contract.Requires<ArgumentOutOfRangeException>(textureUnit >= 0);
            Contract.Requires<ArgumentNullException>(parameters != null);

            this.TextureUnit = textureUnit;
            this.Parameters = parameters.ToImmutableList();
        }

        public void Bind()
        {
            this.Initialize();
            GL.BindSampler(this.TextureUnit, this);
        }

        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        this.Handle = GL.GenSampler();
                        foreach (SamplerParameterDescription description in this.Parameters.EnsureNonNull())
                        {
                            GL.SamplerParameter(this, description.ParameterName, description.Value);
                        }

                        this.IsInitialized = true;
                    }
                }
            }
        }

        public void Unbind()
        {
            GL.BindSampler(this.TextureUnit, 0);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                try
                {
                    GL.DeleteSampler(this);
                }
                catch (Exception ex)
                {
                    Logger.Warn(() => "An error of type '{0}' occured while disposing the {0}'s underlying OpenGL Sampler.".FormatWith(ex.GetType().AssemblyQualifiedName, typeof(Sampler).Name), ex);
                }
                base.Dispose(disposing);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._TextureUnit >= 0);
        }
    }
}
