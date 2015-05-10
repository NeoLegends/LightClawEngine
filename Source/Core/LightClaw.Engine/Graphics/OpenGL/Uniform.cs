using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Engine.Threading;
using LightClaw.Extensions;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    [DebuggerDisplay("Name = {Name}, Location = {Location}")]
    public class Uniform : DispatcherEntity
    {
        private readonly object initializationLock = new object();

        private int _Location;

        public int Location
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);

                return _Location;
            }
            private set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0);

                this.SetProperty(ref _Location, value);
            }
        }

        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                throw new NotSupportedException("The {0}s name cannot be set. It is hardcoded in the shader file.".FormatWith(typeof(Uniform).Name));
            }
        }

        private ShaderProgram _Program;

        public ShaderProgram Program
        {
            get
            {
                Contract.Ensures(Contract.Result<ShaderProgram>() != null);

                return _Program;
            }
            private set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.SetProperty(ref _Program, value);
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

        public Uniform(ShaderProgram program, int location)
        {
            Contract.Requires<ArgumentNullException>(program != null);
            Contract.Requires<ArgumentOutOfRangeException>(location >= 0);

            this.Location = location;
            this.Program = program;

            int nameLength;
            ActiveUniformType uniformType;
            base.Name = GL.GetActiveUniform(this.Program, this.Location, out nameLength, out uniformType);
            this.Type = uniformType; // Set indirectly to fire event
        }

        public void Set(int value)
        {
            this.VerifyAccess();
            GL.ProgramUniform1(this.Program, this.Location, value);
        }

        [CLSCompliant(false)]
        public void Set(uint value)
        {
            this.VerifyAccess();
            GL.ProgramUniform1(this.Program, this.Location, value);
        }

        public void Set(float value)
        {
            this.VerifyAccess();
            GL.ProgramUniform1(this.Program, this.Location, value);
        }

        public void Set(double value)
        {
            this.VerifyAccess();
            GL.ProgramUniform1(this.Program, this.Location, value);
        }

        public void Set(int value1, int value2)
        {
            this.VerifyAccess();
            GL.ProgramUniform2(this.Program, this.Location, value1, value2);
        }

        [CLSCompliant(false)]
        public void Set(uint value1, uint value2)
        {
            this.VerifyAccess();
            GL.ProgramUniform2(this.Program, this.Location, value1, value2);
        }

        public void Set(float value1, float value2)
        {
            this.VerifyAccess();
            GL.ProgramUniform2(this.Program, this.Location, value1, value2);
        }

        public void Set(double value1, double value2)
        {
            this.VerifyAccess();
            GL.ProgramUniform2(this.Program, this.Location, value1, value2);
        }

        public void Set(int value1, int value2, int value3)
        {
            this.VerifyAccess();
            GL.ProgramUniform3(this.Program, this.Location, value1, value2, value3);
        }

        [CLSCompliant(false)]
        public void Set(uint value1, uint value2, uint value3)
        {
            this.VerifyAccess();
            GL.ProgramUniform3(this.Program, this.Location, value1, value2, value3);
        }

        public void Set(float value1, float value2, float value3)
        {
            this.VerifyAccess();
            GL.ProgramUniform3(this.Program, this.Location, value1, value2, value3);
        }

        public void Set(double value1, double value2, double value3)
        {
            this.VerifyAccess();
            GL.ProgramUniform3(this.Program, this.Location, value1, value2, value3);
        }

        public void Set(int value1, int value2, int value3, int value4)
        {
            this.VerifyAccess();
            GL.ProgramUniform4(this.Program, this.Location, value1, value2, value3, value4);
        }

        [CLSCompliant(false)]
        public void Set(uint value1, uint value2, uint value3, uint value4)
        {
            this.VerifyAccess();
            GL.ProgramUniform4(this.Program, this.Location, value1, value2, value3, value4);
        }

        public void Set(float value1, float value2, float value3, float value4)
        {
            this.VerifyAccess();
            GL.ProgramUniform4(this.Program, this.Location, value1, value2, value3, value4);
        }

        public void Set(double value1, double value2, double value3, double value4)
        {
            this.VerifyAccess();
            GL.ProgramUniform4(this.Program, this.Location, value1, value2, value3, value4);
        }

        public void Set(Vector2 value)
        {
            this.Set(value.X, value.Y);
        }

        public void Set(Vector3 value)
        {
            this.Set(value.X, value.Y, value.Z);
        }

        public void Set(Vector4 value)
        {
            this.Set(value.X, value.Y, value.Z, value.W);
        }

        public void Set(Quaternion value)
        {
            this.Set(value.X, value.Y, value.Z, value.W);
        }

        public void Set(Matrix2 value)
        {
            this.Set(value, false);
        }

        public unsafe void Set(Matrix2 value, bool transpose)
        {
            this.VerifyAccess();

            float* pValue = (float*)&value;
            GL.ProgramUniformMatrix3x2(this.Program, this.Location, 1, transpose, pValue);
        }

        public void Set(Matrix2x3 value)
        {
            this.Set(value, false);
        }

        public unsafe void Set(Matrix2x3 value, bool transpose)
        {
            this.VerifyAccess();

            float* pValue = (float*)&value;
            GL.ProgramUniformMatrix2x3(this.Program, this.Location, 1, transpose, pValue);
        }

        public void Set(Matrix2x4 value)
        {
            this.Set(value, false);
        }

        public unsafe void Set(Matrix2x4 value, bool transpose)
        {
            this.VerifyAccess();

            float* pValue = (float*)&value;
            GL.ProgramUniformMatrix2x4(this.Program, this.Location, 1, transpose, pValue);
        }

        public void Set(Matrix3x2 value)
        {
            this.Set(value, false);
        }

        public unsafe void Set(Matrix3x2 value, bool transpose)
        {
            this.VerifyAccess();

            float* pValue = (float*)&value;
            GL.ProgramUniformMatrix3x2(this.Program, this.Location, 1, transpose, pValue);
        }

        public void Set(Matrix3 value)
        {
            this.Set(value, false);
        }

        public unsafe void Set(Matrix3 value, bool transpose)
        {
            this.VerifyAccess();

            float* pValue = (float*)&value;
            GL.ProgramUniformMatrix3(this.Program, this.Location, 1, transpose, pValue);
        }

        public void Set(Matrix3x4 value)
        {
            this.Set(value, false);
        }

        public unsafe void Set(Matrix3x4 value, bool transpose)
        {
            this.VerifyAccess();

            float* pValue = (float*)&value;
            GL.ProgramUniformMatrix3x4(this.Program, this.Location, 1, transpose, pValue);
        }

        public void Set(Matrix4x2 value)
        {
            this.Set(value, false);
        }

        public unsafe void Set(Matrix4x2 value, bool transpose)
        {
            this.VerifyAccess();

            float* pValue = (float*)&value;
            GL.ProgramUniformMatrix4x2(this.Program, this.Location, 1, transpose, pValue);
        }

        public void Set(Matrix4x3 value)
        {
            this.Set(value, false);
        }

        public unsafe void Set(Matrix4x3 value, bool transpose)
        {
            this.VerifyAccess();

            float* pValue = (float*)&value;
            GL.ProgramUniformMatrix4x3(this.Program, this.Location, 1, transpose, pValue);
        }

        public void Set(Matrix4 value)
        {
            this.Set(value, false);
        }

        public unsafe void Set(Matrix4 value, bool transpose)
        {
            this.VerifyAccess();
            
            float* pValue = (float*)&value;
            GL.ProgramUniformMatrix4(this.Program, this.Location, 1, transpose, pValue);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Location >= 0);
            Contract.Invariant(this._Program != null);
        }
    }
}
