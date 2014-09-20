using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;
using LightClaw.Extensions;
using OpenTK.Graphics.OpenGL4;

namespace LightClaw.Engine.Graphics.OpenGL
{
    public class Uniform : Entity, IInitializable
    {
        private readonly object initializationLock = new object();

        private bool _IsInitialized;

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
                throw new NotSupportedException("The {0}s name cannot be set.".FormatWith(typeof(Uniform).Name));
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
                Contract.Ensures(Enum.IsDefined(typeof(ActiveUniformType), Contract.Result<ActiveUniformType>()));

                return _Type;
            }
            private set
            {
                Contract.Requires<ArgumentException>(Enum.IsDefined(typeof(ActiveUniformType), value));

                this.SetProperty(ref _Type, value);
            }
        }

        public Uniform(ShaderProgram program, int location)
        {
            Contract.Requires<ArgumentNullException>(program != null);
            Contract.Requires<ArgumentOutOfRangeException>(location >= 0);

            this.Location = location;
        }

        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                lock (initializationLock)
                {
                    if (!this.IsInitialized)
                    {
                        int nameLength;
                        ActiveUniformType uniformType;
                        base.Name = GL.GetActiveUniform(this.Program, this.Location, out nameLength, out uniformType);
                        this.Type = uniformType; // Set indirectly to fire event

                        this.IsInitialized = true;
                    }
                }
            }
        }

        public void Set(int value)
        {
            this.Initialize();
            GL.ProgramUniform1(this.Program, this.Location, value);
        }

        public void Set(uint value1)
        {
            this.Initialize();
            GL.ProgramUniform1(this.Program, this.Location, value1);
        }

        public void Set(float value)
        {
            this.Initialize();
            GL.ProgramUniform1(this.Program, this.Location, value);
        }

        public void Set(double value)
        {
            this.Initialize();
            GL.ProgramUniform1(this.Program, this.Location, value);
        }

        public void Set(int value1, int value2)
        {
            this.Initialize();
            GL.ProgramUniform2(this.Program, this.Location, value1, value2);
        }

        public void Set(uint value1, uint value2)
        {
            this.Initialize();
            GL.ProgramUniform2(this.Program, this.Location, value1, value2);
        }

        public void Set(float value1, float value2)
        {
            this.Initialize();
            GL.ProgramUniform2(this.Program, this.Location, value1, value2);
        }

        public void Set(double value1, double value2)
        {
            this.Initialize();
            GL.ProgramUniform2(this.Program, this.Location, value1, value2);
        }

        public void Set(int value1, int value2, int value3)
        {
            this.Initialize();
            GL.ProgramUniform3(this.Program, this.Location, value1, value2, value3);
        }

        public void Set(uint value1, uint value2, uint value3)
        {
            this.Initialize();
            GL.ProgramUniform3(this.Program, this.Location, value1, value2, value3);
        }

        public void Set(float value1, float value2, float value3)
        {
            this.Initialize();
            GL.ProgramUniform3(this.Program, this.Location, value1, value2, value3);
        }

        public void Set(double value1, double value2, double value3)
        {
            this.Initialize();
            GL.ProgramUniform3(this.Program, this.Location, value1, value2, value3);
        }

        public void Set(int value1, int value2, int value3, int value4)
        {
            this.Initialize();
            GL.ProgramUniform4(this.Program, this.Location, value1, value2, value3, value4);
        }

        public void Set(uint value1, uint value2, uint value3, uint value4)
        {
            this.Initialize();
            GL.ProgramUniform4(this.Program, this.Location, value1, value2, value3, value4);
        }

        public void Set(float value1, float value2, float value3, float value4)
        {
            this.Initialize();
            GL.ProgramUniform4(this.Program, this.Location, value1, value2, value3, value4);
        }

        public void Set(double value1, double value2, double value3, double value4)
        {
            this.Initialize();
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

        public void Set(Matrix value)
        {
            this.Set(value, false);
        }

        public void Set(Matrix value, bool transpose)
        {
            this.Initialize();
            GL.ProgramUniformMatrix4(this.Program, this.Location, 16, transpose, value.ToArray());
        }

        public void Set(Matrix3x2 value)
        {
            this.Set(value, false);
        }

        public void Set(Matrix3x2 value, bool transpose)
        {
            this.Initialize();
            GL.ProgramUniformMatrix3x2(this.Program, this.Location, 16, transpose, value.ToArray());
        }

        public void Set(Matrix3x3 value)
        {
            this.Set(value, false);
        }

        public void Set(Matrix3x3 value, bool transpose)
        {
            this.Initialize();
            GL.ProgramUniformMatrix3(this.Program, this.Location, 16, transpose, value.ToArray());
        }

        public void Set(Matrix5x4 value)
        {
            this.Initialize();
            GL.ProgramUniform4(this.Program, this.Location, value.M11, value.M12, value.M13, value.M14);
            GL.ProgramUniform4(this.Program, this.Location + 4, value.M21, value.M22, value.M23, value.M24);
            GL.ProgramUniform4(this.Program, this.Location + 8, value.M31, value.M32, value.M33, value.M34);
            GL.ProgramUniform4(this.Program, this.Location + 12, value.M41, value.M42, value.M43, value.M44);
            GL.ProgramUniform4(this.Program, this.Location + 16, value.M51, value.M52, value.M53, value.M54);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this._Location >= 0);
            Contract.Invariant(this._Program != null);
            Contract.Invariant(Enum.IsDefined(typeof(ActiveUniformType), _Type));
        }
    }
}
