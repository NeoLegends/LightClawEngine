using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Graphics.OpenGL;
using LightClaw.Extensions;
using OpenTK;

namespace LightClaw.Engine.Graphics
{
    public class DataEffectUniform : EffectUniform
    {
        public DataEffectUniform(EffectPass pass, ProgramUniform uniform)
            : base(pass, uniform)
        {
            Contract.Requires<ArgumentNullException>(pass != null);
            Contract.Requires<ArgumentNullException>(uniform != null);
        }

        public void Set(int value)
        {
            this.Uniform.Set(value);
        }

        [CLSCompliant(false)]
        public void Set(uint value)
        {
            this.Uniform.Set(value);
        }

        public void Set(float value)
        {
            this.Uniform.Set(value);
        }

        public void Set(double value)
        {
            this.Uniform.Set(value);
        }

        public void Set(int value1, int value2)
        {
            this.Uniform.Set(value1, value2);
        }

        [CLSCompliant(false)]
        public void Set(uint value1, uint value2)
        {
            this.Uniform.Set(value1, value2);
        }

        public void Set(float value1, float value2)
        {
            this.Uniform.Set(value1, value2);
        }

        public void Set(double value1, double value2)
        {
            this.Uniform.Set(value1, value2);
        }

        public void Set(int value1, int value2, int value3)
        {
            this.Uniform.Set(value1, value2, value3);
        }

        [CLSCompliant(false)]
        public void Set(uint value1, uint value2, uint value3)
        {
            this.Uniform.Set(value1, value2, value3);
        }

        public void Set(float value1, float value2, float value3)
        {
            this.Uniform.Set(value1, value2, value3);
        }

        public void Set(double value1, double value2, double value3)
        {
            this.Uniform.Set(value1, value2, value3);
        }

        public void Set(int value1, int value2, int value3, int value4)
        {
            this.Uniform.Set(value1, value2, value3, value4);
        }

        [CLSCompliant(false)]
        public void Set(uint value1, uint value2, uint value3, uint value4)
        {
            this.Uniform.Set(value1, value2, value3, value4);
        }

        public void Set(float value1, float value2, float value3, float value4)
        {
            this.Uniform.Set(value1, value2, value3, value4);
        }

        public void Set(double value1, double value2, double value3, double value4)
        {
            this.Uniform.Set(value1, value2, value3, value4);
        }

        public void Set(Vector2 value)
        {
            this.Uniform.Set(value);
        }

        public void Set(Vector3 value)
        {
            this.Uniform.Set(value);
        }

        public void Set(Vector4 value)
        {
            this.Uniform.Set(value);
        }

        public void Set(Quaternion value)
        {
            this.Uniform.Set(value);
        }

        public void Set(ref Matrix2 value)
        {
            this.Uniform.Set(ref value);
        }

        public void Set(ref Matrix2 value, bool transpose)
        {
            this.Uniform.Set(ref value, transpose);
        }

        public void Set(ref Matrix2x3 value)
        {
            this.Uniform.Set(ref value);
        }

        public void Set(ref Matrix2x3 value, bool transpose)
        {
            this.Uniform.Set(ref value, transpose);
        }

        public void Set(ref Matrix2x4 value)
        {
            this.Uniform.Set(ref value);
        }

        public void Set(ref Matrix2x4 value, bool transpose)
        {
            this.Uniform.Set(ref value, transpose);
        }

        public void Set(ref Matrix3x2 value)
        {
            this.Uniform.Set(ref value);
        }

        public void Set(ref Matrix3x2 value, bool transpose)
        {
            this.Uniform.Set(ref value, transpose);
        }

        public void Set(ref Matrix3 value)
        {
            this.Uniform.Set(ref value);
        }

        public void Set(ref Matrix3 value, bool transpose)
        {
            this.Uniform.Set(ref value, transpose);
        }

        public void Set(ref Matrix3x4 value)
        {
            this.Uniform.Set(ref value);
        }

        public void Set(ref Matrix3x4 value, bool transpose)
        {
            this.Uniform.Set(ref value, transpose);
        }

        public void Set(ref Matrix4x2 value)
        {
            this.Uniform.Set(ref value);
        }

        public void Set(ref Matrix4x2 value, bool transpose)
        {
            this.Uniform.Set(ref value, transpose);
        }

        public void Set(ref Matrix4x3 value)
        {
            this.Uniform.Set(ref value);
        }

        public void Set(ref Matrix4x3 value, bool transpose)
        {
            this.Uniform.Set(ref value, transpose);
        }

        public void Set(ref Matrix4 value)
        {
            this.Uniform.Set(ref value);
        }

        public void Set(ref Matrix4 value, bool transpose)
        {
            this.Uniform.Set(ref value, transpose);
        }

        public override Binding Bind() 
        {
            return default(Binding);
        }

        public override void Unbind() { }
    }
}
