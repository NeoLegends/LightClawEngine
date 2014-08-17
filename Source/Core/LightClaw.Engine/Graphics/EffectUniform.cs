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
    public abstract class EffectUniform : Entity
    {
        private int _Location;

        public int Location
        {
            get
            {
                return _Location;
            }
            protected set
            {
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
                return _Stage;
            }
            protected set
            {
                this.SetProperty(ref _Stage, value);
            }
        }

        private string _UniformName;

        public string UniformName
        {
            get
            {
                return _UniformName;
            }
            protected set
            {
                this.SetProperty(ref _UniformName, value);
            }
        }

        protected EffectUniform() { }

        protected EffectUniform(EffectStage stage, string name)
        {
            Contract.Requires<ArgumentNullException>(stage != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(name));

            this.Location = GL.GetUniformLocation(stage, name);
            this.Stage = stage;
            this.UniformName = name;
        }
    }
}
