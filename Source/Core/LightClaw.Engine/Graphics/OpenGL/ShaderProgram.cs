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
    public class ShaderProgram : GLObject, IBindable, IInitializable
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

        private ImmutableList<string> _Sources;

        public ImmutableList<string> Sources
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableList<string>>() != null);

                return _Sources;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                Contract.Requires<ArgumentException>(value.All(source => source != null)); // No IsNullOrEmpty here as the compiler will just ignore it, no need for beef

                this.SetProperty(ref _Sources, value);
            }
        }

        private ShaderType _Type;

        public ShaderType Type
        {
            get
            {
                return _Type;
            }
            private set
            {
                this.SetProperty(ref _Type, value);
            }
        }

        public ShaderProgram(string[] sources, ShaderType type)
        {
            Contract.Requires<ArgumentNullException>(sources != null);
            Contract.Requires<ArgumentException>(sources.Any(source => !string.IsNullOrWhiteSpace(source)));

            this.Sources = sources.ToImmutableList();
            this.Type = type;
        }

        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        this.Handle = GL.CreateShaderProgram(this.Type, this.Sources.Count, this.Sources.ToArray());

                        int result;
                        GL.GetProgram(this, GetProgramParameterName.LinkStatus, out result);
                        if (result == 0)
                        {
                            string infoLog = GL.GetProgramInfoLog(this);
                            string message = "{0} creation failed. Error code: {1}; Info Log: {2}.".FormatWith(typeof(ShaderProgram).Name, result, infoLog);
                            Logger.Warn(message); // Message already created, so use direct logging call instead of lambda
                            throw new CompilationFailedException(message, infoLog, result);
                        }

                        this.IsInitialized = true;
                    }
                }
            }
        }

        public void Bind()
        {
            this.Initialize();
            GL.UseProgram(this);
        }

        public void Unbind()
        {
            GL.UseProgram(0);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                GL.DeleteProgram(this);
            }
            catch (Exception ex)
            {
                Logger.Warn(() => "An exception of type '{0}' was thrown while disposing the {1}'s underlying OpenGL Shader Program.".FormatWith(ex.GetType().AssemblyQualifiedName, typeof(ShaderProgram).Name), ex);
            }
            base.Dispose(disposing);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Sources != null);
        }
    }
}
