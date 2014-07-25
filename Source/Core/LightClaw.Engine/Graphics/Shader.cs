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

        public ShaderType Type { get; private set; }

        private Shader(ShaderType type)
            : base(GL.CreateShader(type))
        {
            this.Type = type;
        }

        public Shader(string source, ShaderType type)
            : this(type)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            GL.ShaderSource(this, source);
            GL.CompileShader(this);

            int result = 0;
            if (!this.CheckStatus(ShaderParameter.CompileStatus, out result))
            {
                string message = "Compiling shader (Error Code: {0}) from source ({1}) failed.".FormatWith(result, source);
                logger.Error(message);
                throw new InvalidOperationException(message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                GL.DeleteShader(this);
                int result = 0;
                if (!this.CheckStatus(ShaderParameter.DeleteStatus, out result))
                {
                    logger.Warn("Deleting the shader failed. Error Code: {0}".FormatWith(result));
                }
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
