using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics
{
    public abstract class UniformLocation
    {
        public int Index { get; private set; }

        public string Name { get; private set; }

        public ShaderProgram Program { get; private set; }

        protected UniformLocation() { }

        protected UniformLocation(ShaderProgram program, string name)
        {
            Contract.Requires<ArgumentNullException>(program != null);
            Contract.Requires<ArgumentNullException>(name != null);

            this.Index = GL.GetUniformLocation(program, name);
            this.Name = name;
            this.Program = program;
        }
    }

    public class Int32UniformLocation : UniformLocation
    {
        public int Value
        {
            get
            {
                int result = 0;
                GL.GetUniform(this.Program, this.Index, out result);
                return result;
            }
            set
            {
                GL.ProgramUniform1(this.Program, this.Index, value);
            }
        }

        public Int32UniformLocation(ShaderProgram program, string name)
            : base(program, name)
        {
            Contract.Requires<ArgumentNullException>(program != null);
            Contract.Requires<ArgumentNullException>(name != null);
        }
    }

    public class SingleUniformLocation : UniformLocation
    {
        public float Value
        {
            get
            {
                float result = 0.0f;
                GL.GetUniform(this.Program, this.Index, out result);
                return result;
            }
            set
            {
                GL.ProgramUniform1(this.Program, this.Index, value);
            }
        }

        public SingleUniformLocation(ShaderProgram program, string name)
            : base(program, name)
        {
            Contract.Requires<ArgumentNullException>(program != null);
            Contract.Requires<ArgumentNullException>(name != null);
        }
    }

    public class DoubleUniformLocation : UniformLocation
    {
        public double Value
        {
            get
            {
                double result = 0.0;
                GL.GetUniform(this.Program, this.Index, out result);
                return result;
            }
            set
            {
                GL.ProgramUniform1(this.Program, this.Index, value);
            }
        }

        public DoubleUniformLocation(ShaderProgram program, string name)
            : base(program, name)
        {
            Contract.Requires<ArgumentNullException>(program != null);
            Contract.Requires<ArgumentNullException>(name != null);
        }
    }

    public class Vector2UniformLocation : UniformLocation
    {
        public Vector2 Value
        {
            get
            {
                float[] result = new float[2];
                GL.GetUniform(this.Program, this.Index, result);
                return new Vector2(result);
            }
            set
            {
                GL.ProgramUniform2(this.Program, this.Index, value.X, value.Y);
            }
        }

        public Vector2UniformLocation(ShaderProgram program, string name)
            : base(program, name)
        {
            Contract.Requires<ArgumentNullException>(program != null);
            Contract.Requires<ArgumentNullException>(name != null);
        }
    }

    public class Vector3UniformLocation : UniformLocation
    {
        public Vector3 Value
        {
            get
            {
                float[] result = new float[3];
                GL.GetUniform(this.Program, this.Index, result);
                return new Vector3(result);
            }
            set
            {
                GL.ProgramUniform3(this.Program, this.Index, value.X, value.Y, value.Z);
            }
        }

        public Vector3UniformLocation(ShaderProgram program, string name)
            : base(program, name)
        {
            Contract.Requires<ArgumentNullException>(program != null);
            Contract.Requires<ArgumentNullException>(name != null);
        }
    }

    public class Vector4UniformLocation : UniformLocation
    {
        public Vector4 Value
        {
            get
            {
                float[] result = new float[4];
                GL.GetUniform(this.Program, this.Index, result);
                return new Vector4(result);
            }
            set
            {
                GL.ProgramUniform4(this.Program, this.Index, value.X, value.Y, value.Z, value.W);
            }
        }

        public Vector4UniformLocation(ShaderProgram program, string name)
            : base(program, name)
        {
            Contract.Requires<ArgumentNullException>(program != null);
            Contract.Requires<ArgumentNullException>(name != null);
        }
    }

    public class MatrixUniformLocation : UniformLocation
    {
        public Matrix Value
        {
            get
            {
                float[] result = new float[16];
                GL.GetUniform(this.Program, this.Index, result);
                return new Matrix(result);
            }
            set
            {
                GL.ProgramUniformMatrix4(this.Program, this.Index, 1, false, value.ToArray());
            }
        }

        public MatrixUniformLocation(ShaderProgram program, string name)
            : base(program, name)
        {
            Contract.Requires<ArgumentNullException>(program != null);
            Contract.Requires<ArgumentNullException>(name != null);
        }
    }

    public class Matrix3x2UniformLocation : UniformLocation
    {
        public Matrix3x2 Value
        {
            get
            {
                float[] result = new float[6];
                GL.GetUniform(this.Program, this.Index, result);
                return new Matrix3x2(result);
            }
            set
            {
                GL.ProgramUniformMatrix4(this.Program, this.Index, 1, false, value.ToArray());
            }
        }

        public Matrix3x2UniformLocation(ShaderProgram program, string name)
            : base(program, name)
        {
            Contract.Requires<ArgumentNullException>(program != null);
            Contract.Requires<ArgumentNullException>(name != null);
        }
    }

    public class Matrix3x3UniformLocation : UniformLocation
    {
        public Matrix3x3 Value
        {
            get
            {
                float[] result = new float[9];
                GL.GetUniform(this.Program, this.Index, result);
                return new Matrix3x3(result);
            }
            set
            {
                GL.ProgramUniformMatrix4(this.Program, this.Index, 1, false, value.ToArray());
            }
        }

        public Matrix3x3UniformLocation(ShaderProgram program, string name)
            : base(program, name)
        {
            Contract.Requires<ArgumentNullException>(program != null);
            Contract.Requires<ArgumentNullException>(name != null);
        }
    }

    public class Matrix5x4UniformLocation : UniformLocation
    {
        public Matrix3x3 Value
        {
            get
            {
                float[] result = new float[20];
                GL.GetUniform(this.Program, this.Index, result);
                return new Matrix3x3(result);
            }
            set
            {
                GL.ProgramUniformMatrix4(this.Program, this.Index, 1, false, value.ToArray());
            }
        }

        public Matrix5x4UniformLocation(ShaderProgram program, string name)
            : base(program, name)
        {
            Contract.Requires<ArgumentNullException>(program != null);
            Contract.Requires<ArgumentNullException>(name != null);
        }
    }
}
