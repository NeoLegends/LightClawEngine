using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenTK;

namespace LightClaw.Engine.Core
{
    /// <summary>
    /// Represents a <see cref="JsonConverter"/> for the built-in OpenTK math types.
    /// </summary>
    public class MathConverter : JsonConverter
    {
        private static Dictionary<Type, int> serializableTypes = new Dictionary<Type, int>
        { 
            { typeof(Matrix2), 0 }, { typeof(Matrix2x3), 1 }, { typeof(Matrix2x4), 2 },
            { typeof(Matrix2d), 3 }, { typeof(Matrix2x3d), 4 }, { typeof(Matrix2x4d), 5 },

            { typeof(Matrix3), 6 }, { typeof(Matrix3x2), 7 }, { typeof(Matrix3x4), 8 },
            { typeof(Matrix3d), 9 }, { typeof(Matrix3x2d), 10 }, { typeof(Matrix3x4d), 11 },

            { typeof(Matrix4), 12 }, { typeof(Matrix4x2), 13 }, { typeof(Matrix4x3), 14 },
            { typeof(Matrix4d), 15 }, { typeof(Matrix4x2d), 16 }, { typeof(Matrix4x3d), 17 },

            { typeof(Vector2), 18 }, { typeof(Vector3), 19 }, { typeof(Vector4), 20 },
            { typeof(Vector2d), 21 }, { typeof(Vector3d), 22 }, { typeof(Vector4d), 23 },
                
            { typeof(Quaternion), 24 }, { typeof(Quaterniond), 25 }
        };

        /// <summary>
        /// Determines whether the <see cref="MathConverter"/> can convert the specified type to JSON.
        /// </summary>
        /// <param name="objectType">The <see cref="Type"/> to check for.</param>
        /// <returns><c>true</c> if instances of the specified <see cref="Type"/> can be converted to JSON, otherwise <c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return serializableTypes.ContainsKey(objectType);
        }

        /// <summary>
        /// Converts JSON to an instance of the specified <paramref name="objectType"/>.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">The <see cref="Type"/> to deserialize to.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The newly created object.</returns>
        [ContractVerification(false)]
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // We use a combination of dictionary and switch for performance here.
            // Its not the most readable way, but it is fast. This is also how the 
            // compiler implements a large switch on strings.
            int key;
            if (serializableTypes.TryGetValue(objectType, out key))
            {
                try
                {
                    switch (key)
                    {
                        case 0: // Matrix2
                            float[] fvals = serializer.Deserialize<float[]>(reader);
                            return new Matrix2(fvals[0], fvals[1], fvals[2], fvals[3]);
                        case 1: // Matrix2x3
                            fvals = serializer.Deserialize<float[]>(reader);
                            return new Matrix2x3(fvals[0], fvals[1], fvals[2], fvals[3], fvals[4], fvals[5]);
                        case 2: // Matrix2x4
                            fvals = serializer.Deserialize<float[]>(reader);
                            return new Matrix2x4(fvals[0], fvals[1], fvals[2], fvals[3], fvals[4], fvals[5], fvals[6], fvals[7]);

                        case 3: // Matrix2d
                            double[] dvals = serializer.Deserialize<double[]>(reader);
                            return new Matrix2d(dvals[0], dvals[1], dvals[2], dvals[3]);
                        case 4: // Matrix2x3d
                            dvals = serializer.Deserialize<double[]>(reader);
                            return new Matrix2x3d(dvals[0], dvals[1], dvals[2], dvals[3], dvals[4], dvals[5]);
                        case 5: // Matrix2d
                            dvals = serializer.Deserialize<double[]>(reader);
                            return new Matrix2x4d(dvals[0], dvals[1], dvals[2], dvals[3], dvals[4], dvals[5], dvals[6], dvals[7]);


                        case 6: // Matrix3
                            fvals = serializer.Deserialize<float[]>(reader);
                            return new Matrix3(fvals[0], fvals[1], fvals[2], fvals[3], fvals[4], fvals[5], fvals[6], fvals[7], fvals[8]);
                        case 7: // Matrix3x2
                            fvals = serializer.Deserialize<float[]>(reader);
                            return new Matrix3x2(fvals[0], fvals[1], fvals[2], fvals[3], fvals[4], fvals[5]);
                        case 8: // Matrix3x4
                            fvals = serializer.Deserialize<float[]>(reader);
                            return new Matrix3x4(fvals[0], fvals[1], fvals[2], fvals[3], fvals[4], fvals[5], fvals[6], fvals[7], fvals[8], fvals[9], fvals[10], fvals[11]);

                        case 9: // Matrix3d
                            dvals = serializer.Deserialize<double[]>(reader);
                            return new Matrix3d(dvals[0], dvals[1], dvals[2], dvals[3], dvals[4], dvals[5], dvals[6], dvals[7], dvals[8]);
                        case 10: // Matrix3x2d
                            dvals = serializer.Deserialize<double[]>(reader);
                            return new Matrix3x2d(dvals[0], dvals[1], dvals[2], dvals[3], dvals[4], dvals[5]);
                        case 11: // Matrix3x4d
                            dvals = serializer.Deserialize<double[]>(reader);
                            return new Matrix3x4d(dvals[0], dvals[1], dvals[2], dvals[3], dvals[4], dvals[5], dvals[6], dvals[7], dvals[8], dvals[9], dvals[10], dvals[11]);


                        case 12: // Matrix4
                            fvals = serializer.Deserialize<float[]>(reader);
                            return new Matrix4(fvals[0], fvals[1], fvals[2], fvals[3], fvals[4], fvals[5], fvals[6], fvals[7], fvals[8], fvals[9], fvals[10], fvals[11], fvals[12], fvals[13], fvals[14], fvals[15]);
                        case 13: // Matrix4x2
                            fvals = serializer.Deserialize<float[]>(reader);
                            return new Matrix4x2(fvals[0], fvals[1], fvals[2], fvals[3], fvals[4], fvals[5], fvals[6], fvals[7]);
                        case 14: // Matrix4x3
                            fvals = serializer.Deserialize<float[]>(reader);
                            return new Matrix4x3(fvals[0], fvals[1], fvals[2], fvals[3], fvals[4], fvals[5], fvals[6], fvals[7], fvals[8], fvals[9], fvals[10], fvals[11]);

                        case 15: // Matrix4d
                            dvals = serializer.Deserialize<double[]>(reader);
                            return new Matrix4d(dvals[0], dvals[1], dvals[2], dvals[3], dvals[4], dvals[5], dvals[6], dvals[7], dvals[8], dvals[9], dvals[10], dvals[11], dvals[12], dvals[13], dvals[14], dvals[15]);
                        case 16: // Matrix4x2d
                            dvals = serializer.Deserialize<double[]>(reader);
                            return new Matrix4x2d(dvals[0], dvals[1], dvals[2], dvals[3], dvals[4], dvals[5], dvals[6], dvals[7]);
                        case 17: // Matrix4x3d
                            dvals = serializer.Deserialize<double[]>(reader);
                            return new Matrix4x3d(dvals[0], dvals[1], dvals[2], dvals[3], dvals[4], dvals[5], dvals[6], dvals[7], dvals[8], dvals[9], dvals[10], dvals[11]);


                        case 18: // Vector2
                            fvals = serializer.Deserialize<float[]>(reader);
                            return new Vector2(fvals[0], fvals[1]);
                        case 19: // Vector3
                            fvals = serializer.Deserialize<float[]>(reader);
                            return new Vector3(fvals[0], fvals[1], fvals[2]);
                        case 20: // Vector4
                            fvals = serializer.Deserialize<float[]>(reader);
                            return new Vector4(fvals[0], fvals[1], fvals[2], fvals[3]);

                        case 21: // Vector2d
                            dvals = serializer.Deserialize<double[]>(reader);
                            return new Vector2d(dvals[0], dvals[1]);
                        case 22: // Vector3d
                            dvals = serializer.Deserialize<double[]>(reader);
                            return new Vector3d(dvals[0], dvals[1], dvals[2]);
                        case 23: // Vector4d
                            dvals = serializer.Deserialize<double[]>(reader);
                            return new Vector4d(dvals[0], dvals[1], dvals[2], dvals[3]);


                        case 24: // Quaterion
                            fvals = serializer.Deserialize<float[]>(reader);
                            return new Quaternion(fvals[0], fvals[1], fvals[2], fvals[3]);
                        case 25: // Quateriond
                            dvals = serializer.Deserialize<double[]>(reader);
                            return new Quaterniond(dvals[0], dvals[1], dvals[2], dvals[3]);

                        default:
                            throw new NotSupportedException("Unknown type to deserialize!");
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    throw new JsonException(
                        string.Format(
                            "A {0} should have been deserialized, but the array of floats that should have held the values was too small.",
                            objectType.Name
                        ),
                        ex
                    );
                }
            }
            else
            {
                return serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Writes the specified <paramref name="value"/> to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value to write to JSON.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            // We use a combination of dictionary and switch for performance here.
            // Its not the most readable way, but it is fast. This is also how the 
            // compiler implements a large switch on strings.
            int key;
            if (serializableTypes.TryGetValue(value.GetType(), out key))
            {
                writer.WriteStartArray();
                try
                {
                    switch (key)
                    {
                        case 0: // Matrix2
                            Matrix2 m2 = (Matrix2)value;
                            writer.WriteValue(m2.M11);
                            writer.WriteValue(m2.M12);
                            writer.WriteValue(m2.M21);
                            writer.WriteValue(m2.M22);
                            break;
                        case 1: // Matrix2x3
                            Matrix2x3 m23 = (Matrix2x3)value;
                            writer.WriteValue(m23.M11);
                            writer.WriteValue(m23.M12);
                            writer.WriteValue(m23.M13);
                            writer.WriteValue(m23.M21);
                            writer.WriteValue(m23.M22);
                            writer.WriteValue(m23.M23);
                            break;
                        case 2: // Matrix2x4
                            Matrix2x4 m24 = (Matrix2x4)value;
                            writer.WriteValue(m24.M11);
                            writer.WriteValue(m24.M12);
                            writer.WriteValue(m24.M13);
                            writer.WriteValue(m24.M14);
                            writer.WriteValue(m24.M21);
                            writer.WriteValue(m24.M22);
                            writer.WriteValue(m24.M23);
                            writer.WriteValue(m24.M24);
                            break;

                        case 3: // Matrix2d
                            Matrix2d m2d = (Matrix2d)value;
                            writer.WriteValue(m2d.M11);
                            writer.WriteValue(m2d.M12);
                            writer.WriteValue(m2d.M21);
                            writer.WriteValue(m2d.M22);
                            break;
                        case 4: // Matrix2x3d
                            Matrix2x3d m23d = (Matrix2x3d)value;
                            writer.WriteValue(m23d.M11);
                            writer.WriteValue(m23d.M12);
                            writer.WriteValue(m23d.M13);
                            writer.WriteValue(m23d.M21);
                            writer.WriteValue(m23d.M22);
                            writer.WriteValue(m23d.M23);
                            break;
                        case 5: // Matrix2d
                            Matrix2x4d m24d = (Matrix2x4d)value;
                            writer.WriteValue(m24d.M11);
                            writer.WriteValue(m24d.M12);
                            writer.WriteValue(m24d.M13);
                            writer.WriteValue(m24d.M14);
                            writer.WriteValue(m24d.M21);
                            writer.WriteValue(m24d.M22);
                            writer.WriteValue(m24d.M23);
                            writer.WriteValue(m24d.M24);
                            break;


                        case 6: // Matrix3
                            Matrix3 m3 = (Matrix3)value;
                            writer.WriteValue(m3.M11);
                            writer.WriteValue(m3.M12);
                            writer.WriteValue(m3.M13);
                            writer.WriteValue(m3.M21);
                            writer.WriteValue(m3.M22);
                            writer.WriteValue(m3.M23);
                            writer.WriteValue(m3.M31);
                            writer.WriteValue(m3.M32);
                            writer.WriteValue(m3.M33);
                            break;
                        case 7: // Matrix3x2
                            Matrix3x2 m32 = (Matrix3x2)value;
                            writer.WriteValue(m32.M11);
                            writer.WriteValue(m32.M12);
                            writer.WriteValue(m32.M21);
                            writer.WriteValue(m32.M22);
                            writer.WriteValue(m32.M31);
                            writer.WriteValue(m32.M32);
                            break;
                        case 8: // Matrix3x4
                            Matrix3x4 m34 = (Matrix3x4)value;
                            writer.WriteValue(m34.M11);
                            writer.WriteValue(m34.M12);
                            writer.WriteValue(m34.M13);
                            writer.WriteValue(m34.M14);
                            writer.WriteValue(m34.M21);
                            writer.WriteValue(m34.M22);
                            writer.WriteValue(m34.M23);
                            writer.WriteValue(m34.M24);
                            writer.WriteValue(m34.M31);
                            writer.WriteValue(m34.M32);
                            writer.WriteValue(m34.M33);
                            writer.WriteValue(m34.M34);
                            break;

                        case 9: // Matrix3d
                            Matrix3d m3d = (Matrix3d)value;
                            writer.WriteValue(m3d.M11);
                            writer.WriteValue(m3d.M12);
                            writer.WriteValue(m3d.M13);
                            writer.WriteValue(m3d.M21);
                            writer.WriteValue(m3d.M22);
                            writer.WriteValue(m3d.M23);
                            writer.WriteValue(m3d.M31);
                            writer.WriteValue(m3d.M32);
                            writer.WriteValue(m3d.M33);
                            break;
                        case 10: // Matrix3x2d
                            Matrix3x2d m32d = (Matrix3x2d)value;
                            writer.WriteValue(m32d.M11);
                            writer.WriteValue(m32d.M12);
                            writer.WriteValue(m32d.M21);
                            writer.WriteValue(m32d.M22);
                            writer.WriteValue(m32d.M31);
                            writer.WriteValue(m32d.M32);
                            break;
                        case 11: // Matrix3x4d
                            Matrix3x4d m34d = (Matrix3x4d)value;
                            writer.WriteValue(m34d.M11);
                            writer.WriteValue(m34d.M12);
                            writer.WriteValue(m34d.M13);
                            writer.WriteValue(m34d.M14);
                            writer.WriteValue(m34d.M21);
                            writer.WriteValue(m34d.M22);
                            writer.WriteValue(m34d.M23);
                            writer.WriteValue(m34d.M24);
                            writer.WriteValue(m34d.M31);
                            writer.WriteValue(m34d.M32);
                            writer.WriteValue(m34d.M33);
                            writer.WriteValue(m34d.M34);
                            break;


                        case 12: // Matrix4
                            Matrix4 m4 = (Matrix4)value;
                            writer.WriteValue(m4.M11);
                            writer.WriteValue(m4.M12);
                            writer.WriteValue(m4.M13);
                            writer.WriteValue(m4.M14);
                            writer.WriteValue(m4.M21);
                            writer.WriteValue(m4.M22);
                            writer.WriteValue(m4.M23);
                            writer.WriteValue(m4.M24);
                            writer.WriteValue(m4.M31);
                            writer.WriteValue(m4.M32);
                            writer.WriteValue(m4.M33);
                            writer.WriteValue(m4.M34);
                            writer.WriteValue(m4.M41);
                            writer.WriteValue(m4.M42);
                            writer.WriteValue(m4.M43);
                            writer.WriteValue(m4.M44);
                            break;
                        case 13: // Matrix4x2
                            Matrix4x2 m42 = (Matrix4x2)value;
                            writer.WriteValue(m42.M11);
                            writer.WriteValue(m42.M12);
                            writer.WriteValue(m42.M21);
                            writer.WriteValue(m42.M22);
                            writer.WriteValue(m42.M31);
                            writer.WriteValue(m42.M32);
                            writer.WriteValue(m42.M41);
                            writer.WriteValue(m42.M42);
                            break;
                        case 14: // Matrix4x3
                            Matrix4x3 m43 = (Matrix4x3)value;
                            writer.WriteValue(m43.M11);
                            writer.WriteValue(m43.M12);
                            writer.WriteValue(m43.M13);
                            writer.WriteValue(m43.M21);
                            writer.WriteValue(m43.M22);
                            writer.WriteValue(m43.M23);
                            writer.WriteValue(m43.M31);
                            writer.WriteValue(m43.M32);
                            writer.WriteValue(m43.M33);
                            writer.WriteValue(m43.M41);
                            writer.WriteValue(m43.M42);
                            writer.WriteValue(m43.M43);
                            break;

                        case 15: // Matrix4d
                            Matrix4d m4d = (Matrix4d)value;
                            writer.WriteValue(m4d.M11);
                            writer.WriteValue(m4d.M12);
                            writer.WriteValue(m4d.M13);
                            writer.WriteValue(m4d.M14);
                            writer.WriteValue(m4d.M21);
                            writer.WriteValue(m4d.M22);
                            writer.WriteValue(m4d.M23);
                            writer.WriteValue(m4d.M24);
                            writer.WriteValue(m4d.M31);
                            writer.WriteValue(m4d.M32);
                            writer.WriteValue(m4d.M33);
                            writer.WriteValue(m4d.M34);
                            writer.WriteValue(m4d.M41);
                            writer.WriteValue(m4d.M42);
                            writer.WriteValue(m4d.M43);
                            writer.WriteValue(m4d.M44);
                            break;
                        case 16: // Matrix4x2d
                            Matrix4x2d m42d = (Matrix4x2d)value;
                            writer.WriteValue(m42d.M11);
                            writer.WriteValue(m42d.M12);
                            writer.WriteValue(m42d.M21);
                            writer.WriteValue(m42d.M22);
                            writer.WriteValue(m42d.M31);
                            writer.WriteValue(m42d.M32);
                            writer.WriteValue(m42d.M41);
                            writer.WriteValue(m42d.M42);
                            break;
                        case 17: // Matrix4x3d
                            Matrix4x3d m43d = (Matrix4x3d)value;
                            writer.WriteValue(m43d.M11);
                            writer.WriteValue(m43d.M12);
                            writer.WriteValue(m43d.M13);
                            writer.WriteValue(m43d.M21);
                            writer.WriteValue(m43d.M22);
                            writer.WriteValue(m43d.M23);
                            writer.WriteValue(m43d.M31);
                            writer.WriteValue(m43d.M32);
                            writer.WriteValue(m43d.M33);
                            writer.WriteValue(m43d.M41);
                            writer.WriteValue(m43d.M42);
                            writer.WriteValue(m43d.M43);
                            break;


                        case 18: // Vector2
                            Vector2 v2 = (Vector2)value;
                            writer.WriteValue(v2.X);
                            writer.WriteValue(v2.Y);
                            break;
                        case 19: // Vector3
                            Vector3 v3 = (Vector3)value;
                            writer.WriteValue(v3.X);
                            writer.WriteValue(v3.Y);
                            writer.WriteValue(v3.Z);
                            break;
                        case 20: // Vector4
                            Vector4 v4 = (Vector4)value;
                            writer.WriteValue(v4.X);
                            writer.WriteValue(v4.Y);
                            writer.WriteValue(v4.Z);
                            writer.WriteValue(v4.W);
                            break;

                        case 21: // Vector2d
                            Vector2d v2d = (Vector2d)value;
                            writer.WriteValue(v2d.X);
                            writer.WriteValue(v2d.Y);
                            break;
                        case 22: // Vector3d
                            Vector3d v3d = (Vector3d)value;
                            writer.WriteValue(v3d.X);
                            writer.WriteValue(v3d.Y);
                            writer.WriteValue(v3d.Z);
                            break;
                        case 23: // Vector4d
                            Vector4d v4d = (Vector4d)value;
                            writer.WriteValue(v4d.X);
                            writer.WriteValue(v4d.Y);
                            writer.WriteValue(v4d.Z);
                            writer.WriteValue(v4d.W);
                            break;


                        case 24: // Quaterion
                            Quaternion q = (Quaternion)value;
                            writer.WriteValue(q.X);
                            writer.WriteValue(q.Y);
                            writer.WriteValue(q.Z);
                            writer.WriteValue(q.W);
                            break;
                        case 25: // Quateriond
                            Quaterniond qd = (Quaterniond)value;
                            writer.WriteValue(qd.X);
                            writer.WriteValue(qd.Y);
                            writer.WriteValue(qd.Z);
                            writer.WriteValue(qd.W);
                            break;


                        default:
                            throw new NotSupportedException("Unknown type to deserialize!");
                    }
                }
                finally
                {
                    writer.WriteEndArray();
                }
            }
            else
            {
                if (serializer != null)
                {
                    serializer.Serialize(writer, value);
                }
            }
        }
    }
}
