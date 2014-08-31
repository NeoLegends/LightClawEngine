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
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class EffectPass : DisposableEntity, IBindable, IInitializable
    {
        private readonly object initializationLock = new object();

        private readonly bool ownsPipeline;

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

        private TextureUnitManager _TextureUnitManager = new TextureUnitManager();

        public TextureUnitManager TextureUnitManager
        {
            get
            {
                Contract.Ensures(Contract.Result<TextureUnitManager>() != null);

                return _TextureUnitManager;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _TextureUnitManager, value);
            }
        }

        private UniformBufferManager _UniformBufferManager = new UniformBufferManager();

        public UniformBufferManager UniformBufferManager
        {
            get
            {
                Contract.Ensures(Contract.Result<UniformBufferManager>() != null);

                return _UniformBufferManager;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _UniformBufferManager, value);
            }
        }

        public EffectPass(ShaderPipeline pipeline, bool ownsPipeline = false) 
        {
            Contract.Requires<ArgumentNullException>(pipeline != null);

            this.ownsPipeline = ownsPipeline;
            this.ShaderPipeline = pipeline;
        }

        public void Bind()
        {
            this.Initialize();
            GL.BindProgramPipeline(this.ShaderPipeline);
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

        public void Unbind()
        {
            GL.BindProgramPipeline(0);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (ownsPipeline)
                {
                    this.ShaderPipeline.Dispose();
                }
                foreach (EffectStage stage in this.Stages.FilterNull())
                {
                    stage.Dispose();
                }
                this.UniformBufferManager.Dispose();

                base.Dispose(disposing);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._ShaderPipeline != null);
            Contract.Invariant(this._Stages != null);
            Contract.Invariant(this._TextureUnitManager != null);
        }

        public static implicit operator ShaderPipeline(EffectPass pass)
        {
            return (pass != null) ? pass.ShaderPipeline : null;
        }
    }
}
