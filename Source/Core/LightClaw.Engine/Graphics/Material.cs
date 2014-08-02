using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    public abstract class Material : Entity, IBindable, IUpdateable, ILateUpdateable
    {
        public event EventHandler<ParameterEventArgs> Updating;

        public event EventHandler<ParameterEventArgs> Updated;

        public event EventHandler<ParameterEventArgs> LateUpdating;

        public event EventHandler<ParameterEventArgs> LateUpdated;

        private Shader _Shader;

        public Shader Shader
        {
            get
            {
                return _Shader;
            }
            set
            {
                this.SetProperty(ref _Shader, value);
            }
        }

        public void Bind()
        {
            Shader s = this.Shader;
            if (s != null)
            {
                s.Bind();
            }
        }

        public void Unbind()
        {
            Shader s = this.Shader;
            if (s != null)
            {
                s.Unbind();
            }
        }

        public void Update(GameTime gameTime)
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.Updating, this.Updated))
            {
                Shader s = this.Shader;
                if (s != null)
                {
                    s.Update(gameTime);
                }
            }
        }

        public void LateUpdate()
        {
            using (ParameterEventArgsRaiser raiser = new ParameterEventArgsRaiser(this, this.LateUpdating, this.LateUpdated))
            {
                Shader s = this.Shader;
                if (s != null)
                {
                    s.LateUpdate();
                }
            }
        }

        protected T GetShader<T>()
            where T : Shader
        {
            return (T)this.Shader;
        }
    }
}
