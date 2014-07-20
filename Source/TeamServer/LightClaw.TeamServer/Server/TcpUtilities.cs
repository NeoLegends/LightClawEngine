using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LightClaw.Common;

namespace LightClaw.TeamServer.Server
{
    /// <summary>
    /// Contains a set of useful methods for communicating with clients over TCP.
    /// </summary>
    public static class TcpUtilities
    {
        /// <summary>
        /// Sends a response message in Unicode-Encoding to the client with the specified <see cref="Stream"/>.
        /// </summary>
        /// <remarks>
        /// The difference to <see cref="WritePackage(System.String)"/> is that this method will not send any data about the length of the sent <see cref="String"/>.
        /// </remarks>
        /// <param name="clientStream">The client to send the message to.</param>
        /// <param name="message">The message to send.</param>
        public static void Respond(Stream clientStream, String message)
        {
            Respond(clientStream, message, Encoding.Unicode);
        }

        /// <summary>
        /// Sends a response message in the specified <see cref="Encoding"/> to the client with the specified <see cref="Stream"/>.
        /// </summary>
        /// <remarks>
        /// The difference to <see cref="WritePackage(System.String)"/> is that this method will not send any data about the length of the sent <see cref="String"/>.
        /// </remarks>
        /// <param name="clientStream">The client to send the message to.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="encoding">The <see cref="Encoding"/> to use.</param>
        public static void Respond(Stream clientStream, String message, Encoding encoding)
        {
            if (clientStream == null)
                throw new ArgumentNullException("clientStream");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            Respond(clientStream, encoding.GetBytes(message));
        }

        /// <summary>
        /// Sends a response message to the client with the specified <see cref="Stream"/>.
        /// </summary>
        /// <remarks>
        /// The difference to <see cref="WritePackage(System.String)"/> is that this method will not send any data about the length of the sent <see cref="String"/>.
        /// </remarks>
        /// <param name="clientStream">The client to send the message to.</param>
        /// <param name="data">The data to send to the client.</param>
        public static void Respond(Stream clientStream, byte[] data)
        {
            if (clientStream == null)
                throw new ArgumentNullException("clientStream");
            if (data == null)
                throw new ArgumentNullException("data");

            clientStream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Writes a package to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The <see cref="Boolean"/> to write.</param>
        public static void WritePackage(Stream clientStream, bool value)
        {
            byte[] response = BitConverter.GetBytes(value);
            clientStream.Write(response, 0, response.Length);
        }

        /// <summary>
        /// Writes a package to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The <see cref="Byte"/>-array to write.</param>
        public static void WritePackage(Stream clientStream, byte[] value)
        {
            WritePackage(clientStream, value.LongLength);
            clientStream.Write(value, 0, value.Length);
        }

        /// <summary>
        /// Writes a package to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The <see cref="Int16"/> to write.</param>
        public static void WritePackage(Stream clientStream, short value)
        {
            byte[] response = BitConverter.GetBytes(value);
            clientStream.Write(response, 0, response.Length);
        }

        /// <summary>
        /// Writes a package to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The <see cref="UInt16"/> to write.</param>
        public static void WritePackage(Stream clientStream, ushort value)
        {
            byte[] response = BitConverter.GetBytes(value);
            clientStream.Write(response, 0, response.Length);
        }

        /// <summary>
        /// Writes a package to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The <see cref="Int32"/> to write.</param>
        public static void WritePackage(Stream clientStream, int value)
        {
            byte[] response = BitConverter.GetBytes(value);
            clientStream.Write(response, 0, response.Length);
        }

        /// <summary>
        /// Writes a package to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The <see cref="UInt32"/> to write.</param>
        public static void WritePackage(Stream clientStream, uint value)
        {
            byte[] response = BitConverter.GetBytes(value);
            clientStream.Write(response, 0, response.Length);
        }

        /// <summary>
        /// Writes a package to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The <see cref="Int16"/> to write.</param>
        public static void WritePackage(Stream clientStream, long value)
        {
            byte[] response = BitConverter.GetBytes(value);
            clientStream.Write(response, 0, response.Length);
        }

        /// <summary>
        /// Writes a package to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The <see cref="UInt16"/> to write.</param>
        public static void WritePackage(Stream clientStream, ulong value)
        {
            byte[] response = BitConverter.GetBytes(value);
            clientStream.Write(response, 0, response.Length);
        }

        /// <summary>
        /// Writes a package to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The <see cref="Single"/> to write.</param>
        public static void WritePackage(Stream clientStream, float value)
        {
            byte[] response = BitConverter.GetBytes(value);
            clientStream.Write(response, 0, response.Length);
        }

        /// <summary>
        /// Writes a package to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The <see cref="Double"/> to write.</param>
        public static void WritePackage(Stream clientStream, double value)
        {
            byte[] response = BitConverter.GetBytes(value);
            clientStream.Write(response, 0, response.Length);
        }

        /// <summary>
        /// Writes a package to the <see cref="Stream"/> that is compatible with <see cref="ReadString"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The <see cref="String"/> to write.</param>
        public static void WritePackage(Stream clientStream, String value)
        {
            WritePackage(clientStream, Encoding.Unicode.GetByteCount(value));
            WritePackage(clientStream, Encoding.Unicode.GetBytes(value));
        }

        /// <summary>
        /// Writes a package to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The <see cref="Guid"/> to write.</param>
        public static void WritePackage(Stream clientStream, Guid value)
        {
            WritePackage(clientStream, value.ToByteArray());
        }

        /// <summary>
        /// Reads a <see cref="Boolean"/> out of the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to read out the value from.</param>
        /// <returns>The read data.</returns>
        public static bool ReadBool(Stream clientStream)
        {
            byte result = (byte)clientStream.ReadByte();
            return BitConverter.ToBoolean(new byte[] { result }, 0);
        }

        /// <summary>
        /// Reads a <see cref="Byte"/>-array out of the specified <see cref="Stream"/>.
        /// </summary>
        /// <remarks>The length of the <see cref="Byte"/>s to read is determined by the first 4 <see cref="Byte"/>s in the <see cref="Stream"/></remarks>
        /// <param name="clientStream">The <see cref="Stream"/> to read out the value from.</param>
        /// <returns>The read data.</returns>
        public static byte[] ReadByteArray(Stream clientStream)
        {
            byte[] result = new byte[ReadInt32(clientStream)];
            clientStream.ReadBytes(result, result.Length);
            return result;
        }

        /// <summary>
        /// Reads an <see cref="Int16"/> out of the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to read out the value from.</param>
        /// <returns>The read data.</returns>
        public static short ReadInt16(Stream clientStream)
        {
            byte[] result = new byte[2];
            clientStream.ReadBytes(result, result.Length);
            return BitConverter.ToInt16(result, 0);
        }

        /// <summary>
        /// Reads a <see cref="UInt16"/> out of the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to read out the value from.</param>
        /// <returns>The read data.</returns>
        public static ushort ReadUInt16(Stream clientStream)
        {
            byte[] result = new byte[2];
            clientStream.ReadBytes(result, result.Length);
            return BitConverter.ToUInt16(result, 0);
        }

        /// <summary>
        /// Reads an <see cref="Int32"/> out of the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to read out the value from.</param>
        /// <returns>The read data.</returns>
        public static int ReadInt32(Stream clientStream)
        {
            byte[] result = new byte[4];
            clientStream.ReadBytes(result, result.Length);
            return BitConverter.ToInt32(result, 0);
        }

        /// <summary>
        /// Reads a <see cref="UInt32"/> out of the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to read out the value from.</param>
        /// <returns>The read data.</returns>
        public static uint ReadUInt32(Stream clientStream)
        {
            byte[] result = new byte[4];
            clientStream.ReadBytes(result, result.Length);
            return BitConverter.ToUInt32(result, 0);
        }

        /// <summary>
        /// Reads an <see cref="Int64"/> out of the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to read out the value from.</param>
        /// <returns>The read data.</returns>
        public static long ReadInt64(Stream clientStream)
        {
            byte[] result = new byte[8];
            clientStream.ReadBytes(result, result.Length);
            return BitConverter.ToInt64(result, 0);
        }

        /// <summary>
        /// Reads a <see cref="UInt64"/> out of the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to read out the value from.</param>
        /// <returns>The read data.</returns>
        public static ulong ReadUInt64(Stream clientStream)
        {
            byte[] result = new byte[8];
            clientStream.ReadBytes(result, result.Length);
            return BitConverter.ToUInt64(result, 0);
        }

        /// <summary>
        /// Reads a <see cref="Single"/> out of the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to read out the value from.</param>
        /// <returns>The read data.</returns>
        public static float ReadSingle(Stream clientStream)
        {
            byte[] result = new byte[4];
            clientStream.ReadBytes(result, result.Length);
            return BitConverter.ToUInt64(result, 0);
        }

        /// <summary>
        /// Reads a <see cref="Double"/> out of the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to read out the value from.</param>
        /// <returns>The read data.</returns>
        public static double ReadDouble(Stream clientStream)
        {
            byte[] result = new byte[8];
            clientStream.ReadBytes(result, result.Length);
            return BitConverter.ToUInt64(result, 0);
        }

        /// <summary>
        /// Reads a <see cref="String"/> out of the specified <see cref="Stream"/>.
        /// </summary>
        /// <remarks>The amount of <see cref="Char"/>acters to read is determined by the first 4 <see cref="Byte"/>s in the <see cref="Stream"/>.</remarks>
        /// <param name="clientStream">The <see cref="Stream"/> to read out the value from.</param>
        /// <returns>The read data.</returns>
        public static String ReadString(Stream clientStream)
        {
            byte[] result = new byte[ReadInt32(clientStream)];
            clientStream.ReadBytes(result, result.Length);
            return Encoding.Unicode.GetString(result);
        }

        /// <summary>
        /// Reads a <see cref="Guid"/> out of the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="clientStream">The <see cref="Stream"/> to read out the value from.</param>
        /// <returns>The read data.</returns>
        public static Guid ReadGuid(Stream clientStream)
        {
            byte[] result = new byte[16];
            clientStream.ReadBytes(result, result.Length);
            return new Guid(result);
        }
    }
}
