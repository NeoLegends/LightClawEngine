using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public abstract class EffectUniform : DisposableEntity
    {
        private int _Location;

        public int Location
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return _Location;
            }
            protected set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                this.SetProperty(ref _Location, value);
            }
        }

        public override string Name
        {
            get
            {
                return this.UniformName;
            }
            set
            {
                throw new NotSupportedException("{0}'s name cannot be set. It is hardcoded in the shader file".FormatWith(typeof(EffectUniform).Name));
            }
        }

        private EffectStage _Stage;

        public EffectStage Stage
        {
            get
            {
                Contract.Ensures(Contract.Result<EffectStage>() != null);

                return _Stage;
            }
            protected set
            {
                Contract.Requires(value != null);

                this.SetProperty(ref _Stage, value);
            }
        }

        private string _UniformName;

        public string UniformName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

                return _UniformName;
            }
            protected set
            {
                Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(value));

                this.SetProperty(ref _UniformName, value);
            }
        }

        protected EffectUniform(EffectStage stage, string name)
        {
            Contract.Requires<ArgumentNullException>(stage != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(name));

            this.Location = GL.GetUniformLocation(stage.ShaderProgram, name);
            this.Stage = stage;
            this.UniformName = name;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Location >= 0);
            Contract.Invariant(this._Stage != null);
            Contract.Invariant(!string.IsNullOrWhiteSpace(this._UniformName));
        }
    }
}
