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

                this.SetProperty(ref _Stages, value);
            }
        }

        private UniformBufferPool _UboPool = UniformBufferPool.Default;

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
        {
            Contract.Requires<ArgumentNullException>(pipeline != null);

            this.ShaderPipeline = pipeline;
        }

        public void Bind()
        {
            throw new NotImplementedException();
        }

        public void Unbind()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        this.Stages = this.ShaderPipeline.Programs.FilterNull().Select(program => new EffectStage(program)).ToImmutableList();
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
    }
}
