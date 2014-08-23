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

namespace LightClaw.Engine.Graphics
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

        private int _TextureUnit;

        public int TextureUnit
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return _TextureUnit;
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                this.SetProperty(ref _TextureUnit, value);
            }
        }

        public Sampler(int textureUnit, IEnumerable<SamplerParameterDescription> parameters)
        {
            Contract.Requires<ArgumentOutOfRangeException>(textureUnit >= 0);
            Contract.Requires<ArgumentNullException>(parameters != null);

            this.TextureUnit = textureUnit;
            this.Parameters = parameters.ToImmutableList();
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

        public void Bind()
        {
            int unit = this.TextureUnit;
            if (unit < 0)
            {
                throw new InvalidOperationException("The texture unit to bind to was {0} but has to be greater than or equal to zero.".FormatWith(unit));
            }
            this.Bind(this.TextureUnit);
        }

        public void Bind(int textureUnit)
        {
            Contract.Requires<ArgumentOutOfRangeException>(textureUnit >= 0);

            this.Initialize();
            this.TextureUnit = textureUnit;
            GL.BindSampler(textureUnit, this);
        }

        public void Unbind()
        {
            this.Unbind(this.TextureUnit);
        }

        public void Unbind(int textureUnit)
        {
            Contract.Requires<ArgumentOutOfRangeException>(textureUnit >= 0);

            ThreadF.AssertMainThread();
            GL.BindSampler(textureUnit, 0);
        }

        protected override void Dispose(bool disposing)
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

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._TextureUnit >= 0);
        }
    }
}
