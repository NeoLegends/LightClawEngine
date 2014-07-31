using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightClaw.Engine.Core;

namespace LightClaw.Engine.Graphics
{
    public class MeshData
    {
        public Vector3[] Normals { get; private set; }

        public Vector2[] TextureCoordinates { get; private set; }

        public Vector3[] Vertices { get; private set; }

        public void Save()
        {
            //byte[] normalData = this.Copy(this.Normals);
            //byte[] texCoordData = this.Copy(this.TextureCoordinates);
            //byte[] vertexData = this.Copy(this.Vertices);
        }

        //private unsafe byte[] Copy(Vector2[] source)
        //{
        //    byte[] result = new byte[source.Length * Vector2.SizeInBytes];
        //    fixed (Vector2* pVertices = source)
        //    {
        //        byte* pVertexData = (byte*)&pVertices[0];
        //        for (int i = 0; i < result.Length; i++)
        //        {
        //            result[i] = pVertexData[i];
        //        }
        //    }
        //    return result;
        //}

        //private unsafe byte[] Copy(Vector3[] source)
        //{
        //    byte[] result = new byte[source.Length * Vector3.SizeInBytes];
        //    fixed (Vector3* pVertices = source)
        //    {
        //        byte* pVertexData = (byte*)&pVertices[0];
        //        for (int i = 0; i < result.Length; i++)
        //        {
        //            result[i] = pVertexData[i];
        //        }
        //    }
        //    return result;
        //}
    }
}
