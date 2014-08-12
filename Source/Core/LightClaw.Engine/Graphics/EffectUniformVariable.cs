using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public class EffectUniformVariable : EffectUniform
    {
        public void Set(int value)
        {
            GL.ProgramUniform1(this.Stage, this.Location, value);
        }

        public void Set(bool value)
        {
            GL.ProgramUniform1(this.Stage, this.Location, value ? 1 : 0);
        }

        public void Set(float value)
        {
            GL.ProgramUniform1(this.Stage, this.Location, value);
        }

        public void Set(double value)
        {
            GL.ProgramUniform1(this.Stage, this.Location, value);
        }

        public void Set(float value1, float value2)
        {
            GL.ProgramUniform2(this.Stage, this.Location, value1, value2);
        }

        public void Set(double value1, double value2)
        {
            GL.ProgramUniform2(this.Stage, this.Location, value1, value2);
        }

        public void Set(float value1, float value2, float value3)
        {
            GL.ProgramUniform3(this.Stage, this.Location, value1, value2, value3);
        }

        public void Set(double value1, double value2, double value3)
        {
            GL.ProgramUniform3(this.Stage, this.Location, value1, value2, value3);
        }

        public void Set(Vector2 value)
        {
            GL.ProgramUniform2(this.Stage, this.Location, value.X, value.Y);
        }

        public void Set(Vector3 value)
        {
            GL.ProgramUniform3(this.Stage, this.Location, value.X, value.Y, value.Z);
        }

        public void Set(Vector4 value)
        {
            GL.ProgramUniform4(this.Stage, this.Location, value.X, value.Y, value.Z, value.W);
        }

        public void Set(Quaternion value)
        {
            GL.ProgramUniform4(this.Stage, this.Location, value.X, value.Y, value.Z, value.W);
        }

        public void Set(Matrix value)
        {
            GL.ProgramUniformMatrix4(this.Stage, this.Location, 1, false, value.ToArray());
        }

        public void Set(Matrix3x2 value)
        {
            GL.ProgramUniformMatrix3x2(this.Stage, this.Location, 1, false, value.ToArray());
        }

        public void Set(Matrix3x3 value)
        {
            GL.ProgramUniformMatrix3(this.Stage, this.Location, 1, false, value.ToArray());
        }
    }
}
