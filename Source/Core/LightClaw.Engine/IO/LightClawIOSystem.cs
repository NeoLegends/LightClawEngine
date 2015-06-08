using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;

namespace LightClaw.Engine.IO
{
    internal class LightClawIOSystem : IOSystem
    {
        private readonly IContentManager contentManager;

        public LightClawIOSystem(IContentManager contentManager)
        {
            Contract.Requires<ArgumentNullException>(contentManager != null);

            this.contentManager = contentManager;
        }

        public override IOStream OpenFile(string pathToFile, FileIOMode fileMode)
        {
            return new IOStreamWrapper(contentManager.GetStreamAsync(pathToFile).Result, pathToFile, fileMode);
        }

        private class IOStreamWrapper : IOStream
        {
            private readonly Stream s;

            public override bool IsValid
            {
                get 
                {
                    return true;
                }
            }

            public IOStreamWrapper(Stream s, string filePath, FileIOMode ioMode)
                : base(filePath, ioMode)
            {
                Contract.Requires<ArgumentNullException>(s != null);

                this.s = s;
            }

            public override void Flush()
            {
                this.s.Flush();
            }

            public override long GetFileSize()
            {
                return s.Length;
            }

            public override long GetPosition()
            {
                return s.Position;
            }

            public override long Read(byte[] dataRead, long count)
            {
                return s.Read(dataRead, 0, (int)count);
            }

            public override ReturnCode Seek(long offset, Origin seekOrigin)
            {
                try
                {
                    SeekOrigin so;
                    switch (seekOrigin)
	                {
		                case Origin.Current:
                            so = SeekOrigin.Current;
                           break;
                        case Origin.End:
                            so = SeekOrigin.End;
                            break;
                        case Origin.Set:
                            so = SeekOrigin.Begin;
                            break;
                        default:
                            throw new ArgumentException("Seek origin could not be converted.");
	                }
                    s.Seek(offset, so);
                    return ReturnCode.Success;
                }
                catch (IOException)
                {
                    return ReturnCode.Failure;
                }
            }

            public override long Write(byte[] dataToWrite, long count)
            {
                s.Write(dataToWrite, 0, (int)count);
                return count;
            }

            protected override void Dispose(bool disposing)
            {
                try
                {
                    s.Dispose();
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }
    }
}
