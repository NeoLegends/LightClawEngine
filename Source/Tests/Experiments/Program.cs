using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LightClaw.Engine.IO;
using LightClaw.Extensions;
using log4net;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using ProtoBuf;

namespace Experiments
{
    class Program
    {
        static void Main(string[] args)
        {
            EffectPassReader.PassData pd = new EffectPassReader.PassData(
                "TestPass",
                new EffectPassReader.EffectSource("Effects/Basic.frag", "Effects/Basic.vert"),
                new[] { new EffectPassReader.DataEffectUniformDescription("modelViewProjectionMatrix", 1) },
                new EffectPassReader.SamplerEffectUniformDescription[] { }
            );

            using (FileStream fs = File.Create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PassData.xml")))
            {
                new DataContractSerializer(typeof(EffectPassReader.PassData)).WriteObject(fs, pd);
            }

            Console.WriteLine("Finished.");
            Console.ReadLine();
        }
    }
}
