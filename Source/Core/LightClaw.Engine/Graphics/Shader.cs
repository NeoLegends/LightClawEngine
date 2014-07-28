using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;
using log4net;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class Shader : GLObject
    {
        private static ILog logger = LogManager.GetLogger(typeof(Shader));

        public string Source { get; private set; }

        public ShaderType Type { get; private set; }

        public Shader(string source, ShaderType type)
            : base(GL.CreateShader(type))
        {
            Contract.Requires<ArgumentNullException>(source != null);

            logger.Info("Initializing a new shader of type '{0}'.".FormatWith(type));

            this.Source = source;
            this.Type = type;

            GL.ShaderSource(this, source);
            GL.CompileShader(this);

            int result = 0;
            if (!this.CheckStatus(ShaderParameter.CompileStatus, out result))
            {
                string message = "Compiling the shader (Error Code: {0}) from source ({1}) failed.".FormatWith(result, source);
                logger.Error(message);
                throw new InvalidOperationException(message);
            }

            logger.Debug("Shader initialized.");
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                GL.DeleteShader(this);
            }
            catch (Exception ex)
            {
                logger.Warn(
                    "An exception of type '{0}' was thrown while deleting the shader. Swallowing...".FormatWith(ex.GetType().AssemblyQualifiedName),
                    ex
                );
            }

            base.Dispose(disposing);
        }

        private bool CheckStatus(ShaderParameter parameter)
        {
            int result = 0;
            return CheckStatus(parameter, out result);
        }

        private bool CheckStatus(ShaderParameter parameter, out int result)
        {
            GL.GetShader(this, parameter, out result);
            return (result == 1);
        }
    }
}
