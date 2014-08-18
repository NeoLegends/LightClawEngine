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

        private string[] _Source;

        public string[] Sources
        {
            get
            {
                return _Source;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Source, value);
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

            this.Sources = sources;
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
                        this.Handle = GL.CreateShaderProgram(this.Type, this.Sources.Length, this.Sources);

                        int result;
                        GL.GetProgram(this, GetProgramParameterName.LinkStatus, out result);
                        if (result == 0)
                        {
                            string message = "{0} creation failed. Error code: {1}; Info Log: {2}.".FormatWith(typeof(ShaderProgram).Name, result, GL.GetProgramInfoLog(this));
                            Logger.Warn(() => message);
                            throw new CompilationFailedException(message);
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
    }
}
