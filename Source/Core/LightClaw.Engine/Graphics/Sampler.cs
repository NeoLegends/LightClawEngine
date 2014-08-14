using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class Sampler : GLObject, IBindable
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

        private ImmutableList<SamplerParameterDescription> _Parameters = ImmutableList<SamplerParameterDescription>.Empty;

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

                return Math.Max(_TextureUnit, 0);
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                this.SetProperty(ref _TextureUnit, value);
            }
        }

        public Sampler() { }

        public Sampler(int textureUnit, IEnumerable<SamplerParameterDescription> parameters)
        {
            Contract.Requires<ArgumentOutOfRangeException>(textureUnit >= 0);
            Contract.Requires<ArgumentNullException>(parameters != null);

            this.Initialize(textureUnit, parameters);
        }

        public void Initialize(int textureUnit, IEnumerable<SamplerParameterDescription> parameters)
        {
            Contract.Requires<ArgumentOutOfRangeException>(textureUnit >= 0);
            Contract.Requires<ArgumentNullException>(parameters != null);

            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        this.Handle = GL.GenSampler();
                        this.Parameters = parameters.ToImmutableList();
                        this.TextureUnit = TextureUnit;

                        foreach (SamplerParameterDescription description in parameters)
                        {
                            GL.SamplerParameter(this, description.ParameterName, description.Value);
                        }

                        this.IsInitialized = true;
                        return;
                    }
                }
            }

            throw new NotSupportedException("{0}s cannot be initialized twice.".FormatWith(typeof(Sampler).Name));
        }

        public void Bind()
        {
            this.Bind(this.TextureUnit);
        }

        public void Bind(int textureUnit)
        {
            GL.BindSampler(textureUnit, this);
        }

        public void Unbind()
        {
            this.Unbind(this.TextureUnit);
        }

        public void Unbind(int textureUnit)
        {
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
                logger.Warn(() => "An error of type '{0}' occured while disposing the {0}'s underlying OpenGL Sampler.".FormatWith(ex.GetType().AssemblyQualifiedName, typeof(Sampler).Name), ex);
            }
            base.Dispose(disposing);
        }
    }
}
