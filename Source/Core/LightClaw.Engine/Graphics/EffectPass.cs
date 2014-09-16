using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        private readonly bool ownsProgram;

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

        private ShaderProgram _ShaderProgram;

        public ShaderProgram ShaderProgram
        {
            get
            {
                Contract.Ensures(Contract.Result<ShaderProgram>() != null);

                return _ShaderProgram;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _ShaderProgram, value);
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

        public EffectPass(ShaderProgram program)
            : this(program, false)
        {
            Contract.Requires<ArgumentNullException>(program != null);
        }

        public EffectPass(ShaderProgram program, bool ownsProgram) 
        {
            Contract.Requires<ArgumentNullException>(program != null);

            this.ownsProgram = ownsProgram;
            this.ShaderProgram = program;
        }

        public void Bind()
        {
            this.Initialize();
            GL.UseProgram(this.ShaderProgram);
        }

        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                lock (this.initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        //this.Stages = this.ShaderPipeline.Programs.FilterNull()
                        //                                          .Select(program => 
                        //                                           {
                        //                                               EffectStage stage = new EffectStage(this, program);
                        //                                               stage.Initialize();
                        //                                               return stage;
                        //                                           })
                        //                                          .ToImmutableList();
                        this.IsInitialized = true;
                    }
                }
            }
        }

        public void Unbind()
        {
            GL.UseProgram(0);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (ownsProgram)
                {
                    this.ShaderProgram.Dispose();
                }
                this.UniformBufferManager.Dispose();

                base.Dispose(disposing);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._ShaderProgram != null);
            Contract.Invariant(this._TextureUnitManager != null);
        }

        public static implicit operator ShaderProgram(EffectPass pass)
        {
            return (pass != null) ? pass.ShaderProgram : null;
        }
    }
}
