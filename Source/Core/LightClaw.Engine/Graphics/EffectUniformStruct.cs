using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    public class EffectUniformStruct : EffectUniform
    {
        private List<EffectUniformVariable> _UniformVariables = new List<EffectUniformVariable>();

        public List<EffectUniformVariable> UniformVariables
        {
            get
            {
                return _UniformVariables;
            }
            private set
            {
                this.SetProperty(ref _UniformVariables, value);
            }
        }

        private List<EffectUniformStruct> _Structs = new List<EffectUniformStruct>();

        public List<EffectUniformStruct> Structs
        {
            get
            {
                return _Structs;
            }
            private set
            {
                this.SetProperty(ref _Structs, value);
            }
        }
    }
}
