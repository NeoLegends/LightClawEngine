using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Threading
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeMessage : ICloneable, IEquatable<NativeMessage>
    {
        public IntPtr Handle { get; private set; }

        public uint Message { get; private set; }

        public IntPtr WParam { get; private set; }

        public IntPtr LParam { get; private set; }

        public uint Time { get; private set; }

        public Point MousePosition { get; private set; }

        public NativeMessage(
                    IntPtr handle,
                    uint message,
                    IntPtr wparam,
                    IntPtr lparam,
                    uint time,
                    Point mousePosition
                )
            : this()
        {
            this.Handle = handle;
            this.Message = message;
            this.WParam = wparam;
            this.LParam = lparam;
            this.Time = time;
            this.MousePosition = mousePosition;
        }

        public object Clone()
        {
            return new NativeMessage(this.Handle, this.Message, this.WParam, this.LParam, this.Time, this.MousePosition);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is NativeMessage) ? this.Equals((NativeMessage)obj) : false;
        }

        public bool Equals(NativeMessage other)
        {
            return (this.Handle == other.Handle) && (this.Message == other.Message) &&
                   (this.WParam == other.WParam) && (this.LParam == other.LParam) &&
                   (this.Time == other.Time) && (this.MousePosition == other.MousePosition);
        }

        public override int GetHashCode()
        {
            return HashF.GetHashCode(this.Handle, this.Message, this.WParam, this.LParam, this.Time, this.MousePosition);
        }

        public static bool operator ==(NativeMessage left, NativeMessage right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NativeMessage left, NativeMessage right)
        {
            return !(left == right);
        }
    }
}
