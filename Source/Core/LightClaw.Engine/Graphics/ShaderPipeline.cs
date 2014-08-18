using System;
using System.Collections.Generic;
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

        private ObservableCollection<ShaderProgram> _Programs = new ObservableCollection<ShaderProgram>();

        public ObservableCollection<ShaderProgram> Programs
        {
            get
            {
                Contract.Ensures(Contract.Result<ObservableCollection<ShaderProgram>>() != null);

                return _Programs;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Programs, value);
            }
        }

        public ShaderPipeline(IEnumerable<ShaderProgram> programs)
        {
            Contract.Requires<ArgumentNullException>(programs != null);
            Contract.Requires<ArgumentException>(programs.Any());

            this.Programs.CollectionChanged += (s, e) =>
            {
                if (e.Action != NotifyCollectionChangedAction.Move)
                {
                    if (e.OldItems != null)
                    {
                        foreach (ShaderProgram program in e.OldItems)
                        {
                            GL.UseProgramStages(this, GetProgramStageMask(program.Type), 0);
                        }
                    }
                    if (e.NewItems != null)
                    {
                        foreach (ShaderProgram program in e.NewItems)
                        {
                            GL.UseProgramStages(this, GetProgramStageMask(program.Type), program);
                        }
                    }
                }
            };
            this.Programs.AddRange(programs);
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
                        foreach (ShaderProgram program in this.Programs.FilterNull())
                        {
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

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Programs != null);
        }

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
