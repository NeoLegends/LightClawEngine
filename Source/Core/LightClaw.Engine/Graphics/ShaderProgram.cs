using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Extensions;
using log4net;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class ShaderProgram : GLObject, IBindable
    {
        private static ILog logger = LogManager.GetLogger(typeof(ShaderProgram));

        public int Count { get; private set; }

        public ImmutableList<UniformLocation> UniformLocations { get; private set; }

        public ImmutableList<Shader> Shaders { get; private set; }

        public ShaderProgram(IEnumerable<Shader> shaders, IEnumerable<UniformLocation> uniformLocations)
        {
            Contract.Requires<ArgumentNullException>(shaders != null);
            Contract.Requires<ArgumentNullException>(uniformLocations != null);

            this.UniformLocations = uniformLocations.ToImmutableList();
            this.Shaders = shaders.ToImmutableList();

            foreach (Shader shader in this.Shaders)
            {
                GL.AttachShader(this, shader);
            }
            GL.LinkProgram(this);

            int result;
            if (!this.CheckStatus(GetProgramParameterName.LinkStatus, out result))
            {
                string message = "Linking the the shader program failed. Error Code: ".FormatWith(result);
                logger.Error(message);
                throw new InvalidOperationException(message);
            }
        }

        public void Bind()
        {
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
                foreach (Shader shader in this.Shaders)
                {
                    GL.DetachShader(this, shader);
                }
            }
            catch (Exception ex)
            {
                logger.Warn(
                    "An exception of type '{0}' was thrown while detaching the shaders from the program. Swallowing...".FormatWith(ex.GetType().AssemblyQualifiedName),
                    ex
                );
            }
            try
            {
                GL.DeleteProgram(this);

                int result;
                if (!this.CheckStatus(GetProgramParameterName.DeleteStatus, out result))
                {
                    logger.Warn("Deleting the shader program failed. Error Code: {0}".FormatWith(result));
                }
            }
            catch (Exception ex)
            {
                logger.Warn(
                    "An exception of type '{0}' was thrown while deleting the ShaderProgram. Swallowing...".FormatWith(ex.GetType().AssemblyQualifiedName),
                    ex
                );
            }
            base.Dispose(disposing);
        }

        private bool CheckStatus(GetProgramParameterName parameter)
        {
            int result = 0;
            return CheckStatus(parameter, out result);
        }

        private bool CheckStatus(GetProgramParameterName parameter, out int result)
        {
            GL.GetProgram(this, parameter, out result);
            return (result == 1);
        }
    }
}
