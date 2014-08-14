using System;
using System.Collections.Generic;
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
            private set
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
            private set
            {
                this.SetProperty(ref _Stage, value);
            }
        }

        private ActiveUniformType _Type;

        public ActiveUniformType Type
        {
            get
            {
                return _Type;
            }
            private set
            {
                this.SetProperty(ref _Type, value);
            }
        }

        private string _UniformName;

        public string UniformName
        {
            get
            {
                return _UniformName;
            }
            private set
            {
                this.SetProperty(ref _UniformName, value);
            }
        }
    }
}
