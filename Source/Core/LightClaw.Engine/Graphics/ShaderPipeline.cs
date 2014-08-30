using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class ShaderPipeline : GLObject, IBindable, IInitializable
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

        private ImmutableList<ShaderProgram> _Programs;

        public ImmutableList<ShaderProgram> Programs
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableList<ShaderProgram>>() != null);

                return _Programs;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                Contract.Requires<ArgumentException>(value.All(program => program != null));

                this.SetProperty(ref _Programs, value);
            }
        }

        public ShaderPipeline(params ShaderProgram[] programs)
        {
            Contract.Requires<ArgumentNullException>(programs != null);
            Contract.Requires<ArgumentException>(programs.Any(program => program.Type == ShaderType.FragmentShader));
            Contract.Requires<ArgumentException>(programs.Any(program => program.Type == ShaderType.VertexShader));
            Contract.Requires<ArgumentException>(!programs.ContainsDuplicates(program => program.Type));

            this.Programs = programs.FilterNull().ToImmutableList();
        }

        public void Bind()
        {
            GL.BindProgramPipeline(this);
        }

        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        this.Handle = GL.GenProgramPipeline();
                        foreach (ShaderProgram program in this.Programs)
                        {
                            program.Initialize();
                            GL.UseProgramStages(this, GetProgramStageMask(program.Type), program);
                        }

                        this.IsInitialized = true;
                    }
                }
            }
        }

        public void Unbind()
        {
            GL.BindProgramPipeline(0);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                try
                {
                    GL.UseProgramStages(this, ProgramStageMask.AllShaderBits, 0);
                    GL.DeleteProgramPipeline(this);
                }
                catch (Exception ex)
                {
                    Logger.Warn(() => "An exception of type '{0}' happened while disposing the {1}'s underlying shader program.".FormatWith(ex.GetType().AssemblyQualifiedName, typeof(ShaderPipeline).Name));
                }
                base.Dispose(disposing);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Programs != null);
        }

        [ContractVerification(false)] // Spills out some obscure warning about "shaderType != 36313" always leading to the same result
        private static ProgramStageMask GetProgramStageMask(ShaderType shaderType)
        {
            switch (shaderType)
            {
                case ShaderType.ComputeShader:
                    return ProgramStageMask.ComputeShaderBit;
                case ShaderType.FragmentShader:
                    return ProgramStageMask.FragmentShaderBit;
                case ShaderType.GeometryShader:
                    return ProgramStageMask.GeometryShaderBit;
                case ShaderType.TessControlShader:
                    return ProgramStageMask.TessControlShaderBit;
                case ShaderType.TessEvaluationShader:
                    return ProgramStageMask.TessEvaluationShaderBit;
                case ShaderType.VertexShader:
                    return ProgramStageMask.VertexShaderBit;
                default:
                    throw new NotSupportedException("Getting the program stage mask is only supported for predefined values in ShaderType.");
            }
        }
    }
}
