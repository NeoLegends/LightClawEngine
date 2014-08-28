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
    public class EffectPass : Entity, IBindable, IInitializable
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

        private ShaderPipeline _ShaderPipeline;

        public ShaderPipeline ShaderPipeline
        {
            get
            {
                Contract.Ensures(Contract.Result<ShaderPipeline>() != null);

                return _ShaderPipeline;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _ShaderPipeline, value);
            }
        }

        private ImmutableList<EffectStage> _Stages = ImmutableList<EffectStage>.Empty;

        public ImmutableList<EffectStage> Stages
        {
            get
            {
                Contract.Ensures(Contract.Result<ImmutableList<EffectStage>>() != null);

                return _Stages;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                Contract.Requires<ArgumentException>(value.All(stage => stage != null));

                this.SetProperty(ref _Stages, value);
            }
        }

        private UniformBufferPool _UboPool;

        public UniformBufferPool UboPool
        {
            get
            {
                Contract.Ensures(Contract.Result<UniformBufferPool>() != null);

                return _UboPool;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _UboPool, value);
            }
        }

        public EffectPass(ShaderPipeline pipeline) 
            : this(pipeline, UniformBufferPool.Default)
        {
            Contract.Requires<ArgumentNullException>(pipeline != null);

            this.ShaderPipeline = pipeline;
        }

        public EffectPass(ShaderPipeline pipeline, UniformBufferPool uboPool)
        {
            Contract.Requires<ArgumentNullException>(pipeline != null);
            Contract.Requires<ArgumentNullException>(uboPool != null);

            this.ShaderPipeline = pipeline;
            this.UboPool = uboPool;
        }

        public void Bind()
        {
            this.Initialize();
            GL.BindProgramPipeline(this.ShaderPipeline);
        }

        public void Unbind()
        {
            GL.BindProgramPipeline(0);
        }

        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        this.Stages = this.ShaderPipeline.Programs.FilterNull()
                                                                  .Select(program => 
                                                                   {
                                                                       EffectStage stage = new EffectStage(this, program);
                                                                       stage.Initialize();
                                                                       return stage;
                                                                   })
                                                                  .ToImmutableList();
                        this.IsInitialized = true;
                    }
                }
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._ShaderPipeline != null);
            Contract.Invariant(this._Stages != null);
            Contract.Invariant(this._UboPool != null);
        }

        public static implicit operator ShaderPipeline(EffectPass pass)
        {
            return (pass != null) ? pass.ShaderPipeline : null;
        }
    }
}
