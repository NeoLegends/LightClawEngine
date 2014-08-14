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
    public class EffectPass : GLObject, IBindable
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

        public override string Name
        {
            get
            {
                return this.PassName;
            }
            set
            {
                throw new NotSupportedException("{0}'s name cannot be set.".FormatWith(typeof(EffectPass).Name));
            }
        }

        private string _PassName;

        public string PassName
        {
            get
            {
                return _PassName;
            }
            private set
            {
                this.SetProperty(ref _PassName, value);
            }
        }

        private ObservableCollection<EffectStage> _Stages = new ObservableCollection<EffectStage>();

        public ObservableCollection<EffectStage> Stages
        {
            get
            {
                Contract.Ensures(Contract.Result<ObservableCollection<EffectStage>>() != null);

                return _Stages;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Stages, value);
            }
        }

        public EffectPass() 
        {
            this.Stages.CollectionChanged += (s, e) =>
            {
                if (e.Action != NotifyCollectionChangedAction.Move)
                {
                    if (e.OldItems != null)
                    {
                        foreach (EffectStage stage in e.OldItems)
                        {
                            GL.UseProgramStages(this, GetProgramStageMask(stage.Type), 0);
                        }
                    }
                    if (e.NewItems != null)
                    {
                        foreach (EffectStage stage in e.NewItems)
                        {
                            GL.UseProgramStages(this, GetProgramStageMask(stage.Type), stage);
                        }
                    }
                }
            };
        }

        public EffectPass(IEnumerable<EffectStage> stages)
            : this()
        {
            Contract.Requires<ArgumentNullException>(stages != null);
            Contract.Requires<ArgumentException>(!stages.Duplicates(stage => stage.Type));

            this.Initialize(stages);
        }

        public void Bind()
        {
            GL.BindProgramPipeline(this);
        }

        public void Unbind()
        {
            GL.BindProgramPipeline(0);
        }

        public void Initialize(IEnumerable<EffectStage> stages)
        {
            Contract.Requires<ArgumentNullException>(stages != null);
            Contract.Requires<ArgumentException>(!stages.Duplicates(stage => stage.Type));

            if (!this.IsInitialized) // Double check to avoid lock acquiring, if possible
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        this.Stages.AddRange(stages); // Attachment / detachment will take place in event handler

                        this.IsInitialized = true;
                        return;
                    }
                }
            }

            throw new NotSupportedException("{0} already initialized. Cannot be initialized again. Create a new one instead.".FormatWith(typeof(EffectPass).Name));
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                GL.UseProgramStages(this, ProgramStageMask.AllShaderBits, 0);
                GL.DeleteProgramPipeline(this);
            }
            catch (Exception ex)
            {
                logger.Warn("An error occured while disposing the {0}'s underlying OpenGL Program Pipeline.".FormatWith(typeof(EffectPass).Name), ex);
            }
            base.Dispose(disposing);
        }

        private bool CheckStatus(ProgramPipelineParameter pName)
        {
            return (QueryValue(pName) == 1);
        }

        private bool CheckStatus(ProgramPipelineParameter pName, out int result)
        {
            result = this.QueryValue(pName);
            return (result == 1);
        }

        private int QueryValue(ProgramPipelineParameter pName)
        {
            int result;
            GL.GetProgramPipeline(this, pName, out result);
            return result;
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

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Stages != null);
        }
    }
}
